using System.Collections;
using System.Security.Claims;
using API.Data;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
namespace API.Controllers;

[Authorize]
public class UsersController(IUnitOfWork unitOfWork,
 IMapper mapper, 
 IPhotoService photoService) : BaseApiController
{
       
//[Authorize(Roles = "Admin")]
 [HttpGet]
  public async Task<ActionResult<IEnumerable<MemberDto>>> GetUsers([FromQuery] UserParams userParams)
  {
    var currentUsername =User.GetUsername();

    userParams.CurrentUsername = currentUsername;
     var users = await unitOfWork.UserRepository.GetMembersAsync(userParams);

    Response.AddPaginationHeader(users);

    return Ok(users);
  }
  [HttpGet("{Id:int}")]
  public async Task<ActionResult<AppUser>> GetUserById(int id)
  {

    var user = await unitOfWork.UserRepository.GetUserByIdAsync(id);

    if(user == null)
    {
        return NotFound();
    }

    return Ok(user);
  }

  [Authorize(Roles = "Member")]
  [HttpGet("{username}")]
  public async Task<ActionResult<MemberDto>> GetUser(string username)
  {
     var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;

    bool isCurrentUser = currentUsername == username;

    var user = await unitOfWork.UserRepository.GetMemberAsync(username, isCurrentUser);

    if(user == null)
    {
        return NotFound();
    }

    return user;
  }

  [HttpPut]
  public async Task<ActionResult> UpdateUser(MemberUpdateDto memberUpdateDto)
  {
    var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

    if(user == null)
    {
      return BadRequest("User not found.");
    }

    mapper.Map(memberUpdateDto, user);

    if(await unitOfWork.Complete()) 
    {
      return NoContent();
    }

     return BadRequest("Ops, something went wrong. Please try again later");
  }

  [HttpPost("add-photo")]
  public async Task<ActionResult<PhotoDto>> AddPhoto(IFormFile file)
  {
    var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
    
    if(user == null)
    {
        return BadRequest("Cannot update user");
    }

    var result = await photoService.AddPhotoAsync(file);

    if(result.Error != null)
    {
      return BadRequest(result.Error.Message);
    }

    var photo = new Photo
    {
        Url = result.SecureUrl.AbsoluteUri,
        PublicId = result.PublicId
    };

   
    user.Photos.Add(photo);
    
    if(await unitOfWork.Complete())
    {
      return CreatedAtAction(nameof(GetUser),
       new {username= user.UserName},
       mapper.Map<PhotoDto>(photo));
    }

    return BadRequest("Problem adding photo");
  }

  [HttpPut("set-main-photo/{photoId:int}")]
  public async Task<ActionResult> SetMainPhoto(int photoId)
  {
    var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

    if(user == null)
    {
     return BadRequest("Could not find user");
     }

     var photo = user.Photos.FirstOrDefault(x => x.Id == photoId);

     if(photo == null || photo.IsMain)
     {
      return BadRequest("Cannot use this as main photo");
     }

     var currentMain = user.Photos.FirstOrDefault(x => x.IsMain);

     if(currentMain != null)
     {
      currentMain.IsMain = false;
     }

     photo.IsMain = true;

     if( await unitOfWork.Complete())
     {
      return NoContent();
     }

      return BadRequest("Problem setting main photo");
  }

  [HttpDelete("delete-photo/{photoid:int}")]
  public async Task<ActionResult> DeletePhoto(int photoid)
  {
    var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());

    if(user == null) return BadRequest("User not found");

    var photo = await unitOfWork.PhotoRepository.GetPhotoById(photoid);

    if(photo == null || photo.IsMain)
    {
      return BadRequest("This photo cannot be deleted");
    }

    if(photo.PublicId != null)
    {
      var result = await photoService.DeletePhotoAsync(photo.PublicId);
      if(result.Error != null)
      {
        return BadRequest(result.Error.Message);
      }

      user.Photos.Remove(photo);

      if(await unitOfWork.Complete())
      {
        return Ok();
      }
    }
    else{
      var isComplete = await unitOfWork.UserRepository.DeleteBasePhoto(photo.Id);
      if(isComplete)
      {
        return Ok();
      }
    }
      return BadRequest("Problem deleting photo");
  }
}
