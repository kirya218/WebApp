using System.ComponentModel.DataAnnotations;

namespace WebApp.Entities
{
    public class Schedule : BaseLookup
    {
        [Display(Name = "Начальная дата")]
        public DateTime StartDate { get; set; }

        [Display(Name = "Конечная дата")]
        public DateTime EndDate { get; set; }

        [Display(Name = "Ответсвенный")]
        public Contact? Owner { get; set; }

        [Display(Name = "Пациент")]
        public Contact? Patient { get; set; }

        [Display(Name = "Процедура")]
        public Procedure? Procedure { get; set; }

        [Display(Name = "Весь день")]
        public bool IsAllDay { get; set; }

        [Display(Name = "Колор")]
        public string? Color { get; set; }
    }
}
