

using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class Ticket
    {
        public int Id { get; set; }

        //----------------- F.K's ---------------------------
        [DisplayName("Ticket Type")]
        public int TicketTypeId { get; set; }

        [DisplayName("Ticket Priority")]
        public int TicketPriorityId { get; set; }

        [DisplayName("Ticket Status")]
        public int TicketStatusId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Ticket Owner")]
        public string? OwnerUserId { get; set; }

        [DisplayName("Ticket Developer")]
        public string? DeveloperUserId { get; set; }
        //---------------------------------------------

        [Required]
        [StringLength(50)]
        [DisplayName("Title")]
        public string? Title { get; set; }

        [Required]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Created")]
        public DateTimeOffset Created { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Updated")]
        public DateTimeOffset? Updated { get; set; }

        [DisplayName("Archived")]
        public bool Archived { get; set; }

        [DisplayName("Archived by Project")]
        public bool ArchivedByProject { get; set; }

        //----Navigational Properties-------------------------------
        public virtual Project? Project { get; set; }
        public virtual TicketPriority? TicketPriority { get; set; }
        public virtual TicketType? TicketType { get; set; }
        public virtual TicketStatus? TicketStatus { get; set; }
        public virtual BTUser? OwnerUser { get; set; }
        public virtual BTUser? DeveloperUser { get; set; }
        //----------------------------------------------------------

        //--Me as the Ticket table have Id's in the following tables:-------------
        public virtual ICollection<TicketComment> Comments { get; set; } = new HashSet<TicketComment>();
        public virtual ICollection<TicketAttachment> TicketAttachments { get; set; } = new HashSet<TicketAttachment>();
        public virtual ICollection<TicketHistory> History { get; set; } = new HashSet<TicketHistory>();
        public virtual ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();
        //These guys have Nvigational properties that point to me (Ticket)---------------
        //One Ticket has MANY Comments, TicketAttachments, Notifications---------------------
    }
}
