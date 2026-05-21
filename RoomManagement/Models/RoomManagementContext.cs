using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace RoomManagement.Models;

public partial class RoomManagementContext : DbContext
{
    public RoomManagementContext()
    {
    }

    public RoomManagementContext(DbContextOptions<RoomManagementContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillDetail> BillDetails { get; set; }

    public virtual DbSet<Contract> Contracts { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__bills__3213E83F65FB2837");

            entity.ToTable("bills");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Month).HasColumnName("month");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.RoomPrice)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("room_price");
            entity.Property(e => e.ServiceTotal)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("service_total");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.HasQueryFilter(e => e.DeletedAt == null);
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Year).HasColumnName("year");

            entity.HasOne(d => d.Room).WithMany(p => p.Bills)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("bills_ibfk_1");
        });

        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__bill_det__3213E83FBF4D3E3F");

            entity.ToTable("bill_details");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.BillId).HasColumnName("bill_id");
            entity.Property(e => e.NewIndex).HasColumnName("new_index");
            entity.Property(e => e.OldIndex).HasColumnName("old_index");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.Quantity).HasColumnName("quantity");
            entity.Property(e => e.ServiceId).HasColumnName("service_id");
            entity.Property(e => e.Total)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total");

            entity.HasOne(d => d.Bill).WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.BillId)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_bill_details_bill");

            entity.HasOne(d => d.Service).WithMany(p => p.BillDetails)
                .HasForeignKey(d => d.ServiceId)
                .HasConstraintName("bill_details_ibfk_2");
        });

        modelBuilder.Entity<Contract>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__contract__3213E83F1BC73F54");

            entity.ToTable("contracts");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Deposit)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("deposit");
            entity.Property(e => e.EndDay).HasColumnName("end_day");
            entity.Property(e => e.RoomId).HasColumnName("room_id");
            entity.Property(e => e.StartDay).HasColumnName("start_day");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.TenantId).HasColumnName("tenant_id");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.HasQueryFilter(e => e.DeletedAt == null);
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");

            entity.HasOne(d => d.Room).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.RoomId)
                .HasConstraintName("FK_contracts_rooms");

            entity.HasOne(d => d.Tenant).WithMany(p => p.Contracts)
                .HasForeignKey(d => d.TenantId)
                .HasConstraintName("FK_contracts_tenants");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__payments__3213E83F0DCD74CD");

            entity.ToTable("payments");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.BillId).HasColumnName("bill_id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.HasQueryFilter(e => e.DeletedAt == null);
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
            entity.Property(e => e.Note).HasColumnName("note");
            entity.Property(e => e.PaymentDate).HasColumnName("payment_date");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(50)
                .HasColumnName("payment_method");

            entity.HasOne(d => d.Bill).WithMany(p => p.Payments)
                .HasForeignKey(d => d.BillId)
                .HasConstraintName("payments_ibfk_1");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__rooms__3213E83F675C0AB5");

            entity.ToTable("rooms");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Area).HasColumnName("area");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description).HasColumnName("description");
            entity.Property(e => e.MaxPeople).HasColumnName("max_people");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.RoomCode)
                .HasMaxLength(20)
                .HasColumnName("room_code");
            entity.Property(e => e.RoomName)
                .HasMaxLength(100)
                .HasColumnName("room_name");
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.HasQueryFilter(e => e.DeletedAt == null);
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__services__3213E83F292F1CED");

            entity.ToTable("services");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Price)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price");
            entity.Property(e => e.ServiceName)
                .HasMaxLength(100)
                .HasColumnName("service_name");
            entity.Property(e => e.Unit)
                .HasMaxLength(50)
                .HasColumnName("unit");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.HasQueryFilter(e => e.DeletedAt == null);
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<Tenant>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tenants__3213E83FEDA81A33");

            entity.ToTable("tenants");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .HasColumnName("address");
            entity.Property(e => e.Birthday).HasColumnName("birthday");
            entity.Property(e => e.Cccd)
                .HasMaxLength(20)
                .HasColumnName("cccd");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Gender)
                .HasMaxLength(10)
                .HasColumnName("gender");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.HasQueryFilter(e => e.DeletedAt == null);
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83F1C887057");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Image)
                .HasMaxLength(255)
                .HasColumnName("image");
            entity.Property(e => e.Name)
                .HasMaxLength(100)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(20)
                .HasColumnName("phone");
            entity.Property(e => e.Role)
                .HasMaxLength(20)
                .HasColumnName("role");
            entity.Property(e => e.UpdatedAt)
                .HasColumnType("datetime")
                .HasColumnName("updated_at");
            entity.HasQueryFilter(e => e.DeletedAt == null);
            entity.Property(e => e.DeletedAt)
                .HasColumnType("datetime")
                .HasColumnName("deleted_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
