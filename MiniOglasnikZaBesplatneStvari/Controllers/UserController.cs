using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MiniOglasnikZaBesplatneStvari.Dtos;
using MiniOglasnikZaBesplatneStvari.Models;
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

        [HttpGet("[action]")]
        public ActionResult GetToken()
        {
            try
            {
                // The same secure key must be used here to create JWT,
                // as the one that is used by middleware to verify JWT
                var secureKey = _configuration["JWT:SecureKey"];
                var serializedToken = JwtTokenProvider.CreateToken(secureKey, 10);

                return Ok(serializedToken);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.Message);
            }
        }

        [HttpPost("[action]")]
        public ActionResult<UserDetailsDto> Register(UserDetailsDto UserDetailsDto)
        {
            try
            {
                // Check if there is such a username in the database already
                var trimmedUsername = UserDetailsDto.Username.Trim();
                if (_context.UserDetails.Any(x => x.Username.Equals(trimmedUsername)))
                    return BadRequest($"Username {trimmedUsername} already exists");

                // Hash the password
                var b64salt = PasswordHashProvider.GetSalt();
                var b64hash = PasswordHashProvider.GetHash(UserDetailsDto.Password, b64salt);

                // Create user from DTO and hashed password
                var user = new UserDetail
                {
                    IdUserDetails = UserDetailsDto.Id,
                    Username = UserDetailsDto.Username,
                    PasswordHash = b64hash,
                    PasswordSalt = b64salt,
                    Email = UserDetailsDto.Email,
                    Phone = UserDetailsDto.Phone,
                };

                // Add user and save changes to database
                _context.Add(user);
                _context.SaveChanges();

                // Update DTO Id to return it to the client
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

                // Try to get a user from database
                var existingUser = _context.UserDetails.FirstOrDefault(x => x.Username == userDto.Username);
                if (existingUser == null)
                    return BadRequest(genericLoginFail);

                // Check is password hash matches
                var b64hash = PasswordHashProvider.GetHash(userDto.Password, existingUser.PasswordSalt);
                if (b64hash != existingUser.PasswordHash)
                    return BadRequest(genericLoginFail);

                // Create and return JWT token
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
