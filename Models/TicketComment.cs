﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class TicketComment
    {
        public int Id { get; set; }

        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [DisplayName("Team Member")]
        public string? UserId { get; set; }

        [Required]
        [DisplayName("Member Comment")]
        public string? Comment { get; set; }

        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? User { get; set; }
    }
}
