using ProductAPI.Models;

namespace ProductAPI.Services
{
    public interface IDepartmentService
    {
        Task<IEnumerable<Department>> GetAllAsync();
        Task<Department?> GetByCodigoAsync(string codigo);
        Task<bool> ExistsAsync(string codigo);
    }
}