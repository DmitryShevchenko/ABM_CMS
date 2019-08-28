using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ABM_CMS.Models
{
    public class TokenModel
    {
        [Key] public string Id { get; set; }

        /// <summary>
        /// The clientId where it comes from.
        /// </summary>
        [Required]
        public string ClientId { get; set; }

        /// <summary>
        /// Value of the token.
        /// </summary>
        [Required]
        public string Value { get; set; }

        /// <summary>
        /// Get the token creation date.
        /// </summary>
        [Required]
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// The user it was issued to.
        /// </summary>
        [Required]
        public string UserId { get; set; }
        
        [Required] public DateTime LastModifiedDate { get; set; }
        [Required] public DateTime ExpiryTime { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}