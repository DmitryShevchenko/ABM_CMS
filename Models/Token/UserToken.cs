using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ABM_CMS.Models.Identity;

namespace ABM_CMS.Models.Token
{
    public class UserToken
    {
        [Key] public string Id { get; set; }

        public string UserId { get; set; }
        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public string TokenProvider { get; set; }

        public string Purpose { get; set; }
        public string Token { get; set; }
    }
}