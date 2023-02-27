namespace WebApp.Entities
{
    public class Schedule : BaseLookup
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public Contact? Owner { get; set; }
        public Contact? Patient { get; set; }
        public Procedure? Procedure { get; set; }
        public bool IsAllDay { get; set; }
        public string? Color { get; set; }
    }
}
