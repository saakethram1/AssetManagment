using AssetManagment.Models.Domain;

namespace AssetManagment.Services.Interfaces
{
    public interface IAssignmentService
    {
        Task AssignAsync(int assetId, int employeeId, string? notes = null);

        // Return an assignment (set returned date, update asset status)
        Task ReturnAsync(int assignmentId, DateTime returnedOn);

        // Get current assignment for an asset (if any)
        Task<AssetAssignment?> GetCurrentAssignmentForAssetAsync(int assetId);

        // History via EF (optional)
        Task<List<AssetAssignment>> GetHistoryByAssetAsync(int assetId);
        Task<List<AssetAssignment>> GetHistoryByEmployeeAsync(int employeeId);
    }
}
