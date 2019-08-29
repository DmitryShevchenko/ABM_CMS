using System;

namespace ABM_CMS.Models
{
    public class TokenResponseModel
    {
        /// <summary>
        /// JWT Token value.
        /// </summary>
        public string Token { get; set; }
        /// <summary>
        /// Expiration time for the token.
        /// </summary>
        public DateTime Expiration { get; set; }
        /// <summary>
        /// RefreshToken value.
        /// </summary>
        public string RefreshToken { get; set; }
        /// <summary>
        /// User role.
        /// </summary>
        public string Roles { get; set; }
        /// <summary>
        /// User Name.
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// User Email.
        /// </summary>
        public string Email { get; set; }
    }
}