using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ABM_CMS.Interfaces
{
    public interface IMessageSender
    {
        Task Send(IdentityUser user, string subject, string message);
    }
}