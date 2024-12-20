using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WebApplication1.Models;

public partial class Flight
{
    public int FlightId { get; set; }
    [Required(ErrorMessage = "Flight Number is required.")]
    public string FlightNumber { get; set; } = null!;
    [Required]    
    public int DepartureAirportId { get; set; }
    [Required]
    public int ArrivalAirportId { get; set; }

    
    public DateTime DepartureTime { get; set; }
    [Required(ErrorMessage = "Arrival time is required.")]
    public DateTime ArrivalTime { get; set; }
      [Required(ErrorMessage = "Price is required.")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0.")]
    public decimal Price { get; set; }

    public virtual Airport ArrivalAirport { get; set; } = null!;

    public virtual ICollection<Booking> Bookings { get; set; } = new List<Booking>();

    public virtual Airport DepartureAirport { get; set; } = null!;
}
