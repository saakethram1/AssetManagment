using AssetManagment.Models.Domain;
using AssetManagment.Models.DTO;

namespace AssetManagment.Services.Interfaces
{
    public interface IEmployeeService
    {
        Task<PagedResult<Employee>> GetPagedAsync(int pageNumber, int pageSize, string? search = null);
        Task<Employee?> GetByIdAsync(int id);
        Task<Employee> CreateAsync(Employee emp);
        Task UpdateAsync(Employee emp);
        Task DeleteAsync(int id);
    }
}
