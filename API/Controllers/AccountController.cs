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
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{

    public class AccountController(
        UserManager<AppUser> userManager,
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

      
                try
                {

                var user = mapper.Map<AppUser>(registerDto);
                
                user.UserName =registerDto.Username.ToLower();

               

                var result = await userManager.CreateAsync(user, registerDto.Password);

                if(!result.Succeeded)
                {
                    return BadRequest(result.Errors);
                }

                await userManager.AddToRoleAsync(user, "Member");
                
                return new UserDto
                {
                    Username = user.UserName,
                    Token = await tokenService.CreateToken(user),
                    KnownAs = user.KnownAs,
                    Gender = user.Gender
                    
                };

                }
                catch(Exception ex)
                {
                    //Log exception here
                    return BadRequest(ex.Message);
                }

               
           
          
        }
        
        [HttpPost("login")]
        public async Task<ActionResult<UserDto>> Login(LoggingDto loggingDto)
        {
            var user = await userManager.Users
            .Include(p =>p.Photos)
            .FirstOrDefaultAsync(usr => 
            usr.NormalizedUserName == loggingDto.Username.ToUpper());

            if(user == null) return Unauthorized("Invalid username");


             return new UserDto
                {
                    Username = user.UserName!,
                    Token = await tokenService.CreateToken(user),
                    PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain)?.Url,
                    KnownAs = user.KnownAs,
                    Gender = user.Gender
                };
        }
        private async Task<bool> UserExists(string username)
        {
            return await userManager.Users.AnyAsync(x => x.NormalizedUserName == username.ToUpper());
        }
    }
}