namespace MiniLibraryManagementSystem.HelperServices.IServices
{
    public interface IUploadFilesServices
    {
        public Task<string> UploadPhotoAsync(IFormFile Image);

    }
}
