namespace Hospital.Models.settings
{
    public class jwtSettings
    {
        public string key { get; set; }
        public string issuer { get; set; }
        public string audience { get; set; }
        public int durationInMinutes { get; set; }
        public int tokenValidateInMinutes { get; set; } = 10;


    }
}
