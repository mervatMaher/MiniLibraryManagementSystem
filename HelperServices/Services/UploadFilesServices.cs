using MiniLibraryManagementSystem.HelperServices.IServices;

namespace MiniLibraryManagementSystem.HelperServices.Services
{
    public class UploadFilesServices : IUploadFilesServices
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        public UploadFilesServices(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public async Task<string> UploadPhotoAsync(IFormFile Image)
        {
            var rootDirectory = Directory.GetCurrentDirectory();
            var UploadsFolderDirectory = Path.Combine(rootDirectory, "wwwroot", "Uploads");

            if (!Directory.Exists(UploadsFolderDirectory))
            {
                Directory.CreateDirectory(UploadsFolderDirectory);
            }
            if (Image != null)
            {
                var ImagefileNameGuid = Guid.NewGuid() + Path.GetExtension(Image.FileName);
                var imagePath = Path.Combine(UploadsFolderDirectory, ImagefileNameGuid);

                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await Image.CopyToAsync(stream);
                }

                var request = _httpContextAccessor.HttpContext.Request;
                var ImageFullPath = $"{request.Scheme}://{request.Host}/Uploads/{ImagefileNameGuid}";
                return ImageFullPath;

            }
            return "there is no Image!";
        }

    }
}
