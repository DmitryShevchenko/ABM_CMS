using System.ComponentModel.DataAnnotations;
using Twilio.TwiML.Messaging;

namespace ABM_CMS.Models.Password
{
    public class ResetPasswordModel
    {
        [DataType(DataType.Password)] [Required(ErrorMessage = "Password value is Required")] public string Password { get; set; }
        [DataType(DataType.Password)] [Required(ErrorMessage = "Confirm Password value is Required")] public string ConfirmPassword { get; set; }
        
        public bool IsPasswordAreSame => Password == ConfirmPassword;
    }
}