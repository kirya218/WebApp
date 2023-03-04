using GridLibrary;

namespace WebApp.Models.Lookup
{
    public class LookupMainEditInput
    {
        public string GridName { get; set; }
        public string ControllerName { get; set; }
        public string LookupName { get; set; }
        public Column[] Columns { get; set; }
    }
}
