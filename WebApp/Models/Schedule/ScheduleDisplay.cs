namespace WebApp.Models.Schedule
{
    public class ScheduleDisplay
    {
        //
        // Сводка:
        //     event id
        public Guid Id { get; set; }

        //
        // Сводка:
        //     event title
        public string Title { get; set; }

        //
        // Сводка:
        //     event time
        public string Time { get; set; }

        //
        // Сводка:
        //     event color
        public string Color { get; set; }
    }
}
