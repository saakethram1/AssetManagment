using AssetManagment.Models.DTO;

namespace AssetManagment.Services.Interfaces
{
    public interface IAssetQueryService
    {
        Task<PagedResult<AssetSearchResult>> SearchAssetsPagedAsync(
        List<string>? assetTypes,
        List<int>? statuses,
        string serialSearch,
        int pageNumber,
        int pageSize
            );

        // Dapper assignment history (optional, used by dashboard)
        Task<PagedResult<AssignmentHistory>> SearchAssignmentHistoryPagedAsync(
            List<string>? assetTypes,
            List<int>? statuses,
            string serialSearch,
            string employeeSearch,
            string? sortBy,
            bool sortDesc,
            int pageNumber,
            int pageSize);

        Task<AssetFilterValues> GetFilterValuesAsync();
        Task<AssetDashboard> GetDashboardAsync();
       
    }
    public record AssetFilterValues(
           List<string> AssetTypes,
           Dictionary<int, string> Statuses,
           List<string> Conditions
       );
}
