namespace TimeTracker.Api.DTO
{
    public class ActivityDTO_GET
    {
        public Guid ActivityId { get; set; }
        public Guid UserId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public DateTime DateTimeCreated { get; set; }
        public int ActivityEstimatedTime { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }

    public class ActivityDTO_POST
    {
        public Guid UserId { get; set; }
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public int ActivityEstimatedTime { get; set; }

    }

    public class ActivityDTO_PUT
    {
        public string ActivityName { get; set; }
        public string ActivityDescription { get; set; }
        public int ActivityEstimatedTime { get; set; }

    }

}
