using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace KASHOP.BLL.Common
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_config["EmailSettings:Host"], int.Parse(_config["EmailSettings:Port"]))
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_config["EmailSettings:Email"], _config["EmailSettings:Password"])
            };

            return client.SendMailAsync(
                new MailMessage(from: _config["EmailSettings:Email"],
                                to: email,
                                subject,
                                message
                                )
                { IsBodyHtml=true});
        }       
    }
}
