namespace TimeTracker.Api.DTO
{
    public class UserDTO_GET
    {
        public Guid UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
    }

    public class UserDTO_LOGIN
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

    public class UserDTO_POST
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile Image { get; set; }
    }

    public class UserDTO_PUT
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public IFormFile Image { get; set; }
    }
}
