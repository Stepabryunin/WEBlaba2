using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEBlaba2.models;
using WEBlaba2.Services;

namespace WEBlaba2.Pages
{
    public class CatalogModel : AuthPageModel
    {
        private readonly ProductService _productService;

        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }

        public CatalogModel(ProductService productService, SessionService sessionService, ClientService authService) : base(sessionService)
        {
            _productService = productService;
            _authService = authService;
        }
        private readonly ClientService _authService;

        [BindProperty]
        public LoginModel LoginData { get; set; }
        public string Message { get; set; }

        [BindProperty(SupportsGet = true)]
        public string SearchQuery { get; set; }

        [BindProperty(SupportsGet = true)]
        public string Category { get; set; } = "Все";

        public List<Product> Products { get; set; } = new List<Product>();
        public string SearchMessage { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            
            // Получаем все продукты
            var allProducts = _productService.GetAllProducts();

            // Применяем поиск, если есть запрос
            if (!string.IsNullOrEmpty(SearchQuery))
            {
                SearchQuery = SearchQuery.TrimStart();
                allProducts = await _productService.SearchByNameAsync(SearchQuery);

                SearchMessage = allProducts.Count > 0
                    ? $"Найдено товаров '{SearchQuery}': {allProducts.Count}"
                    : $"Товаров '{SearchQuery}' не найдено";
            }

            // Применяем фильтрацию по категории
            if (Category != "Все")
            {
                Products = allProducts.Where(p => p.Category == Category).ToList();
            }
            else
            {
                Products = allProducts;
            }

            // Добавляем сообщение о категории, если не "Все"
            if (Category != "Все" && string.IsNullOrEmpty(SearchMessage))
            {
                SearchMessage = $"Категория: {Category}";
            }

            IsAuthenticated = _sessionService.IsAuthenticated();

            if (IsAuthenticated)
            {
                UserName = _sessionService.GetCurrentUserName();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var result = await _authService.LoginAsync(
                LoginData.Login,
                LoginData.Password
            );

            if (result.Success)
            {
                Console.WriteLine($"{result.Message}");
                HttpContext.Session.SetInt32("UserId", result.Client.ClientId);
                HttpContext.Session.SetString("UserName", result.Client.FirstName);

                return RedirectToPage("/Index");
            }
            else
            {
                Console.WriteLine($"{result.Message}");
            }

            Message = result.Message;
            return Page();
        }
    }
}