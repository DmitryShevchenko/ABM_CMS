using System.ComponentModel.DataAnnotations;
using Twilio.TwiML.Messaging;

namespace ABM_CMS.Models.Password
{
    public class ResetPasswordModel
    {
        [DataType(DataType.Password)] [Required] public string Password { get; set; }
        [Required] public string Token { get; set; }
    }
}