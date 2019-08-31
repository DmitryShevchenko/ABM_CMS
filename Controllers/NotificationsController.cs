using System;
using System.Security.Policy;
using System.Text;
using System.Text.Encodings.Web;
using ABM_CMS.Interfaces;
using ABM_CMS.Models.Password;
using ABM_CMS.Pages;

using Microsoft.AspNetCore.Mvc;

namespace ABM_CMS.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult EmailConfirmed(string userId, string emailConfirmationToken) 
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(emailConfirmationToken))
            {
                return Redirect("/login");// 404 not found page
            }

            return View(); //React View.
            
        }

        public IActionResult ResetPasswordView(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                // 404 not found page
            }
            
            return Redirect($"/resetPassword/{UrlEncoder.Default.Encode(token)}");
        }
        
        
    }
}