using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        FlightDBcontext _dbcontext = new FlightDBcontext();

        public IActionResult Index()
        {
            return View();
        }
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        [HttpGet]
        public IActionResult Register()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {

                if(_dbcontext.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("", "A user with this email already exists");
                    return View(user);
                }
                else
                {
                    user.PasswordHash = HashPassword(user.PasswordHash);
                    user.Email = user.Email;
                    user.FullName = user.FullName;
                    user.Role = "User";
                    
                    _dbcontext.Users.Add(user);
                    _dbcontext.SaveChanges();

                    return RedirectToAction("Login");
                }
            }

            return View(user);
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password)
        {
            var hashedpass = HashPassword(password); // Hash the input password
            var user = _dbcontext.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == hashedpass);

            if (user != null)
            {
                // Create claims for the authenticated user
                var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()), // UserId as claim
            new Claim(ClaimTypes.Name, user.Username), // Username as claim
            new Claim(ClaimTypes.Role, user.Role) // Role as claim (Admin or User)
        };

                // Create ClaimsIdentity and sign the user in
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

                // Sign the user in by setting the claims in the authentication cookie
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, claimsPrincipal);

                // Redirect based on user role
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                else if (user.Role == "User")
                {
                    return RedirectToAction("UserDashBoard", "User");
                }
                else
                {
                    ModelState.AddModelError("", "Role not recognized.");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid username or password.");
            }

            return View();
        }

        public async Task<IActionResult> Logout()
        {
            // Sign out the user
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Clear any session data
            HttpContext.Session.Clear();

            // Redirect to the login page after logging out
            return RedirectToAction("Login", "Home");
        }

    }
}

