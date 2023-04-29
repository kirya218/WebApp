using System.ComponentModel.DataAnnotations;

namespace WebApp.Entities
{
    public class ContactInChamber : BaseEntity
    {
        [Display(Name = "Контакт")]
        public Contact? Contact { get; set; }

        [Display(Name = "Палата")]
        public Chamber? Chamber { get; set; }
    }
}
