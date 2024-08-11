using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using TheBugTracker.Extensions;
using TheBugTracker.Models;
using TheBugTracker.Models.ViewModels;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Controllers
{
    [Authorize]
    public class UserRolesController : Controller
    {
        private readonly IBTRolesService _rolesService;
        private readonly IBTCompanyInfoService _companyInfoService;

        public UserRolesController(IBTRolesService rolesService,
                                   IBTCompanyInfoService companyInfoService)
        {
            _rolesService = rolesService;
            _companyInfoService = companyInfoService;
        }
        [HttpGet]
        public async Task<IActionResult> ManageUserRoles()
        {
            //Add an instance of the ViewModel as a List(model)
            List<ManageUserRolesViewModel> model = new(); //referes to the vm not the view;

            //Get CompanyId - using extensions
            int companyId = User.Identity.GetCompanyId().Value;

            //Get all company users
            List<BTUser> users = await _companyInfoService.GetAllMembersAsync(companyId);

            //Loop over the users to populate the ViewModel
            //instantiate ViewModel
            //use _roleService
            //create multi-selects

            foreach (BTUser user in users)
            {
                ManageUserRolesViewModel viewModel = new();
                viewModel.BTUser = user;
                //get roles for the current user
                IEnumerable<string> selected = await _rolesService.GetUserRolesAsync(user);
                //create our multi select list and assign it
                viewModel.Roles = new MultiSelectList(await _rolesService.GetRolesAsync(), "Name", "Name", selected);

                //basically we want to know whci members are available to add into our project
                //and whcih ones are currently on the project.

                //let the model(line 25) add each individual viewModel
                model.Add(viewModel);
            }
            //Return the model to the View
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ManageUserRoles(ManageUserRolesViewModel member)
        {
            //Get the company id
            int companyId = User.Identity.GetCompanyId().Value;

            //Instantiate BTUser
            BTUser btUser = (await _companyInfoService.GetAllMembersAsync(companyId))
                                                      .FirstOrDefault(u => u.Id == member.BTUser.Id)!;

            //Get Roles for the User.
            IEnumerable<string> roles = await _rolesService.GetUserRolesAsync(btUser);

            //Grab the selected role.
            string userRole = member.SelectedRoles.FirstOrDefault();


            if (!string.IsNullOrEmpty(userRole))
            {
                //Remove User from their roles.
                if (await _rolesService.RemoveUserFromRolesRoleAsync(btUser, roles))
                {
                    //Add user to the new role.
                    await _rolesService.AddUserToRoleAsync(btUser, userRole);
                }

            }
            //Navigate back to the view
            return RedirectToAction(nameof(ManageUserRoles));
        }
    }
}
