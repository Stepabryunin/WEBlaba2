using WEBlaba2.context;
using WEBlaba2.models;

namespace WEBlaba2.Services
{
    public class SessionService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ProductContext _db;

        public SessionService(IHttpContextAccessor httpContextAccessor, ProductContext db)
        {
            _httpContextAccessor = httpContextAccessor;
            _db = db;
        }

        public bool IsAuthenticated()
        {
            return GetCurrentUserId().HasValue;
        }

        public int? GetCurrentUserId()
        {
            return _httpContextAccessor.HttpContext.Session.GetInt32("UserId");
        }

        public string GetCurrentUserName()
        {
            return _httpContextAccessor.HttpContext.Session.GetString("UserName");
        }

        public Client GetCurrentUser()
        {
            var userId = GetCurrentUserId();
            if (userId.HasValue)
            {
                return _db.Clients.FirstOrDefault(c => c.ClientId == userId.Value);
            }
            return null;
        }

        public void Logout()
        {
            _httpContextAccessor.HttpContext.Session.Clear();
        }
    }
}