using System.ComponentModel.DataAnnotations;

namespace WebApp.Entities
{
    public class Contact : BaseEntity
    {
        [Display(Name = "Инициалы")]
        public string Name { get; set; }

        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Возраст")]
        public int Age { get; set; }

        [Display(Name = "Дата рождения")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Мобильный телефон")]
        public string? MobilePhone { get; set; }

        [Display(Name = "Рабочий телефон")]
        public string? Phone { get; set; }

        [Display(Name = "Фото")]
        public byte[]? Image { get; set; }

        [Display(Name = "Тип контакта")]
        public ContactType ContactType { get; set; }

        [Display(Name = "Пол")]
        public Gender? Gender { get; set; }
    }
}
