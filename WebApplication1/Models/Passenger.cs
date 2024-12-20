using System;
using System.Collections.Generic;

namespace WebApplication1.Models;

public partial class Passenger
{
    public int PassengerId { get; set; }

    public int BookingId { get; set; }

    public string FullName { get; set; } = null!;

    public int? Age { get; set; }

    public string? Gender { get; set; }

    public virtual Booking Booking { get; set; } = null!;
}
