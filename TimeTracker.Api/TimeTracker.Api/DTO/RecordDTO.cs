namespace TimeTracker.Api.DTO
{
    public class RecordDTO_GET
    {
        public Guid RecordId { get; set; }
        public Guid UserId { get; set; }
        public Guid ActivityId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public int ActivityEstimatedTime { get; set; }
        public float? CurrentRunningTime { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public string CurrentStatus { get; set; }
        public string PreviousStatus { get; set; }
    }

    public class RecordDTO_POST
    {
        public Guid UserId { get; set; }
        public Guid ActivityId { get; set; }
    }

    public class RecordDTO_PUT
    {
        public float? CurrentRunningTime { get; set; }
    }

    public class RecordDTO_REASSIGN
    {
        public Guid UserId { get; set; }
        public Guid ActivityId { get; set; }
    }
}
