using System.ComponentModel.DataAnnotations;

namespace ABM_CMS.Models.Password
{
    public enum PasswordActionType
    {
        ChangePassword = 717590000,
        ForgotPassword = 717590001,
    }
    public class PasswordChangeModel
    {
        /// <summary>
        /// Password or RefreshToken according to user (LoginRequest or RefreshTokenRequest).
        /// Login || RefreshToken
        /// </summary>
        /*[Required]
        public PasswordActionType PasswordActionType { get; set; }*/
        [DataType(DataType.EmailAddress)] public string Email { get; set; }
        [DataType(DataType.Password)] public string CurrentPassword { get; set; }
        [DataType(DataType.Password)] public string NewPassword { get; set; }
    }
}