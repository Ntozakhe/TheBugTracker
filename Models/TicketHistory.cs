using System.ComponentModel;

namespace TheBugTracker.Models
{
    public class TicketHistory
    {
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }
        //This represents the Id of the ticket that exists elsewhere(FK).
        //A primary key of a ticket

        [DisplayName("Updated Item")]
        public string? Property { get; set; }

        [DisplayName("Previouse")]
        public string? OldValue { get; set; }

        [DisplayName("Current")]
        public string? NewValue { get; set; }

        [DisplayName("Date Modified")]
        public DateTimeOffset Created { get; set; }

        [DisplayName("Description of change")]
        public string? Description { get; set; }


        [DisplayName("Team Member")]
        public string? UserId { get; set; }
        //Identity User
        //the person that modified the ticket

        //Navigation properties.
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? User { get; set; }
        //Virtual means, the current fk i possess,
        //their Id's are found on these tables.
    }
}
