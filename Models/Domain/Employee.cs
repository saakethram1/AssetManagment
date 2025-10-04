using AssetManagment.Models.Enums;
using System.ComponentModel.DataAnnotations;

namespace AssetManagment.Models.Domain
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Full Name is required")]
        [MaxLength(150, ErrorMessage = "Full Name cannot exceed 150 characters")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department is required")]
        [MaxLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
        public string Department { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email is required")]
        [MaxLength(150, ErrorMessage = "Email cannot exceed 150 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone Number is required")]
        [MaxLength(25, ErrorMessage = "Phone Number cannot exceed 25 characters")]
        [RegularExpression(@"^\+?[1-9]\d{1,14}$", ErrorMessage = "Invalid phone number format. Use E.164 format (e.g. +1234567890).")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Designation is required")]
        [MaxLength(100, ErrorMessage = "Designation cannot exceed 100 characters")]
        public string Designation { get; set; } = string.Empty;

        [Required(ErrorMessage = "Employment Status is required")]
        public EmploymentStatus Status { get; set; } = EmploymentStatus.Active;

        [Required]
        public DateTime CreatedAtUtc { get; set; } = DateTime.UtcNow;

        // Optional since updates happen later
        public DateTime? UpdatedAtUtc { get; set; }
    }
}
