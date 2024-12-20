using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Airport
{
    public int AirportId { get; set; }

    public string Name { get; set; } = null!;

    public string Code { get; set; } = null!;

    public string? City { get; set; }

    public string? Country { get; set; }

    public virtual ICollection<Flight> FlightArrivalAirports { get; set; } = new List<Flight>();

    public virtual ICollection<Flight> FlightDepartureAirports { get; set; } = new List<Flight>();
}
