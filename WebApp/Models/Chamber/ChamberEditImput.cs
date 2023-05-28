using Microsoft.AspNetCore.Mvc;
using GridLibrary;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Chamber
{
    public class ChamberEditImput
    {
        public Guid Id { get; set; }

        [Display(Name = "Номер палаты")]
        [Remote("VerifyNumberEdit", "ChamberValidate", "Number is incorrect", AdditionalFields = (nameof(Floor)))]
        public int Number { get; set; }

        [Display(Name = "Этаж")]
        [UIHint("Combobox")]
        [AweUrl(Action = "GetFloorsList", Controller = "Chamber")]
        [Required(ErrorMessage = "Обязательно")]
        public int Floor { get; set; }

        [Display(Name = "Количество мест")]
        [UIHint("Combobox")]
        [AweUrl(Action = "GetSeatsList", Controller = "Chamber")]
        [Required(ErrorMessage = "Обязательно")]
        public int QuantitySeats { get; set; }

        [Display(Name = "Постояльцы")]
        [UIHint("MultiLookup")]
        public IEnumerable<Guid>? Patients { get; set; }

        [Display(Name = "Ответственный")]
        [UIHint("Lookup")]
        [Required(ErrorMessage = "Обязательно")]
        public Guid Owner { get; set; }

        [Display(Name = "Тип палаты")]
        [UIHint("Lookup")]
        [Required(ErrorMessage = "Обязательно")]
        public Guid ChamberType { get; set; }

        [Display(Name = "Пол")]
        [UIHint("Lookup")]
        [Required(ErrorMessage = "Обязательно")]
        public Guid Gender { get; set; }
    }
}
