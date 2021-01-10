using System.Threading.Tasks;
using API.DTOs;
using API.Entities;
using API.Helpers;

namespace API.Services
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likesUserId);

        Task<AppUser> GetUserWithLikes(int userId);

        Task<PagedList<LikeDto>> GetUserLikes(LikesParams likesParams);
    }
}
