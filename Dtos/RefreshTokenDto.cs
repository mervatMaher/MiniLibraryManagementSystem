namespace MiniLibraryManagementSystem.Dtos
{
    public class RefreshTokenDto
    {
        public int Id { get; set; }
        public string Token { get; set; }
        public DateTime ExpireOn { get; set; }
        public bool IsExpired { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokeOn { get; set; }
        public bool IsActive { get; set; }
        public string UserId { get; set; }
    }
}
