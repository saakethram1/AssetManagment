using AssetManagment.Data;
using AssetManagment.Models.Domain;
using AssetManagment.Models.DTO;
using AssetManagment.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AssetManagment.Services
{
    public class EmployeeService: IEmployeeService
    {
        private readonly ApplicationDbContext _db;
        public EmployeeService(ApplicationDbContext db) => _db = db;
        public async Task<PagedResult<Employee>> GetPagedAsync(int pageNumber, int pageSize, string? search = null)
        {
            var query = _db.Employees
                           .Where(e => string.IsNullOrWhiteSpace(search)
                                    || EF.Functions.Like(e.FullName, $"%{search}%")
                                    || EF.Functions.Like(e.Email, $"%{search}%")
                                    || EF.Functions.Like(e.Department ?? "", $"%{search}%"));

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(e => e.FullName)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Employee>
            {
                Items = items,
                TotalCount = total,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }

        public async Task<Employee> CreateAsync(Employee emp)
        {
            _db.Employees.Add(emp);
            await _db.SaveChangesAsync();
            return emp;
        }

        public async Task UpdateAsync(Employee emp)
        {
            var existing = await _db.Employees
         .FirstOrDefaultAsync(x => x.Id == emp.Id);

            if (existing is null)
                throw new KeyNotFoundException($"Employee {emp.Id} not found");

            // Option 1: copy explicitly
            existing.FullName = emp.FullName;
            existing.Department = emp.Department;
            existing.Email = emp.Email;
            existing.PhoneNumber = emp.PhoneNumber;
            existing.Designation = emp.Designation;
            existing.Status = emp.Status;
            // ...copy other fields

            await _db.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var entity = await _db.Employees.FindAsync(id);
            if (entity is null) return;
            _db.Employees.Remove(entity);
            await _db.SaveChangesAsync();
        }

        public async Task<Employee?> GetByIdAsync(int id)
        {
            var entity = await _db.Employees.FindAsync(id);
            if (entity is null) return null;    
            return entity;
        }
    }
}
