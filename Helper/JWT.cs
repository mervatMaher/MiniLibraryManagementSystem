namespace MiniLibraryManagementSystem.Helper
{
    public class JWT
    {
        public string JWTSecretKey { get; set; }
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public double ExpirationInMinutes { get; set; }
    }
}
