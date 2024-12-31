using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.DatabaseConnection;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Logics
{
    public class RecordLogic
    {
        private readonly TimeTrackerContext _context;
        public RecordLogic(TimeTrackerContext context)
        {
            _context = context;
        }

        public async Task<List<RecordDTO_GET>> GetRecordByUserId(Guid userId, string? search = "")
        {
            var data = await (from r in _context.Records
                              join a in _context.Activities on r.ActivityId equals a.ActivityId
                              where r.UserId == userId
                              let currentStatus = (from sl in _context.StatusLogs
                                                   where r.RecordId == sl.RecordId
                                                   orderby sl.DateTimeCreated descending
                                                   select new { sl }).FirstOrDefault()
                              orderby r.DateTimeCreated descending
                              select new RecordDTO_GET
                              {
                                  RecordId = r.RecordId,
                                  ActivityId = r.ActivityId,
                                  ActivityName = a.Name,
                                  ActivityDescription = a.Description,
                                  ActivityEstimatedTime = a.EstimatedTime,
                                  CurrentRunningTime = r.CurrentRunningTime,
                                  DateTimeCreated = r.DateTimeCreated,
                                  RecordStatusName = currentStatus.sl.Name
                              }).ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x => x.ActivityName.Contains(search)).ToList();
                return data;
            }
            return data;
        }

        public async Task<bool> AddRecord(List<RecordDTO_POST> dto)
        {
            int success = 0;

            var records = new List<Record>();
            foreach (var item in dto)
            {
                var record = new Record
                {
                    RecordId = Guid.NewGuid(),
                    UserId = item.UserId,
                    ActivityId = item.ActivityId,
                    CurrentRunningTime = 0,
                    DateTimeCreated = DateTime.Now
                };

                var data = _context.Activities.FirstOrDefault(x=>x.ActivityId == item.ActivityId);
                data.IsAssigned = true;

                _context.Activities.Update(data);
                records.Add(record);
            }

            _context.Records.AddRange(records);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }

        public async Task<bool> DeleteRecord(Guid recordId)
        {
            int success = 0;
            var record = await _context.Records.FirstOrDefaultAsync(x => x.RecordId == recordId);
            
            _context.Records.Remove(record);
            success = await _context.SaveChangesAsync();
            
            if(success > 0)
            {
                var activity = await _context.Activities.FirstOrDefaultAsync(x => x.ActivityId == record.ActivityId);

                activity.IsAssigned = false;

                _context.Activities.Update(activity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
