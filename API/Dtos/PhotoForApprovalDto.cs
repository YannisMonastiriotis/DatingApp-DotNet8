using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class PhotoForApprovalDto
    {
        public int Id { get; set; }    
        public  string? Url { get; set; }

        public  string? Username { get; set; }
    
        public bool IsAproved { get; set; }
    }
}