﻿using System;
using System.Net;
using System.Net.Mail;
using Manager.Interfaces;
using Models;

namespace Manager
{
    public class EmailManager : IEmailManager
    {
        private readonly SmtpClient _smtp;

        public EmailManager(SmtpClient smtp = null)
        {
            _smtp = smtp ?? new SmtpClient();
            // TODO add smtp config in settings
            _smtp.Host = "smtp.gmail.com";
            _smtp.Credentials = new NetworkCredential("3wbet.manager@gmail.com", "3wbetManager.@wfl-team1");
            _smtp.EnableSsl = true;
            _smtp.Port = 587;
        }


        public void SendResetPasswordEmail(User user)
        {
            var mailMessage = new MailMessage();

            mailMessage.To.Add(user.Email);
            var token = SingletonManager.Instance.TokenManager.GenerateEmailToken(user.Email, user.Role, user.Username);
            // TODO add url in settings
            var url = "https://qsomazzi.gitlab.io/react-master-group-1/#/reset_password/" + token;
            mailMessage.Subject = "Reset Your Password";
            mailMessage.Body =
                "You requested for a password reset, use the link below to reset your password, Warning this link is only available for 1 hour " +
                url;
            mailMessage.From = new MailAddress("3wbet.manager@gmail.com", "3wbetManager");

            _smtp.Send(mailMessage);
        }

        public void SendVerifyAccountEmail(User user)
        {
            var mailMessage = new MailMessage();

            mailMessage.To.Add(user.Email);
            var token = SingletonManager.Instance.TokenManager.GenerateEmailToken(user.Email, user.Role, user.Username);
            // TODO add goof route
            var url = "https://qsomazzi.gitlab.io/react-master-group-1/#/verify_account/" + token;
            mailMessage.Subject = "Confirm your email";
            mailMessage.Body =
                " To get started, click the link below to confirm your account, Warning this link is only available for 1 hour " +
                url;
            mailMessage.From = new MailAddress("3wbet.manager@gmail.com", "3wbetManager");

            _smtp.Send(mailMessage);
        }

        public void SendWebMasterEmail(Exception exception)
        {
            var mailMessage = new MailMessage();
            // TODO add email in database for manage it in admin
            mailMessage.To.Add("florob95@gmail.com");
            var now = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            mailMessage.Subject = "[" + now + "]" + " ERROR API";
            mailMessage.Body = exception.ToString();
            mailMessage.From = new MailAddress("3wbet.manager@gmail.com", "3wbetManager");

            _smtp.Send(mailMessage);
        }
    }
}