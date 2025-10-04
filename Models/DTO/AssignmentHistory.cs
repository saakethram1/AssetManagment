using AssetManagment.Models.Enums;

namespace AssetManagment.Models.DTO
{
    public class AssignmentHistory
    {
        public int AssignmentId { get; set; }
        public int AssetId { get; set; }
        public string AssetName { get; set; } = string.Empty;
        public string? AssetType { get; set; }
        public string? SerialNumber { get; set; }
        public int StatusInt { get; set; }
        public string Status => ((AssetStatus)StatusInt).ToString();
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public DateTime? ReturnedDate { get; set; }
        public string? Notes { get; set; }
    }
}
