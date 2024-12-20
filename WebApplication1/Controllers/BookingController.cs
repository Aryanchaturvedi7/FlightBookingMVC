using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class BookingController : Controller
    {
        private readonly FlightDBcontext _dbcontext;

        public BookingController(FlightDBcontext dbcontext)
        {
            _dbcontext = dbcontext;
        }

        [HttpGet]
        public IActionResult BookFlight(int flightId)
        {
            // Fetch flight details using flightId
            var flight = _dbcontext.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .FirstOrDefault(f => f.FlightId == flightId);

            if (flight == null)
            {
                TempData["ErrorMessage"] = "Flight not found.";
                return RedirectToAction("SearchFlights", "User");
            }

            // Pass flight details to the view
            return View(flight);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmBooking(int flightId, decimal totalPrice)
        {
            // Get the userId from claims
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user details. Please log in again.";
                return RedirectToAction("Login", "Home");
            }

            // Fetch flight details
            var flight = _dbcontext.Flights.FirstOrDefault(f => f.FlightId == flightId);
            if (flight == null)
            {
                TempData["ErrorMessage"] = "Flight not found.";
                return RedirectToAction("SearchFlights", "User");
            }

            // Create a new booking
            var booking = new Booking
            {
                FlightId = flightId,
                UserId = userId,
                BookingDate = DateTime.Now,
                Status = "Confirmed"
            };

            _dbcontext.Bookings.Add(booking);
            _dbcontext.SaveChanges();

            ViewBag.SuccessMessage = "Booking confirmed successfully!";
            return RedirectToAction("UserDashBoard", "User");
        }

        [HttpGet]
        public IActionResult ViewBookings()
        {
            // Same logic as fetching bookings for UserDashBoard
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
            {
                TempData["ErrorMessage"] = "Unable to retrieve user details. Please log in again.";
                return RedirectToAction("Login", "Home");
            }

            var bookings = _dbcontext.Bookings
                .Include(b => b.Flight)
                .Where(b => b.UserId == userId)
                .ToList();

            return View(bookings);
        }
    }
}
