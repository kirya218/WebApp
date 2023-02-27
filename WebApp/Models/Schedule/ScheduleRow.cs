namespace WebApp.Models.Schedule
{
    public class ScheduleRow
    {
        //
        // Сводка:
        //     row time
        public string Time { get; set; }

        //
        // Сводка:
        //     css class for the row
        public string RowClass { get; set; }

        //
        // Сводка:
        //     row date
        public string Date { get; set; }

        //
        // Сводка:
        //     row day cells
        public IList<ScheduleCell> Cells { get; set; }

        //
        // Сводка:
        //     is an all day row
        public bool IsAllDay { get; set; }

        //
        // Сводка:
        //     event title
        public string Title { get; set; }

        //
        // Сводка:
        //     event notes
        public string Notes { get; set; }
    }
}
