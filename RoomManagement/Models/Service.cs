using System;
using System.Collections.Generic;

namespace RoomManagement.Models;

public partial class Service
{
    public long Id { get; set; }

    public string? ServiceName { get; set; }

    public decimal? Price { get; set; }

    public string? Unit { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual ICollection<BillDetail> BillDetails { get; set; } = new List<BillDetail>();
}
