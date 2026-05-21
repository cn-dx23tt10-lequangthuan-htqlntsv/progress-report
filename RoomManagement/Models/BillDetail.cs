using System;
using System.Collections.Generic;

namespace RoomManagement.Models;

public partial class BillDetail
{
    public long Id { get; set; }

    public long? BillId { get; set; }

    public long? ServiceId { get; set; }

    public int? OldIndex { get; set; }

    public int? NewIndex { get; set; }

    public int? Quantity { get; set; }

    public decimal? Price { get; set; }

    public decimal? Total { get; set; }

    public virtual Bill? Bill { get; set; }

    public virtual Service? Service { get; set; }
}
