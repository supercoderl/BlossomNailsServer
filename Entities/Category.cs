﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace BlossomServer.Entities;

public partial class Category
{
    public int CategoryID { get; set; }

    public string Name { get; set; }

    public bool IsActive { get; set; }

    public int Priority { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Service> Services { get; set; } = new List<Service>();
}