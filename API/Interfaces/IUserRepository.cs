using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;

namespace API.Interfaces
{
    public interface IUserRepository
    {

        Task<AppUser?> GetUserByIdAsync(int id);
        void Update(AppUser appUser);

        Task<bool> SaveAllAsync();

        Task<IEnumerable<AppUser?>> GetUsersAsync();

        Task<AppUser?> GetUserByUsernameAsync(string username);

        Task<IEnumerable<MemberDto?>> GetMembersAsync();

        Task<MemberDto?> GetMemberAsync(string username);
    }
}