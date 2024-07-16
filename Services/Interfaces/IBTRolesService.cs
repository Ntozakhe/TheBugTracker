using TheBugTracker.Models;

namespace TheBugTracker.Services.Interfaces
{
    public interface IBTRolesService
    {
        public Task<bool> IsUserInRoleAsync(BTUser user, string roleName);
        //we simply want to test if the user is in that role.
        public Task<IEnumerable<string>> GetUserRolesAsync(BTUser user);
        //Returns a collection of string roles that belong to the user.
        public Task<bool> AddUserToRoleAsync(BTUser user, string roleName);
        //allows us to make a user belong to a role
        public Task<bool> RemoveUserFromRoleAsync(BTUser user, string roleName);
        //remove a user from a single role
        public Task<bool> RemoveUserFromRolesRoleAsync(BTUser user, IEnumerable<string> roles);
        //removes a user from a list of roles.
        public Task<List<BTUser>> GetUsersInRoleAsync(string roleName, int companyId);
        //give me a list of users that belong to this role for this company.
        public Task<List<BTUser>> GetUsersNotInRoleAsync(string roleName, int companyId);
        public Task<string> GetRoleNameByIdAsync(string roleId);
    }
}
