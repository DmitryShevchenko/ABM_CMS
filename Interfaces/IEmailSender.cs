using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ABM_CMS.Interfaces
{
   public interface IEmailSender
    {
        Task Send(string userEmail, string subject, string message);
    }
}