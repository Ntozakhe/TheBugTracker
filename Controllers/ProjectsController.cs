using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Models.Enums;
using TheBugTracker.Models.ViewModels;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers
{
    [Authorize]
    public class ProjectsController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTLookUpService _lookUpService;
        private readonly IBTFileService _fileService;
        private readonly IBTProjectService _projectService;
        private readonly UserManager<BTUser> _userManager;
        private readonly IBTCompanyInfoService _companyInfoService;

        public ProjectsController(IBTRolesService rolesService,
                                  IBTLookUpService lookUpService,
                                  IBTFileService fileService,
                                  IBTProjectService projectService,
                                  UserManager<BTUser> userManager,
                                  IBTCompanyInfoService companyInfoService)
        {

            _rolesService = rolesService;
            _lookUpService = lookUpService;
            _fileService = fileService;
            _projectService = projectService;
            _userManager = userManager;
            _companyInfoService = companyInfoService;
        }

        public async Task<IActionResult> MyProjects()
        {
            string userId = _userManager.GetUserId(User)!;
            List<Project> projects = await _projectService.GetUserProjectsAsync(userId);
            return View(projects);
        }
        public async Task<IActionResult> AllProjects()
        {
            //we need to return a set of data for Admis and PM's
            //return a set of data for submitters and developers.          
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> projects = new();
            if (User.IsInRole(nameof(Roles.Administrator)) || User.IsInRole(nameof(Roles.ProjectManager)))
            {
                //return a list of all projects in the company including Archived projects
                projects = await _companyInfoService.GetAllProjectsAsync(companyId);
            }
            else
            {
                //These people wont see the Archived projects.
                projects = await _projectService.GetAllProjectsByCompany(companyId);
            }
            return View(projects);
        }
        public async Task<IActionResult> ArchivedProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> projects = await _projectService.GetArchivedProjectsByCompany(companyId);
            return View(projects);
        }

        [Authorize(Roles = nameof(Roles.Administrator))]
        public async Task<IActionResult> UnassigendProjects()
        {
            int companyId = User.Identity.GetCompanyId().Value;
            List<Project> projects = new();
            projects = await _projectService.GetUnassignedProjectsAsync(companyId);
            return View(projects);
        }
        [Authorize(Roles = nameof(Roles.Administrator))]
        public async Task<IActionResult> AssignPM(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            AssignPMViewModel model = new();
            model.Project = await _projectService.GetProjectByIdAsync(id, companyId);
            model.PMList = new SelectList(await _rolesService.GetUsersInRoleAsync(nameof(Roles.ProjectManager), companyId), "Id", "FullName");

            return View(model);
        }

        [Authorize(Roles = nameof(Roles.Administrator))]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignPM(AssignPMViewModel model)
        {
            if (!string.IsNullOrEmpty(model.PMID))
            {
                await _projectService.AddProjectManagerAsync(model.PMID, model.Project.Id);

                return RedirectToAction(nameof(Details), new { id = model.Project.Id });
            }
            return RedirectToAction(nameof(AssignPM), new { id = model.Project.Id });
        }

        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        [HttpGet]
        public async Task<IActionResult> AssignMembers(int id)
        {
            ProjectMembersViewModel model = new();
            int companyId = User.Identity.GetCompanyId().Value;

            model.Project = await _projectService.GetProjectByIdAsync(id, companyId);
            List<BTUser> developers = await _rolesService.GetUsersInRoleAsync(nameof(Roles.Developer), companyId);
            List<BTUser> submitters = await _rolesService.GetUsersInRoleAsync(nameof(Roles.Submitter), companyId);

            List<BTUser> companyMembers = developers.Concat(submitters).ToList();
            //contains the current list of members
            List<string> projectMembers = model.Project.Members.Select(m => m.Id).ToList();
            model.Users = new MultiSelectList(companyMembers, "Id", "FullName", projectMembers);

            return View(model);
        }

        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AssignMembers(ProjectMembersViewModel model)
        {
            if (model.SelectedUsers != null)
            {
                List<string> memberIds = (await _projectService.GetAllProjectMembersExceptPMAsync(model.Project.Id))
                                                               .Select(m => m.Id).ToList();
                //Remove current members
                foreach (string member in memberIds)
                {
                    await _projectService.RemoveUserFromProjectAsync(member, model.Project.Id);
                }

                //Add the Selected Memebrs

                foreach (string member in model.SelectedUsers)
                {
                    await _projectService.AddUserToProjectAsync(member, model.Project.Id);
                }

                //goto project details
                return RedirectToAction("Details", "Projects", new { id = model.Project.Id });
            }
            return RedirectToAction(nameof(AssignMembers), new { id = model.Project.Id });
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {

                if (id == null)
                {
                    return NotFound();
                }
                int companyId = User.Identity.GetCompanyId().Value;

                Project project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

                if (project == null)
                {
                    return NotFound();
                }

                return View(project);
            }
            catch (Exception)
            {

                throw;
            }
        }

        // GET: Projects/Create
        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        public async Task<IActionResult> Create()
        {
            //ask the currently logged in user what is their companyId
            int companyId = User.Identity.GetCompanyId().Value;

            //add an instance of our VM
            AddProjectWithPMViewModel model = new();

            //Load SelectLists with data.
            //SelectList(source, proprty to be selected/captured of the source, property to be dispalyed)
            model.PMLIst = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(),
                                                                companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookUpService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Projects/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                int companyId = User.Identity.GetCompanyId().Value;
                try
                {
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;
                    }

                    model.Project.CompanyId = companyId;


                    await _projectService.AddNewProjectAsync(model.Project);

                    //Add PM if one is chosen
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }

                }
                catch (Exception)
                {

                    throw;
                }
                //Redirect to All Projects
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
        }

        // GET: Projects/Edit/5
        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        public async Task<IActionResult> Edit(int? id)
        {

            int companyId = User.Identity.GetCompanyId().Value;

            AddProjectWithPMViewModel model = new();

            model.Project = await _projectService.GetProjectByIdAsync(id.Value, companyId);

            model.PMLIst = new SelectList(await _rolesService.GetUsersInRoleAsync(Roles.ProjectManager.ToString(),
                                                                companyId), "Id", "FullName");
            model.PriorityList = new SelectList(await _lookUpService.GetProjectPrioritiesAsync(), "Id", "Name");
            return View(model);
        }

        // POST: Projects/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AddProjectWithPMViewModel model)
        {
            if (model != null)
            {
                try
                {
                    if (model.Project.ImageFormFile != null)
                    {
                        model.Project.ImageFileData = await _fileService.ConvertFileToByteArrayAsync(model.Project.ImageFormFile);
                        model.Project.ImageFileName = model.Project.ImageFormFile.FileName;
                        model.Project.ImageContentType = model.Project.ImageFormFile.ContentType;
                    }

                    await _projectService.UpdateProjectAsync(model.Project);

                    //Add PM if one is chosen
                    if (!string.IsNullOrEmpty(model.PmId))
                    {
                        await _projectService.AddProjectManagerAsync(model.PmId, model.Project.Id);
                    }
                    return RedirectToAction("AllProjects");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await ProjectExists(model.Project.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            return RedirectToAction("Edit");
        }

        // GET: Projects/Delete/5
        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        public async Task<IActionResult> Archive(int? id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            if (id == null)
            {
                return NotFound();
            }

            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        [HttpPost, ActionName("Archive")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ArchiveConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.ArchiveProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        public async Task<IActionResult> Restore(int? id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            if (id == null)
            {
                return NotFound();
            }

            var project = await _projectService.GetProjectByIdAsync(id.Value, companyId);
            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }

        // POST: Projects/Delete/5
        [Authorize(Roles = $"{nameof(Roles.Administrator)},{nameof(Roles.ProjectManager)}")]
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;

            var project = await _projectService.GetProjectByIdAsync(id, companyId);

            await _projectService.RestoreProjectAsync(project);

            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProjectExists(int id)
        {
            int companyId = User.Identity.GetCompanyId().Value;
            return (await _projectService.GetAllProjectsByCompany(companyId)).Any(p => p.Id == id);
        }
    }
}
