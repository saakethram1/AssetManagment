using System.ComponentModel.DataAnnotations;

namespace AssetManagment.Models.DTO
{
    public class AssetSearchResult : IValidatableObject
    {
        public int Id { get; set; } // 0 => new

        [Required(ErrorMessage = "Asset Name is required")]
        [MaxLength(200, ErrorMessage = "Asset Name cannot exceed 200 characters")]
        public string AssetName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Asset Type is required")]
        [MaxLength(100, ErrorMessage = "Asset Type cannot exceed 100 characters")]
        public string AssetType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        [MaxLength(150, ErrorMessage = "Model cannot exceed 150 characters")]
        public string? Model { get; set; }

        [Required(ErrorMessage = "Serial Number is required")]
        [MaxLength(100, ErrorMessage = "Serial Number cannot exceed 100 characters")]
        public string? SerialNumber { get; set; }

        [Required(ErrorMessage = "Purchase Date is required")]
        public DateTime? PurchaseDate { get; set; }

        [Required(ErrorMessage = "Warranty Expiry Date is required")]
        public DateTime? WarrantyExpiryDate { get; set; }

        [Required(ErrorMessage = "Condition is required")]
        public string? Condition { get; set; }

        [Required(ErrorMessage = "Status is required")]
        public string? Status { get; set; } // maps to AssetStatus enum name

        [Required(ErrorMessage = "IsSpare flag is required")]
        public bool IsSpare { get; set; }

        [Required(ErrorMessage = "Specifications are required")]
        [MaxLength(1000, ErrorMessage = "Specifications cannot exceed 1000 characters")]
        public string? Specifications { get; set; }

        // Custom cross-field validation
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (PurchaseDate.HasValue && WarrantyExpiryDate.HasValue)
            {
                if (WarrantyExpiryDate.Value < PurchaseDate.Value)
                {
                    yield return new ValidationResult(
                        "Warranty expiry date cannot be earlier than the purchase date.",
                        new[] { nameof(WarrantyExpiryDate) });
                }
            }

            if (PurchaseDate.HasValue && PurchaseDate.Value > DateTime.UtcNow)
            {
                yield return new ValidationResult(
                    "Purchase date cannot be in the future.",
                    new[] { nameof(PurchaseDate) });
            }
        }
    }
}
