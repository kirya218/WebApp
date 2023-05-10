using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.SystemSetting
{
    public class SystemSettingEditInput
    {
        public Guid Id { get; set; }

        [Display(Name = "Название")]
        public string? Name { get; set; }

        [Display(Name = "Код")]
        public string? Code { get; set; }

        [Display(Name = "Значение")]
        public Guid? GuidValue { get; set; }

        [Display(Name = "Значение")]
        public string? StringValue { get; set; }

        [Display(Name = "Значение")]
        public DateTime? DateTimeValue { get; set; }

        [Display(Name = "Значение")]
        public bool? BoolValue { get; set; }

        [Display(Name = "Значение")]
        public float? FloatValue { get; set; }

        [Display(Name = "Значение")]
        public int? IntValue { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }
    }
}
