namespace WebApp.Entities
{
    public class Contact : BaseEntity
    {
        public string Name { get; set; }
        public string FullName { get; set; }
        public string? Email { get; set; }
        public int Age { get; set; }
        public DateTime BirthDate { get; set; }
        public string? MobilePhone { get; set; }
        public string? Phone { get; set; }
        public byte[]? Image { get; set; }
        public ContactType ContactType { get; set; }
        //TO DO: Должность, Гендер
    }
}
