using Newtonsoft.Json;

namespace MiniLibraryManagementSystem.Dtos
{
    public class UserDto
    {
        public string Message { get; set; }
        public bool IsSuccess { get; set; }
        public string FullName {  get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string ProfilePhoto { get; set; }
        public bool IsAuthenticated { get; set; }
        public ICollection<string>? Roles {  get; set; }
        public string? Token { get; set; }
        public DateTime ExpiresOn { get; set; }
        [JsonIgnore]
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiresOn { get; set; }

    }
}
