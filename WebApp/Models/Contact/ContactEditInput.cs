using System.ComponentModel.DataAnnotations;

namespace WebApp.Models.Contact
{
    public class ContactEditInput
    {
        public Guid Id { get; set; }

        [Display(Name = "Short name")]
        [MinLength(6, ErrorMessage = "Short name must be at least {1} characters long")]
        [MaxLength(30, ErrorMessage = "Short name cannot exceed {1} characters")]
        public string? Name { get; set; }

        [Display(Name = "Full name")]
        [Required(ErrorMessage = "Full name is required")]
        [MinLength(6, ErrorMessage = "Full name must be at least {1} characters long")]
        [MaxLength(50, ErrorMessage = "Full name cannot exceed {1} characters")]
        public string FullName { get; set; }

        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Display(Name = "Age")]
        public int Age { get; set; }

        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Display(Name = "Mobile phone")]
        public string? MobilePhone { get; set; }

        [Display(Name = "Work phone")]
        public string? Phone { get; set; }
    }
}
