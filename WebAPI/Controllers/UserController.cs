using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniOglasnikZaBesplatneStvari.Dtos;
using MiniOglasnikZaBesplatneStvariLibrary.Models;
using MiniOglasnikZaBesplatneStvari.Security;

namespace MiniOglasnikZaBesplatneStvari.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AdvertisementRwaContext _context;

        public UserController(IConfiguration configuration, AdvertisementRwaContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("[action]")]
        public ActionResult<UserDetailsDto> Register(UserDetailsDto UserDetailsDto)
        {
            try
            {
                var trimmedUsername = UserDetailsDto.Username.Trim();
                if (_context.UserDetails.Any(x => x.Username.Equals(trimmedUsername)))
                    return BadRequest($"Username {trimmedUsername} already exists");

                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(UserDetailsDto.Password, b64salt);

                var user = new UserDetail
                {
                    IdUserDetails = UserDetailsDto.Id,
                    Username = UserDetailsDto.Username,
                    PasswordHash = b64hash,
                    PasswordSalt = b64salt,
                    Email = UserDetailsDto.Email,
                    Phone = UserDetailsDto.Phone,
                };

                _context.Add(user);
                _context.SaveChanges();

                UserDetailsDto.Id = user.IdUserDetails;

                return Ok(UserDetailsDto);

            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult Login(UserLoginDto userDto)
        {
            try
            {
                var genericLoginFail = "Incorrect username or password";

                var existingUser = _context.UserDetails.FirstOrDefault(x => x.Username == userDto.Username);
                if (existingUser == null)
                    return BadRequest(genericLoginFail);

                var b64hash = PasswordHashProvider.GetHash(userDto.Password, existingUser.PasswordSalt);
                if (b64hash != existingUser.PasswordHash)
                    return BadRequest(genericLoginFail);

                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 120, userDto.Username);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult ChangePassword(ChangePasswordDto changePasswordDto)
        {
            try
            {

                if (string.IsNullOrWhiteSpace(changePasswordDto.Username) ||
                    string.IsNullOrWhiteSpace(changePasswordDto.NewPassword))
                {
                    return BadRequest("There is no input");
                }


                var existingUser = _context.UserDetails
                    .FirstOrDefault(x => x.Username == changePasswordDto.Username);
                if (existingUser == null)
                {
                    return BadRequest("User does not exist");
                }

                var newSalt = PasswordHashProvider.GetSalt();
                var newHash = PasswordHashProvider.GetHash(changePasswordDto.NewPassword, newSalt);

                existingUser.PasswordHash = newHash;
                existingUser.PasswordSalt = newSalt;

                _context.Update(existingUser);
                _context.SaveChanges();

                return Ok("Password was changed successfully");
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }
    }
}
