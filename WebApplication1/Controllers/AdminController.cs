using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.Context;
using WebApplication1.Models;

namespace WebApplication1.Controllers
{
    public class AdminController : Controller
    {
        FlightDBcontext _dbcontext = new FlightDBcontext();
        [HttpGet]
        public IActionResult Dashboard()
        {

            return View();  
        }

        public IActionResult ViewAirports()
        {
            var airports = _dbcontext.Airports.ToList();
            return View(airports); // ViewAirports.cshtml
        }

        // Add Airport
        public IActionResult AddAirport()
        {
            return View(); // AddAirport.cshtml
        }

        [HttpPost]
        public IActionResult AddAirport(Airport airport)
        {
            if (ModelState.IsValid)
            {
                _dbcontext.Airports.Add(airport);
                _dbcontext.SaveChanges();
                return RedirectToAction("ViewAirports");
            }
            return View(airport);
        }

        // Delete Airport
        public IActionResult DeleteAirport(int id)
        {
            var airport = _dbcontext.Airports.Find(id);
            if (airport != null)
            {
                _dbcontext.Airports.Remove(airport);
                _dbcontext.SaveChanges();
            }
            return RedirectToAction("ViewAirports");
        }
    }
}

