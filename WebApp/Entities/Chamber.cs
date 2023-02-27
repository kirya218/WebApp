namespace WebApp.Entities
{
    public class Chamber : BaseEntity
    {
        public int Number { get; set; }
        public int Floor { get; set; }
        public Contact Owner { get; set; }
        public int QuantitySeats { get; set; }
    }
}
