namespace WebApp.Entities
{
    public class User : BaseEntity
    {
        public string Login { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool IsBlocked { get; set; }
        public Role Role { get; set; }
        public Contact Contact { get; set; }
    }
}
