using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc;
using WEBlaba2.models;
using WEBlaba2.Services;

namespace WEBlaba2.Pages.auth
{
    public class AuthorizationModel : PageModel
    {
        private readonly ClientService _authService;

        public AuthorizationModel(ClientService clientService)
        {
            _authService = clientService;
        }

        [BindProperty]
        public LoginModel LoginData { get; set; }

        [BindProperty(SupportsGet = true)] // Добавьте SupportsGet = true
        public string ReturnUrl { get; set; }

        public string Message { get; set; }

        public void OnGet(string returnUrl = null, string message = null)
        {
            ReturnUrl = returnUrl;
            if (!string.IsNullOrEmpty(message))
            {
                Message = message;
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Уберите проверку ModelState.IsValid или обрабатывайте ее точечно
            if (string.IsNullOrEmpty(LoginData.Login) || string.IsNullOrEmpty(LoginData.Password))
            {
                Message = "Пожалуйста, заполните все обязательные поля";
                return Page();
            }

            try
            {
                var result = await _authService.LoginAsync(
                    LoginData.Login,
                    LoginData.Password
                );

                if (result.Success)
                {
                    Console.WriteLine($"{result.Message}");

                    // Сохраняем данные в сессии
                    HttpContext.Session.SetInt32("UserId", result.Client.ClientId);
                    HttpContext.Session.SetString("UserName", result.Client.FirstName);
                    HttpContext.Session.SetString("UserEmail", result.Client.Email);

                    Console.WriteLine($"Пользователь {result.Client.FirstName} успешно авторизован");

                    // Редирект на returnUrl или на главную
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return LocalRedirect(ReturnUrl);
                    }
                    return RedirectToPage("/Index");
                }
                else
                {
                    Console.WriteLine($"Ошибка авторизации: {result.Message}");
                    Message = result.Message;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Исключение при авторизации: {ex.Message}");
                Message = "Произошла ошибка при авторизации. Попробуйте позже.";
            }

            return Page();
        }
    }
}