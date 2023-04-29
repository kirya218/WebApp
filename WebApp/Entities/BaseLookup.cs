using System.ComponentModel.DataAnnotations;

namespace WebApp.Entities
{
    public abstract class BaseLookup : BaseEntity
    {
        [Display(Name = "Название")]
        public string Name { get; set; }

        [Display(Name = "Описание")]
        public string? Description { get; set; }
    }
}
