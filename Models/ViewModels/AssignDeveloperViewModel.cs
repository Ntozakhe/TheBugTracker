using Microsoft.AspNetCore.Mvc.Rendering;

namespace TheBugTracker.Models.ViewModels
{
    public class AssignDeveloperViewModel
    {
        public Ticket Ticket { get; set; }
        //list of available developers.
        public SelectList Developers { get; set; }
        //to capture the user ID of the selected developer
        public string DeveloperId { get; set; }
    }
}
