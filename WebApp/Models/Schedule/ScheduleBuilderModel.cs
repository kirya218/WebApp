using Omu.AwesomeMvc;
using WebApp.Enums.Schedule;

namespace WebApp.Models.Schedule
{
    public class ScheduleBuilderModel
    {
        public IQueryable<Entities.Schedule> Schedules { get; set; }
        public DateTime? DateSelected { get; set; }
        public GridParams GridParams { get; set; }
        public int HourStep { get; set; }
        public ScheduleViewType? ViewType { get; set; }
    }
}
