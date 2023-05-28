using System.ComponentModel.DataAnnotations;

namespace WebApp.Entities
{
    public class Chamber : BaseEntity
    {
        [Display(Name = "Номер")]
        public int Number { get; set; }

        [Display(Name = "Этаж")]
        public int Floor { get; set; }

        [Display(Name = "Ответсвенный")]
        public Contact Owner { get; set; }

        [Display(Name = "Кол-во мест")]
        public int QuantitySeats { get; set; }

        [Display(Name = "Пол")]
        public Gender Gender { get; set; }

        [Display(Name = "Тип палаты")]
        public ChamberType ChamberType { get; set; }
    }
}
