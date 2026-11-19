namespace Hospital.Service
{
    public interface IEmailService
    {
        void SendPasswordResetEmail(string ToEmail,string body);

    }
}
