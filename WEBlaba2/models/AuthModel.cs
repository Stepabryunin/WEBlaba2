using System.ComponentModel.DataAnnotations;

namespace WEBlaba2.models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Логин или Email обязателен")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        public string Password { get; set; }
    }
}