using System;
using System.Collections.Generic;

namespace RoomManagement.Models;

public partial class Room
{
    public long Id { get; set; }

    public string? RoomCode { get; set; }

    public string? RoomName { get; set; }

    public decimal? Price { get; set; }

    public int? Area { get; set; }

    public int? MaxPeople { get; set; }

    public string? Status { get; set; }

    public string? Description { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    public virtual ICollection<Contract> Contracts { get; set; } = new List<Contract>();
}
