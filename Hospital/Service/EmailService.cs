using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using Hospital.Models.settings;

namespace Hospital.Service
{
    public class EmailService(IOptions<smtpSettings> _SmptSettings) : IEmailService
    {
        private readonly smtpSettings _Smpt = _SmptSettings.Value;
        public void SendPasswordResetEmail(string ToEmail, string body)
        {
            // Sin credenciales SMTP configuradas no se envia correo (evita el error al registrar)
            if (string.IsNullOrWhiteSpace(_Smpt.user) || string.IsNullOrWhiteSpace(_Smpt.password))
                return;
            using var client = new SmtpClient(_Smpt.host, _Smpt.port)
            {
                Credentials = new NetworkCredential(_Smpt.user, _Smpt.password),
                EnableSsl = true
            };
            var mail= new MailMessage
            {
                From= new MailAddress(_Smpt.user,"Soporte De autenticacion"),
                Subject= "token de confirmacion/ recuperacion",
                Body= body,
                IsBodyHtml = true
            };
            mail.To.Add(ToEmail);
            client.Send(mail);
                    
        }

        
    }
}
