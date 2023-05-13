using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.LookupEdit
{
    public class LookupEditAddInput
    {
        public Guid Id { get; set; }

        [Display(Name = "Название")]
        [Required(ErrorMessage = "Обязательно")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }
    }
}
