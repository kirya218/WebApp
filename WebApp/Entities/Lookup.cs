using System.ComponentModel.DataAnnotations;

namespace WebApp.Entities
{
    public class Lookup : BaseLookup
    {
        [Display(Name = "Код")]
        public string LookupCode { get; set; }
    }
}
