using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace ABM_CMS.Models
{
    public class LoginViewModel
    {
        [Required] public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}