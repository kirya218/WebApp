using GridLibrary;
using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Chamber
{
    public class ChamberChoiceImput
    {
        [Display(Name = "Этаж")]
        [UIHint("Odropdown")]
        [AweUrl(Action = "GetFloorsList", Controller = "Chamber")]
        public int? Floor { get; set; }

        [Display(Name = "Количество мест")]
        [UIHint("Odropdown")]
        [AweUrl(Action = "GetSeatsList", Controller = "Chamber")]
        public int? QuantitySeats { get; set; }

        [Display(Name = "Увлечения")]
        [UIHint("MultiLookup")]
        public IEnumerable<Guid>? Hobbies { get; set; }

        [Display(Name = "Тип палаты")]
        [UIHint("Lookup")]
        public Guid? ChamberType { get; set; }

        [Display(Name = "Пол")]
        [UIHint("Lookup")]
        public Guid? Gender { get; set; }

        [Display(Name = "Есть ли братья/сестеры?")]
        public bool HasBrather { get; set; }

        [Display(Name = "Количество братьев/сестер")]
        public int? QuantityBrather { get; set; }
        public Guid SystemType { get; set; }
    }
}
