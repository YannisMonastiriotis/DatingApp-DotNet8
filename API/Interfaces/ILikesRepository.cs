using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface ILikesRepository
    {
        Task<UserLike?> GetUserLike(int sourceUserId, int targetUserId);

        Task<PagedList<MemberDto>> GetUsersWithLikes(LikesParams likesParams);

        Task<IEnumerable<int>> GetCurrentUserLikeIds(int currentUserId);

        void DeleteLike(UserLike like);

        void AddLike(UserLike like);

        Task<bool> SaveChanges();
    }
}