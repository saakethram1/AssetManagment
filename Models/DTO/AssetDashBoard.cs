namespace AssetManagment.Models.DTO
{
    public record AssetTypeCount(string AssetType, int Count);

    public class AssetDashboard
    {
        public int TotalAssets { get; init; }
        public int AssignedAssets { get; init; }
        public int AvailableAssets { get; init; }
        public int UnderRepair { get; init; }
        public int Retired { get; init; }
        public int SpareAssets { get; init; }

        public List<AssetTypeCount> AssetsByType { get; init; } = new();
    }
}
