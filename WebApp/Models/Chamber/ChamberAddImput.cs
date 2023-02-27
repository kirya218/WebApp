using Microsoft.AspNetCore.Mvc;
using Omu.AwesomeMvc;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Chamber
{
    public class ChamberAddImput
    {
        public Guid Id { get; set; }

        [Display(Name = "Номер палаты")]
        [Required(ErrorMessage = "Number is required")]
        [Remote("VerifyNumber", "ChamberValidate", "Number is incorrect", AdditionalFields = (nameof(Floor)))]
        public int Number { get; set; }

        [Display(Name = "Этаж")]
        [UIHint("Odropdown")]
        [AweUrl("GetFloorsList", "Chamber")]
        [Required(ErrorMessage = "Floor is required")]
        public int Floor { get; set; }

        [Display(Name = "Количество мест")]
        [UIHint("Odropdown")]
        [AweUrl("GetSeatsList", "Chamber")]
        [Required(ErrorMessage = "Quantity of seats is required")]
        public int QuantitySeats { get; set; }

        [Display(Name = "Постояльцы")]
        [UIHint("MultiLookup")]
        public IEnumerable<Guid>? Patients { get; set; }

        [Display(Name = "Ответственный")]
        [UIHint("Lookup")]
        [Required(ErrorMessage = "Owner is required")]
        public Guid Owner { get; set; }
    }
}
