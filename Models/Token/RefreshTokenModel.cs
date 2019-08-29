using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABM_CMS.Models
{
    public sealed class RefreshTokenModel
    {
        [Key] public string Id { get; set; }

        /// <summary>
        /// The clientId where it comes from.
        /// ClientID of token (our Database or App) to sure that our token wan`t stolen.
        /// Must be specified in token settings.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Value of the Refresh_Token.
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Get the token Creation Date.
        /// </summary>
        [Required]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// The user it was Issued To.
        /// </summary>
        [Required]
        public string UserId { get; set; }
        
        [Required] public DateTime LastModifiedDate { get; set; }
        [Required] public DateTime ExpiryTime { get; set; }
        
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }
    }
}