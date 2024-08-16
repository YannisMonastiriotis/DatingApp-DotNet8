using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using API.Entities;

namespace API.Interfaces
{
    public interface IPhotoRepository
    {
        Task<List<PhotoForApprovalDto>> GetUnapprovedPhotos();

        Task<Photo?> GetPhotoById(int photoId);

        Task RemovePhoto(int id);


    }
}