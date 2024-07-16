using Microsoft.AspNetCore.Identity;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBugTracker.Models
{
    public class BTUser : IdentityUser
    {
        [Required]
        [Display(Name = "First Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a Max of {1} charactors")]
        public string? FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and a Max of {1} charactors")]
        public string? LastName { get; set; }

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName { get { return $"{FirstName} {LastName}"; } }

        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? AvatarFormFile { get; set; }
        [DisplayName("Avatar")]
        public string? AvatarFileName { get; set; }
        public byte[]? AvatarFileData { get; set; }
        [DisplayName("Avatar Extension")]
        public string? AvatarContentType { get; set; }
        //-------------------------------------------

        public int? CompanyId { get; set; }

        public virtual Company? Company { get; set; }
        public virtual ICollection<Project> Projects { get; set; } = new HashSet<Project>();
        //one company can have many projects but its a Many to Many
        //because on the Project table has a collection of Members

    }
}
