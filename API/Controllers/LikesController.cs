using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;
using API.Extensions;
using API.Helpers;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace API.Controllers
{
    [Authorize]
    public class LikesController(IUnitOfWork unitOfWork) : BaseApiController
    {
       [HttpPost("{targetUserId:int}")]
        public async Task<ActionResult> ToggleLike(int targetUserId)
        {
            var sourceUserId = User.GetUserId();

            if(sourceUserId == targetUserId) 
                {
                return BadRequest("You cannot like yourself");
            }

            var existingLike = await unitOfWork.LikesRepository.GetUserLike(sourceUserId, targetUserId);

            if(existingLike == null)
            {
                var like = new UserLike
                {
                    SourceUserId = sourceUserId,
                    TargetUserId = targetUserId
                };

                unitOfWork.LikesRepository.AddLike(like);
            }
            else
            {
                unitOfWork.LikesRepository.DeleteLike(existingLike);
            }
            var complete = await unitOfWork.Complete();

            return complete ? Ok() :  BadRequest("Failed to update like");

        }

        [HttpGet("list")]
        public async Task<ActionResult<IEnumerable<int>>> GetCurrentUserLikeIds()
        {
            return Ok
            (
                await unitOfWork.LikesRepository.GetCurrentUserLikeIds(User.GetUserId())
            );
        }

        [HttpGet]

        public async Task<ActionResult<IEnumerable<MemberDto>>> GetUserLikes([FromQuery]LikesParams likesParams)
        {
            likesParams.UserId = User.GetUserId();
            var users = await unitOfWork.LikesRepository.GetUsersWithLikes(likesParams);

            Response.AddPaginationHeader(users);
            return Ok(users);
        }
    }
}