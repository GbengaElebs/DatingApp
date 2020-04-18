using System;
using System.ComponentModel.DataAnnotations;

namespace datingapp.api.DTO
{
    public class UserForRegister
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [StringLength(8, MinimumLength = 4, ErrorMessage = "You must enter a password from 4 ")]
        public string Password { get; set; }
        [Required]
        public string Gender { get; set; }
        [Required]
        public string KnownAs { get; set; }
        [Required]
        public DateTime DateOfBirth { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }

        public DateTime Created { get; set; }

        public DateTime LastActive { get; set; }
        public UserForRegister()
        {
            Created = DateTime.Now;
            LastActive = DateTime.Now;
        }
    }
}