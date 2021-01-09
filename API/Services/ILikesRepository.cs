using System.Collections.Generic;
using System.Threading.Tasks;
using API.DTOs;
using API.Entities;

namespace API.Services
{
    public interface ILikesRepository
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likesUserId);

        Task<AppUser> GetUserWithLikes(int userId);

        Task<IEnumerable<LikeDto>> GetUserLikes(string predicate, int userId);
    }
}
