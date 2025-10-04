using AssetManagment.Models.Domain;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace AssetManagment.Data
{
    public class ApplicationDbContextSeed
    {
        public static async Task SeedSampleAsync(IServiceProvider services)
        {
            using var scope = services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>(); // ✅

            await db.Database.MigrateAsync();

            var filePath = Path.Combine(AppContext.BaseDirectory, "Data", "SeedData.json");

            if (File.Exists(filePath))
            {
                var json = await File.ReadAllTextAsync(filePath);

                var seedData = JsonSerializer.Deserialize<SeedDataModel>(json);

                if (seedData != null)
                {
                    if (!await db.Employees.AnyAsync() && seedData.Employees?.Any() == true)
                    {
                        db.Employees.AddRange(seedData.Employees);
                    }

                    if (!await db.Assets.AnyAsync() && seedData.Assets?.Any() == true)
                    {
                        db.Assets.AddRange(seedData.Assets);
                    }

                    await db.SaveChangesAsync();
                }
            }
            else
            {
                // fallback if no JSON found (original 2 records)
                if (!await db.Employees.AnyAsync())
                {
                    db.Employees.AddRange(
                        new Employee { FullName = "Amit Sharma", Email = "amit@company.com", Department = "IT", Designation = "Engineer" },
                        new Employee { FullName = "Neha Gupta", Email = "neha@company.com", Department = "HR", Designation = "HR Executive" }
                    );
                }

                if (!await db.Assets.AnyAsync())
                {
                    db.Assets.AddRange(
                        new Asset { AssetName = "Dell Latitude 7420", AssetType = "Laptop", SerialNumber = "DL-7420-001" },
                        new Asset { AssetName = "HP ProDesk 400", AssetType = "Desktop", SerialNumber = "HP-400-XYZ", IsSpare = true }
                    );
                }

                await db.SaveChangesAsync();
            }
        }
    }

    public class SeedDataModel
    {
        public List<Employee> Employees { get; set; } = new();
        public List<Asset> Assets { get; set; } = new();
    }
}
