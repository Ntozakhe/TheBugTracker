using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TheBugTracker.Data;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Models.ChartModels;
using TheBugTracker.Models.Enums;
using TheBugTracker.Models.ViewModels;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IBTCompanyInfoService _companyInfoService;

        private readonly IBTProjectService _projectService;
        private readonly SignInManager<BTUser> signInManager;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IBTTicketService _ticketService;

        public HomeController(ILogger<HomeController> logger, IBTCompanyInfoService companyInfoService, IBTProjectService projectService, SignInManager<BTUser> signInManager, ApplicationDbContext applicationDbContext, IBTTicketService ticketService)
        {
            _logger = logger;
            _companyInfoService = companyInfoService;
            _projectService = projectService;
            this.signInManager = signInManager;
            _applicationDbContext = applicationDbContext;
            _ticketService = ticketService;
        }

        public IActionResult Index()
        {

            return View();
        }

        public async Task<IActionResult> Dashboard()
        {
            DashboardViewModel model = new();
            int companyId = User.Identity.GetCompanyId().Value;
            model.Company = await _companyInfoService.GetCompanyInfoByIdAsync(companyId);
            model.Projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();
            model.Tickets = model.Projects.SelectMany(p => p.Tickets).Where(t => t.Archived == false).ToList();
            model.Members = model.Company.Members.ToList();
            return View(model);
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }





        //*******************************************************
        //     CHARTS                                   CHARTS
        //******************************************************
        //*************************************************************************************
        //
        // PROJECT PRIORITY                                             PROJECT PRIORITY
        //
        //******************************************************************************************

        // Projects by Priority CanvasJs 
        [HttpPost]
        public async Task<JsonResult> ProjectPriority()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);


            List<ProjectChartViewModel> chartData = new List<ProjectChartViewModel>();

            foreach (string priority in Enum.GetNames(typeof(BTProjectPriority)))
            {
                int priorityCount = (await _projectService.GetAllProjectsByPriority(companyId, priority)).Count();

                chartData.Add(new ProjectChartViewModel
                {
                    y = priorityCount,
                    legendText = priority.ToString(),
                    label = priority.ToString(),
                });
            }
            return Json(chartData);
        }

        //*************************************************************************************
        //
        // PROJECT TICKET COUNTS                                                PROJECT TICKET COUNTS
        //
        //******************************************************************************************
        [HttpPost]
        public async Task<JsonResult> ProjectTicketCount()
        {
            // Project Ticket Count
            // same as ProjectPriority
            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);

            List<ProjectChartViewModel> chartData = new List<ProjectChartViewModel>();

            // labels will be project names
            //List<string> labels = new List<string>();

            //foreach (var name in projects)
            //{
            //    labels.Add(name.Name);
            //}

            //List<object> chartData = new();
            //chartData.Add(labels);

            //chartData.Add(new object[] { "ProjectName", "TicketCount" });

            //List<int> data = new List<int>();


            //foreach (Project prj in projects)
            //{
            //    chartData.Add(new object[] { prj.Name, prj.Tickets.Count() });
            //}

            // for each project name, count tickets

            foreach (var project in projects)
            {
                //int ticketCount = (project.Tickets.Count());
                //data.Add(ticketCount);
                chartData.Add(new ProjectChartViewModel
                {
                    label = project.Name,
                    y = project.Tickets.Count(),
                });
            }
            return Json(chartData);
        }

        //*************************************************************************************
        //
        // TICKET PRIORITY                                                          TICKET PRIORITY
        //
        //******************************************************************************************
        [HttpPost]
        public async Task<JsonResult> TicketPriority()
        {
            int companyId = User.Identity.GetCompanyId().Value;

            // DON'T NEED THIS, data is taken directly from the model in the view

            List<string> priorities = new List<string>();
            var low = _applicationDbContext.Tickets.Where(t => t.TicketPriority.Name == nameof(BTTicketPriority.Low)).ToString();
            var medium = _applicationDbContext.Tickets.Where(t => t.TicketPriority.Name == nameof(BTTicketPriority.Medium)).ToString();
            var high = _applicationDbContext.Tickets.Where(t => t.TicketPriority.Name == nameof(BTTicketPriority.High)).ToString();
            var urgent = _applicationDbContext.Tickets.Where(t => t.TicketPriority.Name == nameof(BTTicketPriority.Urgent)).ToString();

            priorities.Add(low);
            priorities.Add(medium);
            priorities.Add(high);
            priorities.Add(urgent);

            List<TicketChartViewModel> chartData = new List<TicketChartViewModel>();

            foreach (string priority in priorities)
            {
                int priorityCount = (await _ticketService.GetAllTicketsByPriorityAsync(companyId, priority)).Count();

                chartData.Add(new TicketChartViewModel
                {
                    y = priorityCount,
                    label = priority.ToString(),
                });
            }
            return Json(chartData);
        }

        //*************************************************************************************
        //
        // TICKET V DEVELOPERS                                                 TICKET  V DEVELOPERS
        //
        //******************************************************************************************
        [HttpPost]
        public async Task<JsonResult> TicketVDeveloper()
        {

            AmChartData amChartData = new();
            List<AmItem> amItems = new();

            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = (await _companyInfoService.GetAllProjectsAsync(companyId)).Where(p => p.Archived == false).ToList();

            foreach (Project project in projects)
            {
                AmItem item = new();

                item.Project = project.Name;
                item.Tickets = project.Tickets.Count;
                item.Developers = (await _projectService.GetProjectMembersByRoleAsync(project.Id, nameof(Roles.Developer))).Count();

                amItems.Add(item);
            }

            amChartData.Data = amItems.ToArray();


            return Json(amChartData.Data);
        }

        [HttpPost]
        public async Task<JsonResult> PlotlyBarChart()
        {
            PlotlyBarData plotlyData = new();
            List<PlotlyBar> barData = new();

            int companyId = User.Identity.GetCompanyId().Value;

            List<Project> projects = await _projectService.GetAllProjectsByCompany(companyId);

            //Bar One
            PlotlyBar barOne = new()
            {
                X = projects.Select(p => p.Name).ToArray(),
                Y = projects.SelectMany(p => p.Tickets).GroupBy(t => t.ProjectId).Select(g => g.Count()).ToArray(),
                Name = "Tickets",
                Type = "bar"
            };

            //Bar Two
            PlotlyBar barTwo = new()
            {
                X = projects.Select(p => p.Name).ToArray(),
                Y = projects.Select(async p => (await _projectService.GetProjectMembersByRoleAsync(p.Id, nameof(Roles.Developer))).Count).Select(c => c.Result).ToArray(),
                Name = "Developers",
                Type = "bar"
            };

            barData.Add(barOne);
            barData.Add(barTwo);

            plotlyData.Data = barData;

            return Json(plotlyData);
        }



    }
}
