using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TheBugTracker.Models
{
    public class Project
    {
        public int Id { get; set; }
        [DisplayName("Company")]
        public int? CompanyId { get; set; }
        public int? ProjectPriorityId { get; set; }

        //--------------------------------------------
        [Required]
        [StringLength(50)]
        [DisplayName("Project Name")]
        public string? Name { get; set; }

        [DisplayName("Project Description")]
        public string? Description { get; set; }

        [DisplayName("Project Start Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset StartDate { get; set; }

        [DisplayName("Project End Date")]
        [DataType(DataType.Date)]
        public DateTimeOffset EndDate { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        //-------------------------------------------
        [NotMapped]
        [DataType(DataType.Upload)]
        public IFormFile? ImageFormFile { get; set; }
        [DisplayName("File Name")]
        public string? ImageFileName { get; set; }
        public byte[]? ImageFileData { get; set; }
        [DisplayName("File Extension")]
        public string? ImageContentType { get; set; }
        //-------------------------------------------

        public virtual Company? Company { get; set; }
        public virtual ProjectPriority? ProjectPriority { get; set; }


        public virtual ICollection<Ticket> Tickets { get; set; } = new HashSet<Ticket>();
        public virtual ICollection<BTUser> Members { get; set; } = new HashSet<BTUser>();
        //Many-To-Many relationship.
    }
}
