namespace EventMaster.Application.DTOs
{
    public class JwtSettings
    {
        public string ValidIssuer { get; set; } = string.Empty;
        public string ValidAudience { get; set; } = string.Empty;
        public string IssuerSigningKey { get; set; } = string.Empty;
    }
}
