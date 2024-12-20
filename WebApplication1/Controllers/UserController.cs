using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Security.Claims;
using WebApplication1.Context;
using WebApplication1.Models;


namespace WebApplication1.Controllers
{
    public class UserController : Controller
    {
        FlightDBcontext _dbcontext = new FlightDBcontext();
        [HttpGet]
        public IActionResult UserDashBoard()
        {
            // Get the userId from claims (instead of using HttpContext.Session)
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user details. Please log in again.";
                return RedirectToAction("Login", "Home"); // Redirect to login if invalid
            }

            // Fetch user details from the database
            var user = _dbcontext.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Home"); // Redirect if user not found
            }

            // Pass user details to ViewBag (ensure null checks)
            ViewBag.UserName = string.IsNullOrEmpty(user.FullName) ? "User" : user.FullName;
            ViewBag.Email = string.IsNullOrEmpty(user.Email) ? "Not Available" : user.Email;

            // Generate initials (e.g., John Doe → JD), handle if FullName is null or empty
            var initials = string.IsNullOrEmpty(user.FullName)
                ? "NA"
                : string.Join("", user.FullName.Split(' ').Select(n => n[0])).ToUpper();
            ViewBag.Initials = initials;

            return View();
        }

        [HttpGet]
        public IActionResult EditProfile()
        {
            // Get the userId from claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user details. Please log in again.";
                return RedirectToAction("Login", "Home"); // Redirect to login if invalid
            }

            // Fetch user details from the database
            var user = _dbcontext.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Home"); // Redirect if user not found
            }

            return View(user); // Pass user details to the view
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile([Bind("FullName,Email")] User updatedUser)
        {
            // Get the userId from claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);

            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user details. Please log in again.";
                return RedirectToAction("Login", "Home");
            }

            // Fetch the current user from the database
            var user = _dbcontext.Users.FirstOrDefault(u => u.UserId == userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found. Please log in again.";
                return RedirectToAction("Login", "Home");
            }

            // Check if the model state is valid
           

            // Update only the explicitly bound properties
            user.FullName = updatedUser.FullName;
            user.Email = updatedUser.Email;

            try
            {
                _dbcontext.SaveChanges(); // Save changes to the database
                TempData["SuccessMessage"] = "Profile updated successfully!";
                return RedirectToAction("UserDashBoard");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while saving changes. Please try again.";
                Console.WriteLine(ex.Message); // Log the error
            }

            return View(user); // Pass the current user data back to the view in case of an error
        }







    }
}
