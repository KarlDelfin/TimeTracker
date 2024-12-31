namespace TimeTracker.Api.DTO
{
    public class StatusLogDTO_GET
    {
        public string StatusLogName { get; set; }
        public DateTime DateTimeCreated { get; set; }
    }
    public class StatusLogDTO_POST
    {
        public Guid RecordId { get; set; }
        public string StatusName { get; set; }
        public int CurrentRunningTime { get; set; }
    }
}
