using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.DatabaseConnection;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace TimeTracker.Api.Logics
{
    public class RecordLogic
    {
        private readonly TimeTrackerContext _context;
        public RecordLogic(TimeTrackerContext context)
        {
            _context = context;
        }

        public async Task<List<RecordDTO_GET>> GetRecord(string? search = "")
        {
            var data = await (from r in _context.Records
                              join u in _context.Users on r.UserId equals u.UserId
                              join a in _context.Activities on r.ActivityId equals a.ActivityId

                              let currentStatus = (from sl in _context.StatusLogs
                                                   where r.RecordId == sl.RecordId
                                                   orderby sl.DateTimeCreated descending
                                                   select new { sl }).FirstOrDefault()
                              let previousStatus = (from sl in _context.StatusLogs
                                                    where r.RecordId == sl.RecordId && sl.DateTimeCreated < currentStatus.sl.DateTimeCreated
                                                    orderby sl.DateTimeCreated descending
                                                    select new { sl }).FirstOrDefault()
                              orderby r.DateTimeCreated descending
                              select new RecordDTO_GET
                              {
                                  RecordId = r.RecordId,
                                  UserId = r.UserId,
                                  ActivityId = r.ActivityId,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  ActivityName = a.Name,
                                  ActivityDescription = a.Description,
                                  ActivityEstimatedTime = a.EstimatedTime,
                                  CurrentRunningTime = r.CurrentRunningTime,
                                  DateTimeCreated = r.DateTimeCreated,
                                  CurrentStatus = currentStatus.sl.Name,
                                  PreviousStatus = previousStatus.sl.Name,
                              }).ToListAsync();
            if (!string.IsNullOrEmpty(search))
            {
                data = data.Where(x => x.ActivityName.Contains(search)).ToList();
                return data;
            }
            return data;
        }

        public async Task<RecordDTO_GET> GetRecordByRecordId(Guid recordId)
        {
            var data = await (from r in _context.Records
                          join u in _context.Users on r.UserId equals u.UserId
                          join a in _context.Activities on r.ActivityId equals a.ActivityId
                          where r.RecordId == recordId

                          let currentStatus = (from sl in _context.StatusLogs
                                               where r.RecordId == sl.RecordId
                                               orderby sl.DateTimeCreated descending
                                               select new { sl }).FirstOrDefault()
                          let previousStatus = (from sl in _context.StatusLogs
                                                where r.RecordId == sl.RecordId && sl.DateTimeCreated < currentStatus.sl.DateTimeCreated
                                                orderby sl.DateTimeCreated descending
                                                select new { sl }).FirstOrDefault()
                          orderby r.DateTimeCreated descending
                          select new RecordDTO_GET
                          {
                              RecordId = r.RecordId,
                              UserId = r.UserId,
                              ActivityId = r.ActivityId,
                              FirstName = u.FirstName,
                              LastName = u.LastName,
                              ActivityName = a.Name,
                              ActivityDescription = a.Description,
                              ActivityEstimatedTime = a.EstimatedTime,
                              CurrentRunningTime = r.CurrentRunningTime,
                              DateTimeCreated = r.DateTimeCreated,
                              CurrentStatus = currentStatus.sl.Name,
                              PreviousStatus = previousStatus.sl.Name,
                          }).FirstOrDefaultAsync();
            return data;
        }

        public async Task<List<RecordDTO_GET>> GetRecordByUserId(Guid userId, string? search = "")
        {
            var data = await (from r in _context.Records
                              join u in _context.Users on r.UserId equals u.UserId
                              join a in _context.Activities on r.ActivityId equals a.ActivityId
                              where r.UserId == userId
                              
                              let currentStatus = (from sl in _context.StatusLogs
                                                   where r.RecordId == sl.RecordId
                                                   orderby sl.DateTimeCreated descending
                                                   select new { sl }).FirstOrDefault()
                              let previousStatus = (from sl in _context.StatusLogs
                                                    where r.RecordId == sl.RecordId && sl.DateTimeCreated < currentStatus.sl.DateTimeCreated
                                                    orderby sl.DateTimeCreated descending
                                                    select new { sl }).FirstOrDefault()
                              orderby r.DateTimeCreated descending
                              select new RecordDTO_GET
                              {
                                  RecordId = r.RecordId,
                                  UserId = r.UserId,
                                  ActivityId = r.ActivityId,
                                  FirstName = u.FirstName,
                                  LastName = u.LastName,
                                  ActivityName = a.Name,
                                  ActivityDescription = a.Description,
                                  ActivityEstimatedTime = a.EstimatedTime,
                                  CurrentRunningTime = r.CurrentRunningTime,
                                  DateTimeCreated = r.DateTimeCreated,
                                  CurrentStatus = currentStatus.sl.Name,
                                  PreviousStatus = previousStatus.sl.Name,
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
                data.AssignedUserId = item.UserId;

                _context.Activities.Update(data);
                records.Add(record);
            }

            _context.Records.AddRange(records);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }

        public async Task<bool> ReassignRecord(Guid recordId, RecordDTO_REASSIGN dto)
        {
            int success = 0;
            var record = await _context.Records.FirstOrDefaultAsync(x => x.RecordId == recordId);
            record.UserId = dto.UserId;
            record.CurrentRunningTime = 0;
            _context.Records.Update(record);
            success = await _context.SaveChangesAsync();
            if(success > 0)
            {
                var statusLogs = await _context.StatusLogs.Where(x=>x.RecordId == recordId).ToListAsync();
                _context.StatusLogs.RemoveRange(statusLogs);
                _context.SaveChanges();

                var activity = await _context.Activities.FirstOrDefaultAsync(x => x.ActivityId == dto.ActivityId);
                activity.AssignedUserId = dto.UserId;
                _context.Activities.Update(activity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
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

                activity.AssignedUserId = null;

                _context.Activities.Update(activity);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }
    }
}
