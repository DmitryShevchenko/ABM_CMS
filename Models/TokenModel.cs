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
        public DataType CreatedDate { get; set; }

        /// <summary>
        /// The user it was issued to.
        /// </summary>
        [Required]
        public string UserId { get; set; }
        
        [Required] public DataType LastModifiedDate { get; set; }
        [Required] public DataType ExpiryTime { get; set; }
        
        [ForeignKey("UserId")]
        public virtual ApplicationUser User { get; set; }
    }
}