using ActualLab.Fusion.EntityFramework.Operations;
using LiftManagement.HelloBlazorServer.Models;
using LiftManagement.HelloBlazorServer.Models.Lift;
using Microsoft.EntityFrameworkCore;

namespace LiftManagement.HelloBlazorServer.Db
{
    public class AppDbContext:DbContext 
    {
        public AppDbContext(DbContextOptions<AppDbContext> options):base(options)
        { 
        }
        public DbSet<Elevator> Elevator { get; set; }
        public DbSet<ElevatorQueue> Queues { get; set; }


        // ActualLab.Fusion.EntityFramework.Operations tables
        public DbSet<DbOperation> Operations { get; protected set; } = null!;

          protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ElevatorQueue>()
            .Property(e => e.RequestedAt)
            .HasConversion(
                v => v.ToUniversalTime(), // Преобразование в UTC при сохранении
                v => DateTime.SpecifyKind(v, DateTimeKind.Utc) // Преобразование в UTC при чтении
            );
    }
    }
}
