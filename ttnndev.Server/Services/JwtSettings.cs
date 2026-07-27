namespace ttnndev.Server.Services
{
    public class JwtSettings
    {
        public string Issuer { get; set; } = "ttnndev";
        public string Audience { get; set; } = "ttnndev";
        public string SecretKey { get; set; } = null!;
        public int AccessTokenMinutes { get; set; } = 60;
        public int RefreshTokenMinutes { get; set; } = 60;
    }
}
