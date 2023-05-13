using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Contact
{
    public class ContactEditInput
    {
        public Guid Id { get; set; }

        [Display(Name = "Инифиалы")]
        [MinLength(6, ErrorMessage = "Short name must be at least {1} characters long")]
        [MaxLength(30, ErrorMessage = "Short name cannot exceed {1} characters")]
        public string? Name { get; set; }

        [Display(Name = "ФИО")]
        [Required(ErrorMessage = "Обязательно")]
        [MinLength(6, ErrorMessage = "Full name must be at least {1} characters long")]
        [MaxLength(50, ErrorMessage = "Full name cannot exceed {1} characters")]
        public string FullName { get; set; }

        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [Display(Name = "Возраст")]
        public int Age { get; set; }

        [Display(Name = "Дата рождения")]
        [Required(ErrorMessage = "Обязательно")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Мобильный телефон")]
        public string? MobilePhone { get; set; }

        [Display(Name = "Рабочий телефон")]
        public string? Phone { get; set; }
    }
}
