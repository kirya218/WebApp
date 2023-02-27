using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace WebApp.Models.Schedule
{
    public class ScheduleEditInput
    {
        public Guid Id { get; set; }

        [DisplayName("All day")]
        public bool IsAllDay { get; set; }

        [Required]
        public string Title { get; set; }

        [Required]
        [DisplayName("Start date")]
        public DateTime? StartDate { get; set; }

        [Required]
        [DisplayName("End date")]
        public DateTime? EndDate { get; set; }

        [DisplayName("Start time")]
        [UIHint("TimePickerm")]
        public DateTime StartTime { get; set; }

        [DisplayName("End time")]
        [UIHint("TimePickerm")]
        public DateTime EndTime { get; set; }

        [UIHint("Lookup")]
        public Guid? Owner { get; set; }

        [UIHint("Lookup")]
        public Guid? Patient { get; set; }

        [UIHint("Lookup")]
        public Guid? Procedure { get; set; }

        [UIHint("ColorDropdown")]
        public string Color { get; set; }

        [UIHint("Textarea")]
        public string Description { get; set; }
    }
}
