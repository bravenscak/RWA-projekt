using System.ComponentModel.DataAnnotations;

namespace MiniOglasnikZaBesplatneStvari.Dtos
{
    public class UserLoginDto
    {
        [Required(ErrorMessage = "User name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
    }
}
