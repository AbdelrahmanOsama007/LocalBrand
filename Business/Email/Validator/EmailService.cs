using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using MimeKit;
using MailKit.Security;
using MailKit.Net.Smtp;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Email.Dtos;

namespace Business.Email.Validator
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool SendEmail(EmailModel model)
        {
                var smtpSettings = _configuration.GetSection("SmtpSettings");
                string smtpHost = smtpSettings["Host"];
                int smtpPort = int.Parse(smtpSettings["Port"]);
                bool enableSsl = bool.Parse(smtpSettings["EnableSsl"]);
                string username = smtpSettings["Username"];
                string password = smtpSettings["Password"];

                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(model.FromName, username));
                message.To.Add(new MailboxAddress(model.ToName, model.ToEmail));
                message.Subject = model.Subject;
                message.Body = new TextPart("plain")
                {
                    Text = model.Body
                };
                var smtpClient = new SmtpClient();
                smtpClient.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
                smtpClient.Connect(smtpHost, smtpPort, SecureSocketOptions.SslOnConnect);
                smtpClient.Authenticate(username, password);
                try
                {
                    smtpClient.Send(message);
                    smtpClient.Disconnect(true);
                    smtpClient.Dispose();
                    return true;
                }
            catch (Exception ex)
                {
                    return false;
                }
        }
        public bool RecieveEmail(ContactDto model)
        {
            var smtpSettings = _configuration.GetSection("SmtpSettings");
            string smtpHost = smtpSettings["Host"];
            int smtpPort = int.Parse(smtpSettings["Port"]);
            bool enableSsl = bool.Parse(smtpSettings["EnableSsl"]);
            string username = smtpSettings["Username"];
            string password = smtpSettings["Password"];

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(model.Name, model.Email));
            message.To.Add(new MailboxAddress("Eleve Store", username));
            message.Subject = "Client Message";
            message.Body = new TextPart("plain")
            {
                Text = model.Message
            };
            var smtpClient = new SmtpClient();
            smtpClient.ServerCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true;
            smtpClient.Connect(smtpHost, smtpPort, SecureSocketOptions.SslOnConnect);
            smtpClient.Authenticate(username, password);
            try
            {
                smtpClient.Send(message);
                smtpClient.Disconnect(true);
                smtpClient.Dispose();
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
