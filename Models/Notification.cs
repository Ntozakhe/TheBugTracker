﻿using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TheBugTracker.Models
{
    public class Notification
    {
        //Will help us notify our users about things via email
        public int Id { get; set; }

        //A notification is for a particular ticket
        [DisplayName("Ticket")]
        public int TicketId { get; set; }

        [Required]
        [DisplayName("Title")]
        public string? Title { get; set; }

        [Required]
        [DisplayName("Message")]
        public string? Message { get; set; }

        [DataType(DataType.Date)]
        [DisplayName("Date")]
        public DateTimeOffset Created { get; set; }

        [Required]
        [DisplayName("Receipeint")]
        public string? RecipientId { get; set; }

        [Required]
        [DisplayName("Sender")]
        public string? SenderId { get; set; }


        [DisplayName("Has been viewed")]
        public bool Viewed { get; set; }

        //---------------------------------------
        public virtual Ticket? Ticket { get; set; }
        public virtual BTUser? Recipient { get; set; }
        public virtual BTUser? Sender { get; set; }

    }
}
