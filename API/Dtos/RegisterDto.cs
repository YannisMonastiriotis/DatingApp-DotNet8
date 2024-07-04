using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace API.Dtos
{
    public class RegisterDto
    {
        [Required]
        [MaxLength(100)]
        public required string Username { get; set; }
        
        [Required]
        public required string Password { get; set; }    
    }
}