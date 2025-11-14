using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEBlaba2.models;
using WEBlaba2.Services;

namespace WEBlaba2.Pages
{
    public class contactsModel : AuthPageModel
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }

        private readonly ClientService _authService;
        public contactsModel(SessionService sessionService, ClientService authService) : base(sessionService)
        {
            _authService = authService;
        }
        [BindProperty]
        public LoginModel LoginData { get; set; }

        public string Message { get; set; }

        public IActionResult OnGet()
        {
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
