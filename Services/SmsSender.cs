using System;
using System.Threading.Tasks;
using ABM_CMS.Interfaces;
using Microsoft.AspNetCore.Identity;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace ABM_CMS.Services
{
    public class SmsSender : IMessageSender
    {
        //Twilio
        //https://www.twilio.com/docs/sms/quickstart/csharp-dotnet-framework
        public async Task Send(IdentityUser user, string subject, string message)
        {
            throw new NotImplementedException("Register on twilio.com and get [AccountSid and AuthToken]");
            // Find your Account Sid and Token at twilio.com/console
            // DANGER! This is insecure. See http://twil.io/secure
            const string accountSid = "AXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";
            const string authToken = "6XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX";

            TwilioClient.Init(accountSid, authToken);
            
            await MessageResource.CreateAsync(
               
                from: new Twilio.Types.PhoneNumber("+18XXXXXXXXXX"), //  From number, must be an SMS-enabled Twilio number ( This will send sms from ur "To" numbers ).
                body:  $"Hello {user.UserName} !! Welcome to Asp.Net Core With Twilio SMS API !!",
                to: new Twilio.Types.PhoneNumber(user.PhoneNumber));
        }
    }
}