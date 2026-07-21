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
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("amany.khdair0@gmail.com", "dzuo gfqh nabu nxos")
            };

            return client.SendMailAsync(
                new MailMessage(from: "amany.khdair0@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                { IsBodyHtml=true});
        }
    }
}
