using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WEBlaba2.Services;

namespace WEBlaba2.Pages
{
    public class LogoutModel : AuthPageModel
    {
        public bool IsAuthenticated { get; set; }
        public string UserName { get; set; }

        public LogoutModel(SessionService sessionService) : base(sessionService)
        {
        }

        public IActionResult OnGet(string returnUrl = null)
        {
            IsAuthenticated = _sessionService.IsAuthenticated();

            if (IsAuthenticated)
            {
                _sessionService.Logout();
                UserName = _sessionService.GetCurrentUserName();
            }
            if (!string.IsNullOrEmpty(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            return RedirectToPage("/Index");


        }
    }
}
