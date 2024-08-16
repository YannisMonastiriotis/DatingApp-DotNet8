using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using API.Dtos;
using API.Entities;
using API.Helpers;
using API.Interfaces;
using API.Services;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using SQLitePCL;

namespace API.Data
{
    public class UserRepository(DataContext context, IMapper mapper, IConfiguration configuration) : IUserRepository
    {
        public  Task ApprovePhoto(Photo photo, AppUser user)
        {
           
            if(!user.Photos.Any(x => x.IsMain)){
                photo.IsMain = true;
            }else{
                photo.IsMain = false;
            }

            photo.IsAproved = true;
           

            context.SaveChanges();
            return Task.CompletedTask;
        }

        public async Task<bool> DeleteBasePhoto(int id)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            var isDeleted = false;

            try
            {
                using (var connection = new SqliteConnection(connectionString))
                {
                    await connection.OpenAsync();

                    var deleteQuery = "DELETE FROM Photos WHERE Id = @id";

                    using (var command = new SqliteCommand(deleteQuery, connection))
                    {
                        // Use correct parameter name
                        command.Parameters.AddWithValue("@id", id);

                        // Use ExecuteNonQueryAsync for async operations
                        var rowsAffected = await command.ExecuteNonQueryAsync();
                        isDeleted = rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle exceptions as appropriate
                Console.WriteLine($"An error occurred: {ex.Message}");
                // Optionally, log the exception or handle it as needed
            }

            return isDeleted;
        }

        public async Task<MemberDto?> GetMemberAsync(string username, bool isCurrentUser = false)
        {
          var query = context.Users
          .Where(x => x.UserName == username)
          .ProjectTo<MemberDto>(mapper.ConfigurationProvider)
          .AsQueryable();

          if(isCurrentUser)
          {
            query = query.IgnoreQueryFilters();
          }

          return await query.FirstOrDefaultAsync();
        }
 
        public async Task<PagedList<MemberDto?>> GetMembersAsync(UserParams userParams)
        {
            var query =  context.Users 
            .AsQueryable();

            query = query.Where(x => x.UserName != userParams.CurrentUsername);

            if(userParams.Gender !=null)
            {
                query = query.Where(x => x.Gender== userParams.Gender);
            }
            
            var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1));
            var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

            query = query.Where(x => x.DateOfBirth >= minDob && x.DateOfBirth <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                 _=> query.OrderByDescending(x => x.LastActive)
            };
            
          
            return await PagedList<MemberDto?>.CreateAsync(query.ProjectTo<MemberDto>(mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
          return await context.Users
          .FindAsync(id);
        }
        public async Task<AppUser?> GetUserByPhotoIdAsync(int photoAppUserId)
        {
            var photo = await context.Photos.FindAsync(photoAppUserId);
          return  photo != null ?
            await context.Users
            .Include(p => p.Photos)
            .IgnoreQueryFilters()
            .Where(p =>  p.Photos.Any(p=>p.Id == photoAppUserId))
            .FirstOrDefaultAsync() : null;
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

        public async void Update(AppUser appUser)
        {
           context.Entry(appUser).State = EntityState.Modified;
           await context.SaveChangesAsync();
        }

       
    }
}