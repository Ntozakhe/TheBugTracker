using Microsoft.EntityFrameworkCore;
using TheBugTracker.Data;
using TheBugTracker.Models;
using TheBugTracker.Services.Interfaces;

namespace TheBugTracker.Services
{
    public class BTTicketService : IBTTicketService
    {
        private readonly ApplicationDbContext _context;

        public BTTicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddNewTicketAsync(Ticket ticket)
        {
            _context.Add(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task ArchiveTicketAsync(Ticket ticket)
        {
            ticket.Archived = true;
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }

        public async Task AssignTicketAsync(int ticketId, string userId)
        {
            BTUser user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                Ticket ticket = await _context.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
                if (ticket != null)
                {
                    ticket.OwnerUser = user;
                    _context.SaveChangesAsync();
                }
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByCompanyAsync(int companyId)
        {
            try
            {
                List<Ticket> tickets = await _context.Projects
                                                        .Where(p => p.CompanyId == companyId)
                                                        .SelectMany(t => t.Tickets)
                                                            .Include(t => t.TicketAttachments)
                                                            .Include(t => t.Comments)
                                                            .Include(t => t.History)
                                                            .Include(t => t.DeveloperUser)
                                                            .Include(t => t.OwnerUser)
                                                            .Include(t => t.TicketPriority)
                                                            .Include(t => t.TicketStatus)
                                                            .Include(t => t.TicketType)
                                                            .Include(t => t.Project)
                                                        .ToListAsync();
                return tickets;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByPriorityAsync(int companyId, string priorityName)
        {
            try
            {
                int priorityId = (await LookupTicketPriorityIdAsync(priorityName)).Value;
                try
                {
                    List<Ticket> tickets = await _context.Projects
                                                                .Where(p => p.Id == companyId)
                                                                .SelectMany(t => t.Tickets)
                                                                    .Include(t => t.TicketAttachments)
                                                                    .Include(t => t.Comments)
                                                                    .Include(t => t.History)
                                                                    .Include(t => t.DeveloperUser)
                                                                    .Include(t => t.OwnerUser)
                                                                    .Include(t => t.TicketPriority)
                                                                    .Include(t => t.TicketStatus)
                                                                    .Include(t => t.TicketType)
                                                                    .Include(t => t.Project)
                                                                .Where(t => t.TicketPriorityId == priorityId)
                                                                .ToListAsync();
                    return tickets;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByStatusAsync(int companyId, string statusName)
        {
            try
            {
                int statusId = (await LookupTicketStatusIdAsync(statusName)).Value;
                try
                {
                    List<Ticket> tickets = await _context.Projects
                                                                .Where(p => p.Id == companyId)
                                                                .SelectMany(t => t.Tickets)
                                                                    .Include(t => t.TicketAttachments)
                                                                    .Include(t => t.Comments)
                                                                    .Include(t => t.History)
                                                                    .Include(t => t.DeveloperUser)
                                                                    .Include(t => t.OwnerUser)
                                                                    .Include(t => t.TicketPriority)
                                                                    .Include(t => t.TicketStatus)
                                                                    .Include(t => t.TicketType)
                                                                    .Include(t => t.Project)
                                                                .Where(t => t.TicketStatusId == statusId)
                                                                .ToListAsync();
                    return tickets;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetAllTicketsByTypeAsync(int companyId, string typeName)
        {
            try
            {
                int typeId = (await LookupTicketTypeIdAsync(typeName)).Value;
                try
                {
                    List<Ticket> tickets = await _context.Projects
                                                                .Where(p => p.Id == companyId)
                                                                .SelectMany(t => t.Tickets)
                                                                    .Include(t => t.TicketAttachments)
                                                                    .Include(t => t.Comments)
                                                                    .Include(t => t.History)
                                                                    .Include(t => t.DeveloperUser)
                                                                    .Include(t => t.OwnerUser)
                                                                    .Include(t => t.TicketPriority)
                                                                    .Include(t => t.TicketStatus)
                                                                    .Include(t => t.TicketType)
                                                                    .Include(t => t.Project)
                                                                .Where(t => t.TicketTypeId == typeId)
                                                                .ToListAsync();
                    return tickets;
                }
                catch (Exception)
                {

                    throw;
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<List<Ticket>> GetArchivedTicketsAsync(int companyId)
        {
            List<Ticket> tickets = await _context.Projects
                                                        .Where(p => p.CompanyId == companyId)
                                                        .SelectMany(t => t.Tickets)
                                                            .Include(t => t.TicketAttachments)
                                                            .Include(t => t.Comments)
                                                            .Include(t => t.History)
                                                            .Include(t => t.DeveloperUser)
                                                            .Include(t => t.OwnerUser)
                                                            .Include(t => t.TicketPriority)
                                                            .Include(t => t.TicketStatus)
                                                            .Include(t => t.TicketType)
                                                            .Include(t => t.Project)
                                                        .Where(t => t.Archived == true)
                                                        .ToListAsync();
            return tickets;
        }

        public Task<List<Ticket>> GetProjectTicketsByPriorityAsync(string priorityName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByRoleAsync(string role, string userId, int projectId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByStatusAsync(string statusName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetProjectTicketsByTypeAsync(string typeName, int companyId, int projectId)
        {
            throw new NotImplementedException();
        }

        public async Task<Ticket> GetTicketByIdAsync(int ticketId)
        {
            Ticket ticket = await _context.Tickets
                                            //.Include(t => t.TicketType)
                                            //.Include(t => t.TicketPriority)
                                            //.Include(t => t.TicketStatus)
                                            //.Include(t => t.Project)
                                            //.Include(t => t.OwnerUser)
                                            //.Include(t => t.DeveloperUser)
                                            //.Include(t => t.Comments)
                                            //.Include(t => t.TicketAttachments)
                                            //.Include(t => t.History)
                                            //.Include(t => t.Notifications)
                                            .FirstOrDefaultAsync(t => t.Id == ticketId);
            return ticket;
        }

        public Task<BTUser> GetTicketDeveloperAsync(int ticketId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetTicketsByRoleAsync(string role, string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetTicketsByUserIdAsync(string userId, int companyId)
        {
            throw new NotImplementedException();
        }

        public async Task<int?> LookupTicketPriorityIdAsync(string priorityName)
        {
            try
            {
                TicketPriority priority = await _context.TicketPriorities.FirstOrDefaultAsync(t => t.Name == priorityName);
                return priority?.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketStatusIdAsync(string statusName)
        {
            try
            {
                TicketStatus status = await _context.TicketStatuses.FirstOrDefaultAsync(t => t.Name == statusName);
                return status?.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<int?> LookupTicketTypeIdAsync(string typeName)
        {
            try
            {
                TicketType type = await _context.TicketTypes.FirstOrDefaultAsync(t => t.Name == typeName);
                return type?.Id;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task UpdateTicketAsync(Ticket ticket)
        {
            _context.Update(ticket);
            await _context.SaveChangesAsync();
        }
    }
}
