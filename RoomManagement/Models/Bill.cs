using System;
using System.Collections.Generic;

namespace RoomManagement.Models;

public partial class Bill
{
    public long Id { get; set; }

    public long? RoomId { get; set; }

    public int? Month { get; set; }

    public int? Year { get; set; }

    public decimal? RoomPrice { get; set; }

    public decimal? ServiceTotal { get; set; }

    public decimal? Total { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    public virtual Room? Room { get; set; }
}
