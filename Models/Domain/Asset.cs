using AssetManagment.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagment.Models.Domain
{
    public class Asset
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string AssetName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string? AssetType { get; set; }

        [Required,MaxLength(150)]
        public string? Model { get; set; }

        [Required,MaxLength(100)]
        public string? SerialNumber { get; set; }

        [Required,MaxLength(100)]
        public DateTime? PurchaseDate { get; set; }

        [Required,MaxLength(100)]
        public DateTime? WarrantyExpiryDate { get; set; }

        [Required,MaxLength(100)]
        public AssetCondition Condition { get; set; } = AssetCondition.New;
       
        [Required,MaxLength(100)]
        public AssetStatus Status { get; set; } = AssetStatus.Available;

        [Required]
        public bool IsSpare { get; set; }

        public string? Specifications { get; set; }

        [Required, MaxLength(100)]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
