using System.Collections.Generic;
using System.IO;
using ABM_CMS.Interfaces;
using Hangfire;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace ABM_CMS.Controllers
{
    [Route("api/[controller]")]
    public class TestController : Controller
    {
        private readonly IEmailSender _emailSender;
        
        public TestController(IEmailSender emailSender)
        {
            _emailSender = _emailSender;
        }
        
        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("[action]")]
        public IActionResult Index()
        {
            return Content("Hello word");
        }
    }
}