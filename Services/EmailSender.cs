using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using ABM_CMS.Helpers;
using ABM_CMS.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace ABM_CMS.Services
{
    public class EmailSender : IMessageSender
    {
        public readonly EmailSmtp EmailSmtp;

        public EmailSender(IOptions<EmailSmtp> options)
        {
            EmailSmtp = options.Value;
        }

        // https://www.hangfire.io/
        public async Task Send(IdentityUser user, string subject, string message)
        {
            using (var email = new MailMessage()
            {
                From = new MailAddress(EmailSmtp.AppEmail),
                To = {new MailAddress(user.Email)},
                Subject = subject,
                Body = message,
            })
            {
                using (var smtp = new SmtpClient())
                {
                    smtp.Host = EmailSmtp.Host;
                    smtp.Port = EmailSmtp.Port;
                    smtp.EnableSsl = true;
                    smtp.UseDefaultCredentials = false;
                    smtp.Credentials = new NetworkCredential(EmailSmtp.AppEmail, EmailSmtp.Password);
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    
                    //smtp.Send(email);
                   await smtp.SendMailAsync(email);
                }
            }
        }
    }
}

