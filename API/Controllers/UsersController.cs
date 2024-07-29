using System.Collections;
using System.Security.Claims;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
namespace API.Controllers;

[Authorize]
public class UsersController(IUserRepository userRepository, IMapper mapper) : BaseApiController
{
       

 [HttpGet]
  public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers()
  {
     var users = await userRepository.GetMembersAsync();

    return Ok(users);
  }
  [HttpGet("{Id:int}")]
  public async Task<ActionResult<AppUser>> GetUserById(int id)
  {

    var user = await userRepository.GetUserByIdAsync(id);

    if(user == null)
    {
        return NotFound();
    }

    return Ok(user);
  }

  [HttpGet("{username}")]
  public async Task<ActionResult<MemberDto>> GetUser(string username)
  {

    var user = await userRepository.GetMemberAsync(username);

    if(user == null)
    {
        return NotFound();
    }

    return user;
  }

  [HttpPut]
  public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
  {
    var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
    
    if(username == null)
    {
      return BadRequest("Username not found.");
    }

    var user = await userRepository.GetUserByUsernameAsync(username);

    if(user == null)
    {
      return BadRequest("User not found.");
    }

    mapper.Map(memberUpdateDto, user);

    if(await userRepository.SaveAllAsync()) 
    {
      return NoContent();
    }

     return BadRequest("Ops, something went wrong. Please try again later");
  }
}
