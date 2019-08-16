using ABM_CMS.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ABM_CMS.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly IMessageSender _messageSender;
        
        public TestController(IMessageSender messageSender)
        {
            _messageSender = messageSender;
        }
        [HttpGet("[action]")]
        public IActionResult Index()
        {
            BackgroundJob.Enqueue<IMessageSender>(x => x.Send(new IdentityUser(){UserName = "Dima", Email = "dima_che@ukr.net", PhoneNumber = "+380990049919"}, "subject", "message"));
            return Ok();
        }
    }
}