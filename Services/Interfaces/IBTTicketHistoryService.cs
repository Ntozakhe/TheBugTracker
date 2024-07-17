using TheBugTracker.Models;

namespace TheBugTracker.Services.Interfaces
{
    public interface IBTTicketHistoryService
    {
        Task AddHisttoryAsync(Ticket oldTicket, Ticket newTicket, string userId);
        Task<List<TicketHistory>> GetProjectTicketsHistoriesAsync(int projectId, int companyId);
        Task<List<TicketHistory>> GetCompanyTicketsHistoriesAsync(int companyId);
    }
}
