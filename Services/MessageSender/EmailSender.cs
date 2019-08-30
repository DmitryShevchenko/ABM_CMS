using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ABM_CMS.Helpers;
using ABM_CMS.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ABM_CMS.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly EmailSmtp _emailSmtp;

        public EmailSender(IOptions<EmailSmtp> options)
        {
            _emailSmtp = options.Value;
        }

        // https://www.hangfire.io/
        public async Task Send(string userEmail, string subject, string message)
        {
            using (var email = new MailMessage()
            {
                From = new MailAddress(_emailSmtp.AppEmail),
                To = {new MailAddress(userEmail)},
                Subject = subject,
                Body = message,
            })
            {
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = _emailSmtp.Host;
                    smtp.Port = _emailSmtp.Port;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(_emailSmtp.AppEmail, _emailSmtp.Password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    
                    //smtp.Send(email);
                   await smtp.SendMailAsync(email);
                }
            }
        }
    }
}

