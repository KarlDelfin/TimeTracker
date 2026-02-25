using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.DatabaseConnection;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Models;
using TimeTracker.Api.Services;

namespace TimeTracker.Api.Logics
{
    public class ActivityLogic
    {
        private readonly TimeTrackerContext _context;
        public ActivityLogic(TimeTrackerContext context)
        {
            _context = context;
        }

        public async Task<List<ActivityDTO_GET>> GetActivityByUserId(Guid userId, string? search = "", bool isFiltered = false)
        {
            if (isFiltered)
            {
                var data = await (from a in _context.Activities
                                  from u in _context.Users.Where(x => x.UserId == a.AssignedUserId).DefaultIfEmpty()
                                  where a.UserId == userId && a.AssignedUserId == null
                                  orderby a.DateTimeCreated descending
                                  select new ActivityDTO_GET
                                  {
                                      ActivityId = a.ActivityId,
                                      UserId = userId,
                                      ActivityName = a.Name,
                                      ActivityDescription = a.Description,
                                      ActivityEstimatedTime = a.EstimatedTime,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName

                                  }).ToListAsync();
                if (!string.IsNullOrEmpty(search))
                {
                    data = data.Where(x => x.ActivityName.Contains(search)).ToList();
                    return data;
                }
                return data;
            }
            else
            {
                var data = await (from a in _context.Activities
                                  from u in _context.Users.Where(x=>x.UserId == a.AssignedUserId).DefaultIfEmpty()
                                  where a.UserId == userId
                                  orderby a.DateTimeCreated descending
                                  select new ActivityDTO_GET
                                  {
                                      ActivityId = a.ActivityId,
                                      UserId = userId,
                                      ActivityName = a.Name,
                                      ActivityDescription = a.Description,
                                      ActivityEstimatedTime = a.EstimatedTime,
                                      FirstName = u.FirstName,
                                      LastName = u.LastName

                                  }).ToListAsync();
                if (!string.IsNullOrEmpty(search))
                {
                    data = data.Where(x => x.ActivityName.Contains(search)).ToList();
                    return data;
                }
                return data;
            }
        }

        public async Task<bool> AddActivity(ActivityDTO_POST dto)
        {
            int success = 0;

            var data = new Activity();

            data.ActivityId = Guid.NewGuid();
            data.UserId = dto.UserId;
            data.Name = dto.ActivityName;
            data.Description = dto.ActivityDescription;
            data.EstimatedTime = dto.ActivityEstimatedTime;
            //data.IsAssigned = false;
            data.DateTimeCreated = DateTime.Now;

            _context.Activities.Add(data);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }

        public async Task<bool> UpdateActivity(Guid activityId, ActivityDTO_PUT dto)
        {
            int success = 0;

            var data = await _context.Activities.FirstOrDefaultAsync(x=>x.ActivityId == activityId);

            data.Name = dto.ActivityName;
            data.Description = dto.ActivityDescription;
            data.EstimatedTime = dto.ActivityEstimatedTime;

            _context.Activities.Update(data);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }

        public async Task<bool> DeleteActivity(Guid activityId)
        {
            int success = 0;

            var data = await _context.Activities.FirstOrDefaultAsync(x => x.ActivityId == activityId);

            _context.Activities.Remove(data);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }
    }
}
