using System.Collections.Generic;
using ABM_CMS.Models.Token;
using Microsoft.AspNetCore.Identity;

namespace ABM_CMS.Models
{
    public sealed class ApplicationUser : IdentityUser
    {
        public string Notes { get; set; }
        public int Type { get; set; }
        public string DisplayName { get; set; }
        public List<RefreshTokenModel> Tokens { get; set; }
    }
}