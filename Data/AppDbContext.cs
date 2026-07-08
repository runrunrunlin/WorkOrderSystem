using Microsoft.EntityFrameworkCore;
using WorkOrderSystem.Models;

namespace WorkOrderSystem.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Equipment> Equipment => Set<Equipment>();
    public DbSet<WorkOrder> WorkOrders => Set<WorkOrder>();
    public DbSet<Photo> Photos => Set<Photo>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<WorkOrder>()
            .HasOne(w => w.ReportedBy)
            .WithMany(u => u.ReportedOrders)
            .HasForeignKey(w => w.ReportedById)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<WorkOrder>()
            .HasOne(w => w.AssignedTo)
            .WithMany(u => u.AssignedOrders)
            .HasForeignKey(w => w.AssignedToId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Photo>()
            .HasOne(p => p.WorkOrder)
            .WithMany()
            .HasForeignKey(p => p.WorkOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Photo>()
            .HasOne(p => p.UploadedBy)
            .WithMany()
            .HasForeignKey(p => p.UploadedById)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
