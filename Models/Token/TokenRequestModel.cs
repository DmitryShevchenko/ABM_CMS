using System.ComponentModel.DataAnnotations;

namespace ABM_CMS.Models
{
    public enum GrantType
    {
        Login = 717590000,
        RefreshToken = 717590001,
    }
    
    public class TokenRequestModel
    {
        /// <summary>
        /// Password or RefreshToken according to user (LoginRequest or RefreshTokenRequest).
        /// Login || RefreshToken
        /// </summary>
        public GrantType GrantType { get; set; }

        /// <summary>
        /// ClientID of token (our Database or App) to sure that our token wan`t stolen.
        /// Must be specified in token settings.
        /// </summary>
        public string ClientId { get; set; }

        /// <summary>
        /// Required value for user who requesting the token. 
        /// </summary>
        public string UserName { get; set; }
        
        /// <summary>
        /// User Email.
        /// </summary>
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        /// <summary>
        /// RefreshToken value.
        /// </summary>
        public string RefreshToken { get; set; }

        /// <summary>
        /// User Password.
        /// </summary>
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}