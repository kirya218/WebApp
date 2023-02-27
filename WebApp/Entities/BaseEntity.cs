using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApp.Entities
{
    public abstract class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Display(Name = "Id")]
        [Key]
        public Guid Id { get; set; }

        [Display(Name = "Дата изменения")]
        public DateTime ModifiedOn { get; set; }

        [Display(Name = "Дата создания")]
        public DateTime CreatedOn { get; set; }
    }
}