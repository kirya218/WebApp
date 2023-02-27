using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Auth
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Не указано имя")]
        [MaxLength(50, ErrorMessage = "Длина имени не может превышать {1} символов")]
        [RegularExpression(@"[А-ЯЁа-яё]{1,40}\s[А-ЯЁа-яё]{1,40}\s[А-ЯЁа-яё]{1,40}", ErrorMessage = "Используйте символы от А до Я, а также укажите полное ФИО")]
        public string NickName { get; set; }

        [Required(ErrorMessage = "Не указан логин")]
        [MinLength(6, ErrorMessage = "Логин должен быть не короче {1} символов")]
        [MaxLength(30, ErrorMessage = "Длина логина не может превышать {1} символов")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [MinLength(6, ErrorMessage = "Пароль должен быть не короче {1} символов")]
        [MaxLength(30, ErrorMessage = "Длина пароля не может превышать {1} символов")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Не указан пароль")]
        [Compare("Password", ErrorMessage = "Пароль введен неверно")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
    }
}
