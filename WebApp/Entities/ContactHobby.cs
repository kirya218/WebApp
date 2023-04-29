namespace WebApp.Entities
{
    public class ContactHobby : BaseEntity
    {
        public Hobby? Hobby { get; set; }
        public Contact? Contact { get; set; }
    }
}
