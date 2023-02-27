namespace WebApp.Entities
{
    public class SettingValue : BaseEntity
    {
        public Guid? GuidValue { get; set; }
        public bool? BooleanValue { get; set; }
        public DateTime? DateTimeValue { get; set; }
        public float? FloatValue { get; set; }
        public int? IntegerValue { get; set; }
        public string? TextValue { get; set; }
    }
}
