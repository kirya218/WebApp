using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.User
{
    public class UserAddInput
    {
        [Display(Name = "Логин")]
        [Required(ErrorMessage = "Обязательно")]
        public string Login { get; set; }

        [Display(Name = "Никнейм")]
        [Required(ErrorMessage = "Обязательно")]
        public string UserName { get; set; }

        [Display(Name = "Пароль")]
        [Required(ErrorMessage = "Обязательно")]
        [UIHint("Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "Потверждения пароля")]
        [Required(ErrorMessage = "Обязательно")]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        [UIHint("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }

        [Display(Name = "Заблокирован")]
        public bool IsBlocked { get; set; }

        [Display(Name = "Роль")]
        [UIHint("Lookup")]
        [Required(ErrorMessage = "Обязательно")]
        public Guid Role { get; set; }

        [Display(Name = "Контакт")]
        [UIHint("Lookup")]
        [Required(ErrorMessage = "Обязательно")]
        public Guid Contact { get; set; }
    }
}
