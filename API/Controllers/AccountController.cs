using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    public class AccountController(
        DataContext context,
    ITokenService tokenService,
     IMapper mapper
     ) : BaseApiController
    {
        [HttpPost("register")]
        public async Task<ActionResult<UserDto>> Register(RegisterDto registerDto)
        {
            var isExistingUser= await UserExists(registerDto.Username);

                if(isExistingUser)
                {
                    return BadRequest("Username is taken");
                }

          using( var hmac = new HMACSHA512()) 
           {  
                try
                {

                var user = mapper.Map<AppUser>(registerDto);
                
                user.UserName =registerDto.Username.ToLower();
                user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDto.Password));
                user.PasswordSalt = hmac.Key;
                context.Users.Add(user);

                await context.SaveChangesAsync();


                return new UserDto
                {
                    Username = user.UserName,
                    Token = tokenService.CreateToken(user),
                    KnownAs = user.KnownAs
                };

                }
                catch(Exception ex)
                {
                    //Log exception here
                    return BadRequest(ex.Message);
                }

              
            }; 
          
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoggingDto loggingDto)
        {
            var user = await context.Users
            .Include(p =>p.Photos)
            .FirstOrDefaultAsync(usr => 
            usr.UserName == loggingDto.Username.ToLower());

            if(user == null) return Unauthorized("Invalid username");

            using(var hmac = new HMACSHA512(user.PasswordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loggingDto.Password));

                for (int i = 0; i < computedHash.Length; i++)
                {
                    if(computedHash[i] != user.PasswordHash[i])
                    {
                        return Unauthorized("Invalid password");
                    }
                }
            }

             return new UserDto
                {
                    Username = user.UserName,
                    Token = tokenService.CreateToken(user),
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                    KnownAs = user.KnownAs
                };
        }
        private async Task<bool> UserExists(string username)
        {
            return await context.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }
    }
}