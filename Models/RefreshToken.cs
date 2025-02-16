namespace MiniLibraryManagementSystem.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpireOn;
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokeOn { get; set; }
        public bool IsActive => RevokeOn == null && !IsExpired;
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
