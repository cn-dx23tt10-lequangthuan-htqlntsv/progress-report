using System;
using System.Collections.Generic;

namespace RoomManagement.Models;

public partial class Payment
{
    public long Id { get; set; }

    public long? BillId { get; set; }

    public decimal? Amount { get; set; }

    public string? PaymentMethod { get; set; }

    public DateOnly? PaymentDate { get; set; }

    public string? Note { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Bill? Bill { get; set; }
}
