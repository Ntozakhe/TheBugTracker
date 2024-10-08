﻿using Microsoft.AspNetCore.Mvc.Rendering;

namespace TheBugTracker.Models.ViewModels
{
    public class AddProjectWithPMViewModel
    {
        public Project Project { get; set; }
        public SelectList PMLIst { get; set; }
        public string? PmId { get; set; }
        public SelectList PriorityList { get; set; }
        // public int ProjectPriority { get; set; }
    }
}
