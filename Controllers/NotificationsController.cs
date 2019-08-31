using ABM_CMS.Interfaces;
using ABM_CMS.Models.Password;
using ABM_CMS.Pages;

using Microsoft.AspNetCore.Mvc;

namespace ABM_CMS.Controllers
{
    public class NotificationsController : Controller
    {
        public NotificationsController(IPasswordReseter passwordReseter)
        {
            _passwordReseter = passwordReseter;
        }

        private readonly IPasswordReseter _passwordReseter;
        public IActionResult EmailConfirmed(string userId, string emailConfirmationToken) 
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(emailConfirmationToken))
            {
                return Redirect("/login");// 404 not found page
            }

            return View(); //React View.
            
        }

        public IActionResult ResetPasswordView(string userId, string token)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                // 404 not found page
            }


            return Redirect("/resetPassword");

        }
        
        
    }
}