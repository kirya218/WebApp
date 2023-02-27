namespace WebApp.Models.Schedule
{
    public class ScheduleCell
    {
        //
        // Сводка:
        //     cell time representation start in ticks
        public long Ticks { get; set; }

        //
        // Сводка:
        //     cell events
        public ScheduleDisplay[] Events { get; set; }

        //
        // Сводка:
        //     cell day ( for month view )
        public string Day { get; set; }

        //
        // Сводка:
        //     cell date ( for month view )
        public string Date { get; set; }
    }
}
