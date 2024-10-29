using Business.Email.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Email.Validator
{
    public class SendEmail : ISendEmail
    {
        private readonly string _apiKey = "your_mailgun_api_key";
        private readonly string _domain = "your_mailgun_domain";
        public async Task SendEmailAsync(string toEmail, string subject, string message)
        {
            //var client = new MailgunClient(_apiKey);
            //var messageData = new MessageBuilder()
            //    .SetFrom("you@yourdomain.com")
            //    .SetTo(toEmail)
            //    .SetSubject(subject)
            //    .SetText(message)
            //    .Build();

            //await client.SendMessageAsync(_domain, messageData);
        }
    }
}
