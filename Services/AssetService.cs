using AssetManagment.Data;
using AssetManagment.Models.Domain;
using AssetManagment.Models.DTO;
using AssetManagment.Models.Enums;
using AssetManagment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetManagment.Services
{
    public class AssetService : IAssetService
    {
        private readonly ApplicationDbContext _db;

        public AssetService(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<AssetSearchResult> CreateAsync(AssetSearchResult model)
        {
            var entity = new Asset
            {
                AssetName = model.AssetName,
                AssetType = model.AssetType,
                Model = model.Model,   // map correctly (MakeModel vs Model in entity)
                SerialNumber = model.SerialNumber,
                PurchaseDate = model.PurchaseDate,
                WarrantyExpiryDate = model.WarrantyExpiryDate,
                Condition = Enum.Parse<AssetCondition>(model.Condition), // convert string to enum
                Status = Enum.Parse<AssetStatus>(model.Status),          // convert string to enum
                IsSpare = model.IsSpare,
                Specifications = model.Specifications,
                CreatedAtUtc = DateTime.UtcNow
            };

            _db.Assets.Add(entity);
            await _db.SaveChangesAsync();

            return await MapToDtoAsync(entity.Id);
        }

        public async Task<AssetSearchResult> UpdateAsync(AssetSearchResult model)
        {
            var entity = await _db.Assets.FindAsync(model.Id);
            if (entity == null) throw new KeyNotFoundException("Asset not found");

            entity.AssetName = model.AssetName;
            entity.AssetType = model.AssetType;
            entity.Model = model.Model;
            entity.SerialNumber = model.SerialNumber;
            entity.PurchaseDate = model.PurchaseDate;
            entity.WarrantyExpiryDate = model.WarrantyExpiryDate;
            entity.Condition = Enum.Parse<AssetCondition>(model.Condition); // convert string to enum
            entity.Status = Enum.Parse<AssetStatus>(model.Status);
            entity.IsSpare = model.IsSpare;
            entity.Specifications = model.Specifications;
            entity.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync();

            return await MapToDtoAsync(entity.Id);
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Assets.FindAsync(id);
            if (entity != null)
            {
                _db.Assets.Remove(entity);
                await _db.SaveChangesAsync();
            }
        }

        public async Task<AssetSearchResult?> GetAssetByIdAsync(int id)
        {
            return await MapToDtoAsync(id);
        }

        // ---- Helper to build AssetSearchResult with assignment info ----
        private async Task<AssetSearchResult?> MapToDtoAsync(int id)
        {
            var asset = await _db.Assets
                .FirstOrDefaultAsync(a => a.Id == id);

            if (asset == null) return null;


            return new AssetSearchResult
            {
                Id = asset.Id,
                AssetName = asset.AssetName,
                AssetType = asset.AssetType,
                Model = asset.Model,
                SerialNumber = asset.SerialNumber,
                PurchaseDate = asset.PurchaseDate,
                WarrantyExpiryDate = asset.WarrantyExpiryDate,
                Condition = asset.Condition.ToString(), // display enum name
                Status = asset.Status.ToString(),       // display enum name
                IsSpare = asset.IsSpare,
                Specifications = asset.Specifications,
            };
        }
    }
}
