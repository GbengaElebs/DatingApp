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
    }
}