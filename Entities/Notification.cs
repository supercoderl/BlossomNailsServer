﻿// <auto-generated> This file has been auto generated by EF Core Power Tools. </auto-generated>
#nullable disable
using System;
using System.Collections.Generic;

namespace BlossomServer.Entities;

public partial class Notification
{
    public int NotificationID { get; set; }

    public Guid? Receiver { get; set; }

    public string Type { get; set; }

    public int? ObjectID { get; set; }

    public string Message { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? ReadAt { get; set; }

    public virtual User ReceiverNavigation { get; set; }
}