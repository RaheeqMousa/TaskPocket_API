using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace TaskPocket.PL.Utils
{
    public class EmailSettings: IEmailSender
    {
        private readonly IConfiguration _configuration;
        public EmailSettings(IConfiguration conf) {
            this._configuration = conf;
        }

        public Task SendEmailAsync(string email, string subject, string messageHtml)
        {
            var fromAddress = _configuration["EmailSettings:FromAddress"];
            var pass= _configuration["EmailSettings:Password"];

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential("raheeqmousa00@gmail.com", "najt wosl idfc kvqu")
            };

            return client.SendMailAsync(
                new MailMessage(from: "raheeqmousa00@gmail.com",
                                to: email,
                                subject,
                                messageHtml
                                )
                { IsBodyHtml = true });
        }
    }
}
