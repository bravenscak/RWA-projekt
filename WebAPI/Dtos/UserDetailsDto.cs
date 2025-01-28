using System.ComponentModel.DataAnnotations;

namespace MiniOglasnikZaBesplatneStvari.Dtos
{
    public class UserDetailsDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "User name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(256, MinimumLength = 8, ErrorMessage = "Password should be at least 8 characters long")]
        public string Password { get; set; }

        [EmailAddress(ErrorMessage = "Provide a correct e-mail address")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Provide a correct phone number")]
        public string Phone { get; set; }
    }
}
