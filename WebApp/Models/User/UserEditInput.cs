using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.User
{
    public class UserEditInput
    {
        public Guid Id { get; set; }

        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Display(Name = "Никнейм")]
        public string UserName { get; set; }

        [Display(Name = "Пароль")]
        [UIHint("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Потверждения пароля")]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        [UIHint("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Заблокирован")]
        public bool IsBlocked { get; set; }

        [Display(Name = "Роль")]
        [UIHint("Lookup")]
        public Guid Role { get; set; }

        [Display(Name = "Контакт")]
        [UIHint("Lookup")]
        public Guid Contact { get; set; }
    }
}
