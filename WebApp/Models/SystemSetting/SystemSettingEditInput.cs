using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.SystemSetting
{
    public class SystemSettingEditInput<T>
    {
        public Guid Id { get; set; }

        [Display(Name = "Название")]
        public string? Name { get; set; }

        [Display(Name = "Код")]
        public string? Code { get; set; }

        [Display(Name = "Значение")]
        [Required]
        public T Value { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }
    }
}
