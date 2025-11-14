using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEBlaba2.models;
using WEBlaba2.Services;

namespace WEBlaba2.Pages
{
    public class AuthPageModel : PageModel
    {
        protected readonly SessionService _sessionService;

        public AuthPageModel(SessionService sessionService)
        {
            _sessionService = sessionService;
        }

        // Проверка авторизации при загрузке страницы
        public IActionResult CheckAuth()
        {
            if (!_sessionService.IsAuthenticated())
            {
                return RedirectToPage("/auth/Authorization", new { returnUrl = Request.Path });
            }
            return null;
        }

        // Получение текущего пользователя
        public Client CurrentUser => _sessionService.GetCurrentUser();
    }
}