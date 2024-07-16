using System.ComponentModel;

namespace TheBugTracker.Models
{
    public class Invite
    {
        public int Id { get; set; }

        [DisplayName("Date Sent")]
        public DateTimeOffset InviteDate { get; set; }

        [DisplayName("Join Date")]
        public DateTimeOffset JoinDate { get; set; }
        //When the user joins our app

        [DisplayName("Code")]
        public Guid CompanyToken { get; set; }
        //identifying a token for a particular company

        [DisplayName("Company")]
        public int CompanyId { get; set; }

        [DisplayName("Project")]
        public int ProjectId { get; set; }

        [DisplayName("Invitor")]
        public string? InvitorId { get; set; }

        [DisplayName("Invitee")]
        public string? InviteeId { get; set; }

        [DisplayName("Invitee Email")]
        public string? InviteeEmail { get; set; }

        [DisplayName("Invitee First Name")]
        public string? InviteeFirstName { get; set; }

        [DisplayName("Invitee Last Name")]
        public string? InviteeLastName { get; set; }

        public bool IsValid { get; set; }
        //helps us determine if a particular invite is valid or not
        //make an invite expire after a certain number of days.

        public virtual Company? Company { get; set; }
        public virtual Project? Project { get; set; }
        public virtual BTUser? Invitor { get; set; }
        public virtual BTUser? Invitee { get; set; }
    }
}
