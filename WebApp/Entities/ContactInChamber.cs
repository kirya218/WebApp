namespace WebApp.Entities
{
    public class ContactInChamber : BaseEntity
    {
        public Contact? Contact { get; set; }
        public Chamber? Chamber { get; set; }
    }
}
