using System.Threading.Tasks;
using ABM_CMS.Models.Identity;
using ABM_CMS.Models.Token;

namespace ABM_CMS.Interfaces
{
    public interface ITokenCreator
    {
        Task<TokenResponseModel> CreateAccessToken(ApplicationUser user, string refreshToken);
        RefreshTokenModel CreateRefreshToken(string clientId, string userId);
    }
}