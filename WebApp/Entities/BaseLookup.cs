namespace WebApp.Entities
{
    public abstract class BaseLookup : BaseEntity
    {
        public string Name { get; set; }
        public string? Description { get; set; }
    }
}
