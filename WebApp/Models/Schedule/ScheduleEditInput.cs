using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApp.Models.Schedule
{
    public class ScheduleEditInput
    {
        public Guid Id { get; set; }

        [DisplayName("Весь день")]
        public bool IsAllDay { get; set; }

        [Required(ErrorMessage = "Обязательно")]
        [DisplayName("Заголовок")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Обязательно")]
        [DisplayName("Дата начала")]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessage = "Обязательно")]
        [DisplayName("Дата окончания")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Время начала")]
        [UIHint("TimePickerm")]
        public DateTime StartTime { get; set; }

        [DisplayName("Время окончания")]
        [UIHint("TimePickerm")]
        public DateTime EndTime { get; set; }

        [DisplayName("Ответственный")]
        [UIHint("Lookup")]
        public Guid? Owner { get; set; }

        [DisplayName("Постоялец")]
        [UIHint("Lookup")]
        public Guid? Patient { get; set; }

        [DisplayName("Процедура")]
        [UIHint("Lookup")]
        public Guid? Procedure { get; set; }

        [DisplayName("Цвет")]
        [UIHint("ColorDropdown")]
        public string Color { get; set; }

        [DisplayName("Комментарий")]
        [UIHint("Textarea")]
        public string? Description { get; set; }
    }
}
