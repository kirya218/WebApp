namespace WebApp.Entities
{
    public class Setting : BaseLookup
    {
        public string Code { get; set; }
        public string Type { get; set; }
        public SettingValue SettingValue { get; set; }
    }
}
