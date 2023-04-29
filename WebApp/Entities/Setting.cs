using System.ComponentModel.DataAnnotations;

namespace WebApp.Entities
{
    public class Setting : BaseLookup
    {
        [Display(Name = "Код")]
        public string Code { get; set; }

        [Display(Name = "Тип")]
        public string Type { get; set; }

        [Display(Name = "Значение")]
        public SettingValue SettingValue { get; set; }
    }
}
