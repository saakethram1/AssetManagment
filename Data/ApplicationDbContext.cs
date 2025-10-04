using AssetManagment.Models.Domain;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AssetManagment.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser>(options)
    {
        public DbSet<Employee> Employees => Set<Employee>();
        public DbSet<Asset> Assets => Set<Asset>();
        public DbSet<AssetAssignment> AssetAssignments => Set<AssetAssignment>();

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Employee>(e =>
            {
                e.HasIndex(x => x.Email).IsUnique();
            });

            builder.Entity<Asset>(a =>
            {
                a.HasIndex(x => x.SerialNumber).IsUnique(true);
            });

            builder.Entity<AssetAssignment>(aa =>
            {
                aa.HasOne(x => x.Asset)
                  .WithMany()
                  .HasForeignKey(x => x.AssetId)
                  .OnDelete(DeleteBehavior.Restrict);

                aa.HasOne(x => x.Employee)
                  .WithMany()
                  .HasForeignKey(x => x.EmployeeId)
                  .OnDelete(DeleteBehavior.Restrict);
            });
        }
    }

}
