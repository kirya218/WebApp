using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Contact
{
    public class ContactAddInput
    {
        public Guid Id { get; set; }

        [Display(Name = "Инициалы")]
        [MinLength(6, ErrorMessage = "Short name must be at least {1} characters long")]
        [MaxLength(30, ErrorMessage = "Short name cannot exceed {1} characters")]
        public string? Name { get; set; }

        [Display(Name = "ФИО")]
        [Required(ErrorMessage = "Full name is required")]
        [MinLength(6, ErrorMessage = "Full name must be at least {1} characters long")]
        [MaxLength(50, ErrorMessage = "Full name cannot exceed {1} characters")]
        public string FullName { get; set; }

        [Display(Name = "E-mail")]
        public string? Email { get; set; }

        [Display(Name = "Возраст")]
        public int Age { get; set; }

        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Мобильный телефон")]
        public string? MobilePhone { get; set; }

        [Display(Name = "Рабочий телефон")]
        public string? Phone { get; set; }

        [Display(Name = "Роль")]
        [UIHint("Lookup")]
        [Required(ErrorMessage = "Contact type is required")]
        public Guid ContactType { get; set; }
    }
}
