using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Helpers;

namespace API.Interfaces
{
    public interface IUserRepository
    {

        Task<AppUser?> GetUserByIdAsync(int id);
        void Update(AppUser appUser);


        Task<IEnumerable<AppUser?>> GetUsersAsync();

        Task<AppUser?> GetUserByUsernameAsync(string username);

        Task<PagedList<MemberDto?>> GetMembersAsync(UserParams userParams);

        Task<MemberDto?> GetMemberAsync(string username);

        Task<bool> DeleteBasePhoto(int id);
    }
}