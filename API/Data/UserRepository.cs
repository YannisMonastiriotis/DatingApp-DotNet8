using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;

namespace API.Data
{
    public class UserRepository(DataContext context, IMapper mapper) : IUserRepository
    {
       public async Task<MemberDto?> GetMemberAsync(string username)
        {
            return await context.Users
            .Where(x=>x.UserName == username)
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            .SingleOrDefaultAsync();
        }
 
        public async Task<IEnumerable<MemberDto?>> GetMembersAsync()
        {
            var users =   await context.Users
            .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
            .ToListAsync();

            return users;
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
          return await context.Users
          .FindAsync(id);
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
          return await context.Users
           .Include(x => x.Photos)
          .SingleOrDefaultAsync(rd => rd.UserName == username);
        }

        public async Task<IEnumerable<AppUser?>> GetUsersAsync()
        {
            return await context.Users
            .Include(x => x.Photos)
            .ToListAsync();
        }

        public async Task<bool> SaveAllAsync()
        {
            return await context.SaveChangesAsync() > 0;
        }

        public async void Update(AppUser appUser)
        {
           context.Entry(appUser).State = EntityState.Modified;
           await context.SaveChangesAsync();
        }

       
    }
}