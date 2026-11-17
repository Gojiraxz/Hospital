namespace Hospital.Models.settings
{
    public class smtpSettings
    {
        public string host { get; set; }
        public int port { get; set; }
        public bool enableSSL { get; set; }
        public string user { get; set; } = null!;
        public string password { get; set; } = null!;

    }
}
