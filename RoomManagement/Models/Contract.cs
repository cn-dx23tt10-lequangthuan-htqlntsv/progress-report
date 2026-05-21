using System;
using System.Collections.Generic;

namespace RoomManagement.Models;

public partial class Contract
{
    public long Id { get; set; }

    public long? TenantId { get; set; }

    public long? RoomId { get; set; }

    public DateOnly? StartDay { get; set; }

    public DateOnly? EndDay { get; set; }

    public decimal? Deposit { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? DeletedAt { get; set; }

    public virtual Room? Room { get; set; }

    public virtual Tenant? Tenant { get; set; }
}
