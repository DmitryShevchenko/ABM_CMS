using System.Threading.Tasks;
using ABM_CMS.Interfaces;
using MailKit.Net.Smtp;
using Microsoft.AspNetCore.Identity;
using MimeKit;

namespace ABM_CMS.Services
{
    public class EmailSender : IMessageSender
    {
        //MailKit
        public async Task Send(IdentityUser user, string subject, string message)
        {
            var emailMessage = new MimeMessage();
            
            emailMessage.From.Add(new MailboxAddress("Администрация", "login@gmail.com"));
            emailMessage.To.Add(new MailboxAddress("", user.Email));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = message,
            };
            
            using (var client = new SmtpClient())
            {
                await client.ConnectAsync("smtp.gmail.com", 25, false);
                await client.AuthenticateAsync("login@gmail.com", "password");
                await client.SendAsync(emailMessage);
                
                await client.DisconnectAsync(true);
            }

        }
    }
}