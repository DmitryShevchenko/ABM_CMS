using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ABM_CMS.Models
{
    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        /*
        [Required]
        [Display(Name = "User Name")]
        public string UserName { get; set; }
        */

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}