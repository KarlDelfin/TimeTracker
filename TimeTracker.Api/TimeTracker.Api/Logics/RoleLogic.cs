using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.DatabaseConnection;
using TimeTracker.Api.DTO;

namespace TimeTracker.Api.Logics
{
    public class RoleLogic
    {
        private readonly TimeTrackerContext _context;
        public RoleLogic(TimeTrackerContext context)
        {
            _context = context;
        }
        
        public async Task<List<RoleDTO_GET>> GetRole(string? search = "")
        {
            var data = await (from r in _context.Roles
                              select new RoleDTO_GET
                              {
                                  RoleId = r.RoleId,
                                  RoleName = r.Name,
                                  RoleLevel = r.RoleLevel
                              }).ToListAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                data = data.Where(x => x.RoleName.Contains(search)).ToList();
            }
            return data;
        }
    }
}
