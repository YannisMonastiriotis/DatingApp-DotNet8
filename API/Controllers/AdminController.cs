using API.Data;
using API.Entities;
using API.Extensions;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    public class AdminController(UserManager<AppUser> userManager, IUnitOfWork unitOfWork,DataContext context ,
     IPhotoService photoService) : BaseApiController
    {
      [HttpGet("users-with-roles")]
      [Authorize(Policy ="RequireAdminRole")]
      public async Task<ActionResult> GetUsersWithRoles()
      {
        var users = await userManager.Users
        .OrderBy(x => x.UserName)
        .Select(x => new{
            x.Id,
            Username = x.UserName,
            Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
        }).ToListAsync();

        return Ok(users);
      }

      [HttpPost("edit-roles/{username}")]
      [Authorize(Policy ="RequireAdminRole")]
      public async Task<ActionResult> EditRoles(string username, string roles)
      {
            if(string.IsNullOrEmpty(roles))
            {
                return BadRequest("you must select at least ne role");
            }

            var selectedRoles = roles.Split(",").ToArray();

            var user = await userManager.FindByNameAsync(username);

            if(user == null) { return BadRequest("User not found"); }

            var userRoles = await userManager.GetRolesAsync(user);

            var result = await userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));

            if(!result.Succeeded)
            {
                return BadRequest("Failed to add to roles");
            }

            result = await userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));


            if(!result.Succeeded) 
            {
                return BadRequest("Failed t remove from roles");
            }

            return Ok(await userManager.GetRolesAsync(user));
      }
   
      [HttpGet("photos-to-moderate")]
      [Authorize(Policy ="RequireAdminRole")]
      public async Task<ActionResult> GetPhotosForAproval()
      {
        var photos = await unitOfWork.PhotoRepository.GetUnapprovedPhotos();

        return Ok(photos);
      }

      [HttpPost("approve-photo/{photoid:int}")]
      [Authorize(Policy ="RequireAdminRole")]
      public async Task<ActionResult> ApprovePhoto(int photoid)
      {
        var user = await unitOfWork.UserRepository.GetUserByPhotoIdAsync(photoid);
        var photo = await context.Photos.FindAsync(photoid);

        if(user == null)
        {
          return BadRequest("Could not find current user");
        }

        if(photo == null)
        {
          return BadRequest("Could not find photo");
        }

        
        var task = unitOfWork.UserRepository.ApprovePhoto(photo,user);
        
       
        return Ok();
      }

      [HttpPost("reject-photo/{photoId:int}")]
      [Authorize(Policy ="RequireAdminRole")]
      public async Task<ActionResult> RejectPhoto(int photoId)
      { 
        var user = await unitOfWork.UserRepository.GetUserByUsernameAsync(User.GetUsername());
        var photo = await context.Photos.FindAsync(photoId);

        if(photo == null)
        {
          return BadRequest("Could not find photo");
        }

       
    if(photo == null)
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

      if(user == null)
      {
        return BadRequest();
      }
      else{

      user.Photos.Remove(photo);
      }


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
}