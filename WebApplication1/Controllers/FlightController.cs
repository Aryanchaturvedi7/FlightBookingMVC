using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
   // [Authorize(Roles = "Admin")]
    public class FlightController : Controller
    {
        FlightDBcontext _dbcontext = new FlightDBcontext();


        [HttpGet]
        public IActionResult ManageFlights()
        {
            // Fetch the list of flights to manage
            var flights = _dbcontext.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .ToList();

            ViewBag.Airports = new SelectList(_dbcontext.Airports.ToList(), "AirportId", "Name");
            return View(flights); // Return the view with the list of flights
        }


        [HttpGet]
        public IActionResult ViewFlights()
        {
            var flights = _dbcontext.Flights
             .Include(f => f.DepartureAirport)  // Include departure airport data
             .Include(f => f.ArrivalAirport)    // Include arrival airport data
             .ToList(); // Retrieve flights from the database
            return View(flights);
        }

       


        
        
        [HttpGet]
        public IActionResult AddFlight()
        {
            // Get all airports from the database to populate the dropdown lists
            var airports = _dbcontext.Airports.ToList();
            // Create a SelectList from the airports list (to bind with the dropdown)
            ViewBag.Airports = new SelectList(airports, "AirportId", "Name");

            return View();
        }

        [HttpPost]
        
        public IActionResult AddFlight(int departureAirportId, int arrivalAirportId, string flightNumber, DateTime departureTime, DateTime arrivalTime, decimal price)
        {
            // Manually create a new Flight object
            var f = new Flight
            {
                FlightNumber = flightNumber,
                DepartureAirportId = departureAirportId,
                ArrivalAirportId = arrivalAirportId,
                DepartureTime = departureTime,
                ArrivalTime = arrivalTime,
                Price = price
            };
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                _dbcontext.Flights.Add(f);
                _dbcontext.SaveChanges();
                return RedirectToAction("ViewFlights");
            }

           
           

            ViewBag.Airports = new SelectList(_dbcontext.Airports.ToList(), "AirportId", "Name");
            return View(f); // Return the view with the model if validation fails
        }
        [HttpGet]
        public IActionResult EditFlight(int id)
        {
            var flight = _dbcontext.Flights.FirstOrDefault(f => f.FlightId == id);
            if (flight == null)
            {
                return NotFound(); // Flight not found
            }

            var airports = _dbcontext.Airports.ToList();
            ViewBag.Airports = new SelectList(airports, "AirportId", "Name"); // Correctly set SelectList
            return View(flight); // Pass the flight object to the view for editing
        }


        [HttpPost]
        public IActionResult EditFlight(int id, IFormCollection form)
        {
            var existingFlight = _dbcontext.Flights
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .FirstOrDefault(f => f.FlightId == id); // Find the flight by ID

            if (existingFlight == null)
            {
                return NotFound(); // Return 404 if flight not found
            }

            // Explicit binding: Manually assigning the form values to the model
            existingFlight.FlightNumber = form["FlightNumber"]; // Manually bind FlightNumber
            existingFlight.DepartureAirportId = Convert.ToInt32(form["DepartureAirportId"]); // Bind DepartureAirportId
            existingFlight.ArrivalAirportId = Convert.ToInt32(form["ArrivalAirportId"]); // Bind ArrivalAirportId
            existingFlight.DepartureTime = Convert.ToDateTime(form["DepartureTime"]); // Bind DepartureTime
            existingFlight.ArrivalTime = Convert.ToDateTime(form["ArrivalTime"]); // Bind ArrivalTime
            existingFlight.Price = Convert.ToDecimal(form["Price"]); // Bind Price

            // If necessary, validate values manually or re-apply custom validation
            if (string.IsNullOrEmpty(existingFlight.FlightNumber))
            {
                ModelState.AddModelError("FlightNumber", "Flight Number is required.");
            }
            if (existingFlight.DepartureAirportId == 0)
            {
                ModelState.AddModelError("DepartureAirportId", "Departure Airport is required.");
            }
            if (existingFlight.ArrivalAirportId == 0)
            {
                ModelState.AddModelError("ArrivalAirportId", "Arrival Airport is required.");
            }
            if (existingFlight.DepartureTime == DateTime.MinValue)
            {
                ModelState.AddModelError("DepartureTime", "Departure Time is required.");
            }
            if (existingFlight.ArrivalTime == DateTime.MinValue)
            {
                ModelState.AddModelError("ArrivalTime", "Arrival Time is required.");
            }
            if (existingFlight.Price <= 0)
            {
                ModelState.AddModelError("Price", "Price must be greater than 0.");
            }

            if (!ModelState.IsValid)
            {
                // Repopulate the airports dropdown list and return the view if validation fails
                ViewBag.Airports = _dbcontext.Airports.ToList();
                return View(existingFlight); // Return the flight object with model errors
            }

            _dbcontext.SaveChanges(); // Save changes to the database
            return RedirectToAction("ViewFlights"); // Redirect to the flight list page after updating
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteFlight(int id)
        {
            var flight = _dbcontext.Flights.FirstOrDefault(f => f.FlightId == id);
            if (flight == null)
            {
                return NotFound(); // Flight not found
            }

            _dbcontext.Flights.Remove(flight); // Remove the flight
            _dbcontext.SaveChanges(); // Save changes

            return RedirectToAction("ManageFlights"); // Redirect back to the Manage Flights page
        }

        [HttpGet]
        public IActionResult SearchFlights()
        {
            // Fetch all airports for dropdown
            var airports = _dbcontext.Airports.ToList();
            ViewBag.Airports = new SelectList(airports, "AirportId", "Name");

            // Pass an empty list of flights to the view
            return View(new List<Flight>());
        }




        // POST method for processing the search request
        [HttpPost]
        public IActionResult SearchFlights(int? departureAirportId, int? arrivalAirportId)
        {
            // Fetch all airports for dropdowns
            var airports = _dbcontext.Airports.ToList();
            ViewBag.Airports = new SelectList(airports, "AirportId", "Name");

            // Validate inputs
            if (departureAirportId == null || arrivalAirportId == null)
            {
                ViewBag.Message = "Both Departure and Arrival Airports must be selected.";
                return View(new List<Flight>());
            }

            // Filter flights based on selected airports
            var flights = _dbcontext.Flights
                .Where(f => f.DepartureAirportId == departureAirportId && f.ArrivalAirportId == arrivalAirportId)
                .Include(f => f.DepartureAirport)
                .Include(f => f.ArrivalAirport)
                .ToList();
            // Pass filtered flights to the view
            return View(flights);
        }
    }



}
    
