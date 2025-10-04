using System.ComponentModel.DataAnnotations.Schema;

namespace AssetManagment.Models.Domain
{
    public class AssetAssignment
    {
        public int Id { get; set; }

        public int AssetId { get; set; }
        public int EmployeeId { get; set; }

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ReturnedDate { get; set; }
        public string? Notes { get; set; }

        [ForeignKey(nameof(AssetId))] public Asset? Asset { get; set; }
        [ForeignKey(nameof(EmployeeId))] public Employee? Employee { get; set; }
    }
}
