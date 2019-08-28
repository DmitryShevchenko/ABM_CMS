namespace ABM_CMS.Models
{
    public class TokenRequestModel
    {
        /// <summary>
        /// Password or RefreshToken according to user (LoginRequest or RefreshTokenRequest).
        /// </summary>
        public string GrandType { get; set; }
        /// <summary>
        /// ClientID of token (Our Database or App) to sure that our token wan`t stolen
        /// </summary>
        public string ClientId { get; set; }
        /// <summary>
        /// Required value for user who requesting the token. 
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// RefreshToken value.
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Password { get; set; }
    }
}