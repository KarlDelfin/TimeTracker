using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.DatabaseConnection;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Models;
using TimeTracker.Api.Services;
namespace TimeTracker.Api.Logics

{
    public class UserAssignmentLogic
    {
        private readonly TimeTrackerContext _context;
        private readonly Base64Resizer _base64Resizer;
        private readonly FilePath _filePath;

        public UserAssignmentLogic(TimeTrackerContext context, Base64Resizer base64Resizer, FilePath filePath)
        {
            _context = context;
            _base64Resizer = base64Resizer;
            _filePath = filePath;
        }

        public async Task<List<UserAssignmentDTO_GET>> GetUserAssignments(string? search)
        {
            var data = await (from ua in _context.UserAssignments
                              join u in _context.Users on ua.UserId equals u.UserId
                              join r in _context.Roles on ua.RoleId equals r.RoleId
                              select new UserAssignmentDTO_GET
                              {
                                  UserAssignmentId = ua.UserAssignmentId,
                                  UserId = ua.UserId,
                                  RoleId = ua.RoleId,
                                  RoleName = r.Name,
                                  RoleLevel = r.RoleLevel,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Email = u.Email,
                                  Image = _base64Resizer.ResizeImage(_filePath.UserImagePath(u.Image)),
                                  IsDisabled = ua.IsDisabled
                              }).ToListAsync();
            if (!string.IsNullOrWhiteSpace(search))
            {
                data = await (from ua in _context.UserAssignments
                              join u in _context.Users on ua.UserId equals u.UserId
                              join r in _context.Roles on ua.RoleId equals r.RoleId
                              select new UserAssignmentDTO_GET
                              {
                                  UserAssignmentId = ua.UserAssignmentId,
                                  UserId = ua.UserId,
                                  RoleId = ua.RoleId,
                                  RoleName = r.Name,
                                  RoleLevel = r.RoleLevel,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  Email = u.Email,
                                  Image = _base64Resizer.ResizeImage(_filePath.UserImagePath(u.Image)),
                                  IsDisabled = ua.IsDisabled
                              }).Where(x => x.FirstName.Contains(search) || x.LastName.Contains(search) || x.Email.Contains(search)).ToListAsync();
                return data;
            }
            return data;
        }

        public async Task<bool> AssignUserAssignment(List<UserAssignmentDTO_POST> dto)
        {
            int success = 0;
            var data = dto.Select(x => new UserAssignment
            {
                UserAssignmentId = Guid.NewGuid(),
                UserId = x.UserId,
                RoleId = x.RoleId,
                IsDisabled = false,
                DateTimeCreated = DateTime.Now
                
            }).ToList();

            _context.AddRange(data);
            success = await _context.SaveChangesAsync();
            return success > 0;
        }

        public async Task<bool> UpdateUserAssignment(Guid userAssignmentId, UserAssignmentDTO_PUT dto)
        {
            int success = 0;
            var data = await _context.UserAssignments.FirstOrDefaultAsync(x => x.UserAssignmentId == userAssignmentId);
            data.UserId = dto.UserId;
            data.RoleId = dto.RoleId;
            _context.UserAssignments.Update(data);
            success = await _context.SaveChangesAsync();
            return success > 0;
        }

        public async Task<bool> DisableUserAssignment(Guid userAssignmentId)
        {
            int success = 0;
            var data = await _context.UserAssignments.FirstOrDefaultAsync(x=>x.UserAssignmentId == userAssignmentId);

            data.IsDisabled = !data.IsDisabled;
            _context.UserAssignments.Update(data);
            success = await _context.SaveChangesAsync();
            return success > 0;
        }

        
    }
}
