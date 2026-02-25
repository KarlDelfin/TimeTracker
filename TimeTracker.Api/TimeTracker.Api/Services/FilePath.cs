namespace TimeTracker.Api.Services
{
    public class FilePath
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        public FilePath(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public string UserImagePath(string fileName)
        {
            bool isExists = System.IO.Directory.Exists($"{_webHostEnvironment.WebRootPath}/Images/User/");
            if (!isExists) {
                System.IO.Directory.CreateDirectory($"{_webHostEnvironment.WebRootPath}/Images/User/");
            }
            return $"{_webHostEnvironment.WebRootPath}/Images/User/{fileName}";
        }
    }
}
