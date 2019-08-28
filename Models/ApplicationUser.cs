using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace ABM_CMS.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Notes { get; set; }
        public int Type { get; set; }
        public string DisplayName { get; set; }
        public virtual List<TokenModel> Tokens { get; set; }
    }
}