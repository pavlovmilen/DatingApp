using System.Threading.Tasks;
using API.Entities;

namespace API.Services
{
    public interface ITokenService
    {
        Task<string> CreateToken(AppUser appUser);
    }
}
