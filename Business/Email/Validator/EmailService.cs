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
using Infrastructure.IGenericRepository;
using Model.Enums;
using Infrastructure.IRepository;
using Infrastructure.Migrations;

namespace Business.Email.Validator
{
    public class EmailService
    {
        private readonly IConfiguration _configuration;
        private readonly IGenericRepository<ReceivedEmail> _receivedemailRepository;
        private readonly IEmailRepository _emailRepository;
        public EmailService(IConfiguration configuration, IGenericRepository<ReceivedEmail> receivedemailRepository, IEmailRepository emailRepository)
        {
            _configuration = configuration;
            _receivedemailRepository = receivedemailRepository;
            _emailRepository = emailRepository;
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
                message.Body = new TextPart("html")
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
        public async Task<OperationResult> ReceiveEmail(ContactDto model)
        {
            try
            {
                var receivedemail = new ReceivedEmail()
                {
                    Name = model.Name,
                    Email = model.Email,
                    Message = model.Message,
                    EmailStatus = ReceviedEmailEnum.Unread,
                };
                var result = await _receivedemailRepository.AddAsync(receivedemail);
                return result;
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> GetAllReceivedEmails(EmailPagination emailmodel)
        {
            try
            {
                return await _emailRepository.GetAllReceivedEmails(emailmodel);
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something went wrong. Please try again later.", DevelopMessage = ex.Message };
            }
        }
        public async Task<OperationResult> EditEmailStatus(ContactInfo contactobject)
        {
            try
            {
                return await _emailRepository.EditEmailStatus(contactobject);
            }
            catch (Exception ex)
            {
                return new OperationResult() { Success = false, Message = "Something Went Wrong. Please Try Again Later", DevelopMessage = ex.Message };
            }
        }
    }
}
