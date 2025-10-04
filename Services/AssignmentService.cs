using AssetManagment.Data;
using AssetManagment.Models.Domain;
using AssetManagment.Models.Enums;
using AssetManagment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetManagment.Services
{
    public class AssignmentService:IAssignmentService
    {
        private readonly ApplicationDbContext _db;
        public AssignmentService(ApplicationDbContext db) => _db = db;

        public async Task AssignAsync(int assetId, int employeeId, string? notes = null)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            var asset = await _db.Assets.FindAsync(assetId);
            if (asset == null) throw new InvalidOperationException("Asset not found.");
            if (asset.Status != AssetStatus.Available) throw new InvalidOperationException("Only assets with status 'Available' can be assigned.");

            // create assignment
            var assignment = new AssetAssignment
            {
                AssetId = assetId,
                EmployeeId = employeeId,
                AssignedDate = DateTime.UtcNow,
                Notes = notes
            };
            _db.AssetAssignments.Add(assignment);

            // update asset status
            asset.Status = AssetStatus.Assigned;
            _db.Assets.Update(asset);

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }

        public async Task ReturnAsync(int assignmentId, DateTime returnedOn)
        {
            using var tx = await _db.Database.BeginTransactionAsync();

            var asg = await _db.AssetAssignments.FindAsync(assignmentId);
            if (asg == null) throw new InvalidOperationException("Assignment not found.");
            if (asg.ReturnedDate != null) throw new InvalidOperationException("Assignment already returned.");

            asg.ReturnedDate = returnedOn;
            _db.AssetAssignments.Update(asg);

            var asset = await _db.Assets.FindAsync(asg.AssetId);
            if (asset != null)
            {
                asset.Status = AssetStatus.Available;
                _db.Assets.Update(asset);
            }

            await _db.SaveChangesAsync();
            await tx.CommitAsync();
        }

        public Task<AssetAssignment?> GetCurrentAssignmentForAssetAsync(int assetId)
            => _db.AssetAssignments
                  .Include(x => x.Employee)
                  .FirstOrDefaultAsync(x => x.AssetId == assetId && x.ReturnedDate == null);

        public Task<List<AssetAssignment>> GetHistoryByAssetAsync(int assetId)
            => _db.AssetAssignments
                  .Include(x => x.Employee)
                  .Where(x => x.AssetId == assetId)
                  .OrderByDescending(x => x.AssignedDate)
                  .ToListAsync();

        public Task<List<AssetAssignment>> GetHistoryByEmployeeAsync(int employeeId)
            => _db.AssetAssignments
                  .Include(x => x.Asset)
                  .Where(x => x.EmployeeId == employeeId)
                  .OrderByDescending(x => x.AssignedDate)
                  .ToListAsync();
    }
}
