using Microsoft.AspNetCore.Mvc;

namespace ABM_CMS.Controllers
{
    public class NotificationsController : Controller
    {
        public IActionResult EmailConfirmed(string userId, string emailConfirmationToken) 
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(emailConfirmationToken))
            {
                return Redirect("/login");
            }

            return View();
        }
    }
}