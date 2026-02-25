using Microsoft.EntityFrameworkCore;
using TimeTracker.Api.DatabaseConnection;
using TimeTracker.Api.DTO;
using TimeTracker.Api.Models;

namespace TimeTracker.Api.Logics
{
    public class StatusLogLogic
    {
        private readonly TimeTrackerContext _context;
        public StatusLogLogic(TimeTrackerContext context)
        {
            _context = context;
        }

        public async Task<List<StatusLogDTO_GET>> GetStatusLogByRecordId(Guid recordId)
        {
            var data = await (from sl in _context.StatusLogs
                              where sl.RecordId == recordId
                              orderby sl.DateTimeCreated descending
                              select new StatusLogDTO_GET
                              {
                                  StatusLogName = sl.Name,
                                  DateTimeCreated = sl.DateTimeCreated,
                              }).ToListAsync();
            return data;
        }

        // START, PAUSE, COMPLETE
        public async Task<bool> UpdateStatus(StatusLogDTO_POST dto)
        {
            var record = await _context.Records.FirstOrDefaultAsync(x => x.RecordId == dto.RecordId);
            var statusLog = await _context.StatusLogs.OrderByDescending(x => x.DateTimeCreated).FirstOrDefaultAsync(x => x.RecordId == dto.RecordId);
            int success = 0;

            if (dto.StatusName == "Pause" || dto.StatusName == "Complete")
            {
                record.CurrentRunningTime = dto.CurrentRunningTime;
                _context.Records.Update(record);
                await _context.SaveChangesAsync();
            }

            var status = new StatusLog();
            status.StatusLogId = Guid.NewGuid();
            status.RecordId = dto.RecordId;
            status.Name = dto.StatusName;
            status.DateTimeCreated = DateTime.Now;

            _context.StatusLogs.Add(status);
            success = await _context.SaveChangesAsync();

            return success > 0;
        }
    }
}
