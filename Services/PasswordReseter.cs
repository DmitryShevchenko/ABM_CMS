using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ABM_CMS.Interfaces;
using ABM_CMS.Models.Identity;
using ABM_CMS.Models.Password;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ABM_CMS.Services
{
    public class PasswordReseter : IPasswordReseter
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public PasswordReseter(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> ResetPassword(string userId, ResetPasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(userId) || model == null) return new BadRequestResult();
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return new BadRequestResult();

            var resetPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, resetPasswordToken, model.ConfirmPassword);

            if (result.Succeeded) return new OkResult();

            return new BadRequestResult();
        }
    }
}