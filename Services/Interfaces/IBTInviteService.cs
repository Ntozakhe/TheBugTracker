using TheBugTracker.Models;

namespace TheBugTracker.Services.Interfaces
{
    public interface IBTInviteService
    {
        //
        public Task<bool> AcceptInviteAsync(Guid? token, string userId, int companyId);
        public Task AddNewInviteAsync(Invite invite);
        //do any of the invite smatch the companyToken and the email?
        public Task<bool> AnyInviteAsync(Guid token, string email, int companyId);
        public Task<Invite> GetInviteAsync(int inviteId, int companyId);
        public Task<Invite> GetInviteAsync(Guid token, string email, int companyId);
        //validate an invite
        public Task<bool> ValidateInviteCodeAsync(Guid? token);
    }
}
