using System;
using System.Collections.Generic;

namespace RoomManagement.Models;

public partial class Tenant
{
    public long Id { get; set; }

    public string? Name { get; set; }

    public string? Phone { get; set; }

    public string? Cccd { get; set; }

    public DateOnly? Birthday { get; set; }

    public string? Gender { get; set; }

    public string? Address { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
