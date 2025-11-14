using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEBlaba2.Services;
using WEBlaba2.models;

namespace WEBlaba2.Pages.auth
{
    public class registrationModel : PageModel
    {
        private readonly ClientService _authService;

        public registrationModel(ClientService clientService)
        {
            _authService = clientService;
        }

        [BindProperty]
        public Client ClientData { get; set; }

        public string Message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            // Проверяем согласия
            var agreeOferta = Request.Form["agreeOferta"] == "on";
            var agreePrivacy = Request.Form["agreePrivacy"] == "on";
            var agreeRules = Request.Form["agreeRules"] == "on";

            if (!agreeOferta || !agreePrivacy || !agreeRules)
            {
                Message = "Необходимо принять все соглашения";
                return Page();
            }

            if (!ModelState.IsValid)
            {
                Message = "Пожалуйста, исправьте ошибки в форме";
                return Page();
            }

            try
            {
                var result = await _authService.RegisterAsync(
                    ClientData.FirstName,
                    ClientData.LastName,
                    ClientData.MiddleName,
                    ClientData.Phone,
                    ClientData.Email,
                    ClientData.Login,
                    ClientData.Password
                );

                Console.WriteLine($"{result.Message}");

                if (result.Success)
                {
                    return RedirectToPage("/auth/Authorization", new { message = result.Message });
                }

                Message = result.Message;
                return Page();
            }
            catch (Exception ex)
            {
                Message = $"Произошла ошибка при регистрации: {ex.Message}";
                return Page();
            }
        }
    }
}