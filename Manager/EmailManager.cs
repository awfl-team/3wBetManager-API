using System;
using System.Net;
using System.Net.Mail;
using Models;

namespace Manager
{
    public class EmailManager : IDisposable
    {
        private static SmtpClient _smtp;

        public EmailManager(SmtpClient smtp = null)
        {
            _smtp = smtp ?? new SmtpClient();
            // TODO add smtp config in settings
            _smtp.Host = "smtp.gmail.com";
            _smtp.Credentials = new NetworkCredential("3wbet.manager@gmail.com", "3wbetManager.@wfl-team");
            _smtp.EnableSsl = true;
        }

        public void Dispose()
        {
            _smtp = null;
        }

        public void SendResetPasswordEmail(User user)
        {
            var mailMessage = new MailMessage();

            mailMessage.To.Add(user.Email);
            var token = TokenManager.GenerateEmailToken(user.Email, user.Role, user.Username);
            // TODO add url in settings
            var url = "http://localhost:3000/reset_password/" + token;
            mailMessage.Subject = "Reset Your Password";
            mailMessage.Body = "You requested for a password reset, use the link below to reset your password : " + url;
            mailMessage.From = new MailAddress("3wbet.manager@gmail.com", "3wbetManager");

            _smtp.Send(mailMessage);
        }

        public void SendWebMasterEmail(Exception exception)
        {
            var mailMessage = new MailMessage();
            mailMessage.To.Add("florob95@gmail.com");
            var now = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            mailMessage.Subject = "[" + now + "]" + " ERROR API";
            mailMessage.Body = exception.ToString();
            mailMessage.From = new MailAddress("3wbet.manager@gmail.com", "3wbetManager");

            _smtp.Send(mailMessage);
        }
    }
}