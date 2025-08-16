using ProductAPI.DTOs;

namespace ProductAPI.Services
{
    public interface IProductService
    {
        Task<IEnumerable<ProductResponseDto>> GetAllAsync();
        Task<ProductResponseDto?> GetByIdAsync(Guid id);
        Task<ProductResponseDto> CreateAsync(CreateProductDto createProductDto);
        Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto updateProductDto);
        Task<bool> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
        Task<bool> CodigoExistsAsync(string codigo, Guid? excludeId = null);
    }
}