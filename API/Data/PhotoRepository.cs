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

namespace API.Data
{
    public class PhotoRepository(DataContext context, IMapper mapper) : IPhotoRepository
    {
        public async Task<Photo?> GetPhotoById(int photoId)
        {
           return await context.Photos
           .IgnoreQueryFilters().SingleAsync(x => x.Id == photoId);
        }

        public async Task<List<PhotoForApprovalDto>> GetUnapprovedPhotos()
        {
             
             return   await context.Photos
             .IgnoreQueryFilters()
            .Where(p => !p.IsAproved) 
            .Select( u => new PhotoForApprovalDto{
                Id = u.Id,
                Username = u.AppUser.UserName,
                Url = u.Url,
                IsAproved = u.IsAproved
            })
            .ToListAsync();
        }

        public  Task RemovePhoto(int id)
        {
            var photo = context.Photos.Find(id);
            if(photo != null)
            {
                context.Photos.Remove(photo);
            }
               return Task.CompletedTask;
        }
    }
}