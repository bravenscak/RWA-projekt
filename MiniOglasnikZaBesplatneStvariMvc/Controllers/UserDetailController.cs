using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MiniOglasnikZaBesplatneStvariLibrary.Models;
using MiniOglasnikZaBesplatneStvariMvc.Models;
using MiniOglasnikZaBesplatneStvariMvc.Security;
using System.Security.Claims;

namespace MiniOglasnikZaBesplatneStvariMvc.Controllers
{
    public class UserDetailController : Controller
    {
        private readonly AdvertisementRwaContext _context;

        public UserDetailController(AdvertisementRwaContext context, IConfiguration configuration)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Login(string returnUrl)
        {
            ViewData["HideNavbar"] = true;
            var userLoginViewModel = new UserLoginViewModel
            {
                ReturnUrl = returnUrl
            };
            return View(userLoginViewModel);
        }

        [HttpPost]
        public IActionResult Login(UserLoginViewModel userLoginViewModel) {
            var existingUser = _context.UserDetails.Include(u => u.UserRole).FirstOrDefault(u => u.Username == userLoginViewModel.Username);
            if (existingUser == null)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var hash = PasswordHashProvider.GetHash(userLoginViewModel.Password, existingUser.PasswordSalt);
            if (hash != existingUser.PasswordHash)
            {
                ModelState.AddModelError("", "Invalid username or password");
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, existingUser.Username),
                new Claim(ClaimTypes.Role, existingUser.UserRole.Name)
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            var authProperties = new AuthenticationProperties();

            Task.Run(() => HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties)).GetAwaiter().GetResult();

            if (!string.IsNullOrEmpty(userLoginViewModel.ReturnUrl))
            {
                return LocalRedirect(userLoginViewModel.ReturnUrl);
            }
            else
            {
                return RedirectToAction("Index", "Item");
            }
        }

        public IActionResult Logout()
        {
            Task.Run(() => HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme)).GetAwaiter().GetResult();
            return View();
        }

        public IActionResult Register()
        {
            ViewData["HideNavbar"] = true;
            return View();
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult Register(UserDetailViewModel userDetailViewModel)
        {
            var trimmedUsername = userDetailViewModel.Username.Trim();

            if (_context.UserDetails.Any(u => u.Username == trimmedUsername))
            {
                ModelState.AddModelError("Username", "Username already exists");
                return View();
            }

            return RedirectToAction("Confirm registration", userDetailViewModel);
        }

        public IActionResult ConfirmRegistration(UserDetailViewModel userDetailViewModel)
        {
            return View(userDetailViewModel);
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ConfirmRegistrationPost(UserDetailViewModel userDetailViewModel)
        {
            var salt = PasswordHashProvider.GetSalt();
            var hash = PasswordHashProvider.GetHash(userDetailViewModel.Password, salt);

            var userDetail = new UserDetail
            {
                Username = userDetailViewModel.Username,
                PasswordHash = hash,
                PasswordSalt = salt,
                Email = userDetailViewModel.Email,
                Phone = userDetailViewModel.Phone,
                UserRoleId = 2
            };

            _context.UserDetails.Add(userDetail);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Registration successful! You can now log in.";

            return RedirectToAction("Login");
        }

        public IActionResult ProfileDetails()
        {
            var username = HttpContext.User.Identity.Name;

            var userDetail = _context.UserDetails.FirstOrDefault(u => u.Username == username);
            var userDetailViewModel = new UserDetailViewModel
            {
                Username = userDetail.Username,
                Email = userDetail.Email,
                Phone = userDetail.Phone
            };

            return View(userDetailViewModel);
        }

        [Authorize]
        public IActionResult EditProfile(int id)
        {
            var userDetail = _context.UserDetails.First(u => u.IdUserDetails == id);
            var userDetailViewModel = new UserDetailViewModel
            {
                Username = userDetail.Username,
                Email = userDetail.Email,
                Phone = userDetail.Phone
            };

            return View(userDetailViewModel);
        }

        [Authorize]
        [HttpPost]
        public IActionResult EditProfile(int id, UserDetailViewModel userDetailViewModel)
        {
            var userDetail = _context.UserDetails.First(u => u.IdUserDetails == id);
            userDetail.Username = userDetailViewModel.Username;
            userDetail.Email = userDetailViewModel.Email;
            userDetail.Phone = userDetailViewModel.Phone;

            _context.SaveChanges();

            return RedirectToAction("ProfileDetails");
        }

        [HttpPut]
        public ActionResult SetProfileData(int id, [FromBody] UserDetailViewModel userDetailViewModel)
        {
            var userDetail = _context.UserDetails.First(u => u.IdUserDetails == id);
            userDetail.Username = userDetailViewModel.Username;
            userDetail.Email = userDetailViewModel.Email;
            userDetail.Phone = userDetailViewModel.Phone;

            _context.SaveChanges();

            return Ok();
        }
    }
}
  