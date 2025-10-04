using AssetManagment.Models.DTO;

namespace AssetManagment.Services.Interfaces
{
    public interface IAssetService
    {
        Task<AssetSearchResult> CreateAsync(AssetSearchResult model);
        Task<AssetSearchResult> UpdateAsync(AssetSearchResult model);
        Task DeleteAsync(int id);
        Task<AssetSearchResult?> GetAssetByIdAsync(int id);
    }
}
