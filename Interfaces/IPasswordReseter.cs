using System.Threading.Tasks;
using ABM_CMS.Models.Password;
using Microsoft.AspNetCore.Mvc;

namespace ABM_CMS.Interfaces
{
    public interface IPasswordReseter
    {
        Task<IActionResult> ResetPassword(string userId, ResetPasswordModel model);
    }
}