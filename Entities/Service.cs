﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace BlossomServer.Entities;

public partial class Service
{
    public int ServiceID { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int? CategoryID { get; set; }

    public decimal Price { get; set; }

    public string WorkingTime { get; set; }

    public string RepresentativeImage { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Category Category { get; set; }

    public virtual ICollection<ServiceBooking> ServiceBookings { get; set; } = new List<ServiceBooking>();

    public virtual ICollection<ServiceImage> ServiceImages { get; set; } = new List<ServiceImage>();
}