using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEBlaba2.models;
using WEBlaba2.Services;

namespace WEBlaba2.Pages.catalog
{
    public class Alan_Hadash_Brazilian_MurumuruModel : AuthPageModel
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }

        public Alan_Hadash_Brazilian_MurumuruModel(SessionService sessionService, ClientService authService, ProductService productService, CartService cartService) : base(sessionService)
        {
            _authService = authService;
            _productService = productService;
            _cartService = cartService;
        }

        private readonly ProductService _productService;
        private readonly CartService _cartService;
        private readonly ClientService _authService;

        [BindProperty]
        public LoginModel LoginData { get; set; }

        [BindProperty]
        public int Quantity { get; set; } = 1;

        public string Message { get; set; }
        public Product? product { get; set; }

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

        public async Task<IActionResult> OnPostAddToCartAsync(int productId, int quantity)
        {
            var clientId = _sessionService.GetCurrentUserId();

            if (!clientId.HasValue)
            {
                return RedirectToPage("/auth/Authorization", new { returnUrl = Request.Path });
            }

            try
            {
                // ИСПРАВЛЕНО: Получаем товар по ID вместо имени
                var product = await _productService.GetProductByIdAsync(6); // ID из логов
                if (product == null || product.count < quantity)
                {
                    Message = "Недостаточно товара на складе";
                    return await OnGet();
                }

                await _cartService.AddToCartAsync(product.ProductId, clientId.Value, quantity);

                Message = "Товар успешно добавлен в корзину!";
                return await OnGet();
            }
            catch (Exception ex)
            {
                Message = $"Ошибка при добавлении в корзину: {ex.Message}";
                return await OnGet();
            }
        }

        public async Task<IActionResult> OnGet()
        {
            IsAuthenticated = _sessionService.IsAuthenticated();

            // ИСПРАВЛЕНО: Получаем товар по ID вместо имени
            product = await _productService.GetProductByIdAsync(6); // ID = 6 из логов

            if (product == null)
            {
                // Если по ID не нашли, пробуем по имени как запасной вариант
                product = await _productService.SearcOneByNameAsync("Alan Hadash Brazilian Murumuru Hair Сonditioner");

                if (product == null)
                {
                    product = new Product();
                    product.count = 0;
                    product.ProductId = 6; // Устанавливаем правильный ID
                }
            }

            if (IsAuthenticated)
            {
                UserName = _sessionService.GetCurrentUserName();
            }

            return Page();
        }
    }
}