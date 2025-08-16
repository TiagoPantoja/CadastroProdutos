using Dapper;
using ProductAPI.Data;
using ProductAPI.DTOs;

namespace ProductAPI.Services
{
    public class ProductService : IProductService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<ProductService> _logger;

        public ProductService(IDbConnectionFactory connectionFactory, ILogger<ProductService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<ProductResponseDto>> GetAllAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var query = @"
                    SELECT id, codigo, descricao, departamento, preco, status, created_at, updated_at
                    FROM products 
                    WHERE is_deleted = false 
                    ORDER BY created_at DESC";

                var products = await connection.QueryAsync<ProductResponseDto>(query);

                _logger.LogInformation("Retrieved {Count} products", products.Count());
                return products;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving products");
                throw;
            }
        }

        public async Task<ProductResponseDto?> GetByIdAsync(Guid id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var query = @"
                    SELECT id, codigo, descricao, departamento, preco, status, created_at, updated_at
                    FROM products 
                    WHERE id = @Id AND is_deleted = false";

                var product = await connection.QueryFirstOrDefaultAsync<ProductResponseDto>(query, new { Id = id });

                if (product != null)
                    _logger.LogInformation("Product found with ID: {Id}", id);
                else
                    _logger.LogWarning("Product not found with ID: {Id}", id);

                return product;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {Id}", id);
                throw;
            }
        }

        public async Task<ProductResponseDto> CreateAsync(CreateProductDto createProductDto)
        {
            try
            {
                // Verificar se o código já existe
                if (await CodigoExistsAsync(createProductDto.Codigo))
                {
                    throw new InvalidOperationException(
                        $"Já existe um produto com o código '{createProductDto.Codigo}'");
                }

                using var connection = await _connectionFactory.CreateConnectionAsync();

                var id = Guid.NewGuid();
                var now = DateTime.UtcNow;

                var query = @"
                    INSERT INTO products (id, codigo, descricao, departamento, preco, status, created_at, updated_at)
                    VALUES (@Id, @Codigo, @Descricao, @Departamento, @Preco, @Status, @CreatedAt, @UpdatedAt)";

                await connection.ExecuteAsync(query, new
                {
                    Id = id,
                    createProductDto.Codigo,
                    createProductDto.Descricao,
                    createProductDto.Departamento,
                    createProductDto.Preco,
                    createProductDto.Status,
                    CreatedAt = now,
                    UpdatedAt = now
                });

                _logger.LogInformation("Product created with ID: {Id}, Code: {Codigo}", id, createProductDto.Codigo);

                return new ProductResponseDto
                {
                    Id = id,
                    Codigo = createProductDto.Codigo,
                    Descricao = createProductDto.Descricao,
                    Departamento = createProductDto.Departamento,
                    Preco = createProductDto.Preco,
                    Status = createProductDto.Status,
                    CreatedAt = now,
                    UpdatedAt = now
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product with code: {Codigo}", createProductDto.Codigo);
                throw;
            }
        }

        public async Task<ProductResponseDto?> UpdateAsync(Guid id, UpdateProductDto updateProductDto)
        {
            try
            {
                // Verificar se o produto existe
                if (!await ExistsAsync(id))
                {
                    _logger.LogWarning("Attempt to update non-existent product with ID: {Id}", id);
                    return null;
                }

                // Verificar se o código já existe em outro produto
                if (await CodigoExistsAsync(updateProductDto.Codigo, id))
                {
                    throw new InvalidOperationException(
                        $"Já existe outro produto com o código '{updateProductDto.Codigo}'");
                }

                using var connection = await _connectionFactory.CreateConnectionAsync();

                var now = DateTime.UtcNow;

                var query = @"
                    UPDATE products 
                    SET codigo = @Codigo, 
                        descricao = @Descricao, 
                        departamento = @Departamento, 
                        preco = @Preco, 
                        status = @Status, 
                        updated_at = @UpdatedAt
                    WHERE id = @Id AND is_deleted = false";

                var affectedRows = await connection.ExecuteAsync(query, new
                {
                    Id = id,
                    updateProductDto.Codigo,
                    updateProductDto.Descricao,
                    updateProductDto.Departamento,
                    updateProductDto.Preco,
                    updateProductDto.Status,
                    UpdatedAt = now
                });

                if (affectedRows == 0)
                {
                    _logger.LogWarning("No rows affected when updating product with ID: {Id}", id);
                    return null;
                }

                _logger.LogInformation("Product updated with ID: {Id}", id);
                return await GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var query = @"
                    UPDATE products 
                    SET is_deleted = true, updated_at = @UpdatedAt
                    WHERE id = @Id AND is_deleted = false";

                var affectedRows = await connection.ExecuteAsync(query, new
                {
                    Id = id,
                    UpdatedAt = DateTime.UtcNow
                });

                var deleted = affectedRows > 0;

                if (deleted)
                    _logger.LogInformation("Product deleted (logical) with ID: {Id}", id);
                else
                    _logger.LogWarning("Attempt to delete non-existent product with ID: {Id}", id);

                return deleted;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                var query = "SELECT COUNT(1) FROM products WHERE id = @Id AND is_deleted = false";
                var count = await connection.QuerySingleAsync<int>(query, new { Id = id });

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if product exists with ID: {Id}", id);
                throw;
            }
        }

        public async Task<bool> CodigoExistsAsync(string codigo, Guid? excludeId = null)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();

                string query;
                object parameters;

                if (excludeId.HasValue)
                {
                    query =
                        "SELECT COUNT(1) FROM products WHERE codigo = @Codigo AND id != @ExcludeId AND is_deleted = false";
                    parameters = new { Codigo = codigo, ExcludeId = excludeId.Value };
                }
                else
                {
                    query = "SELECT COUNT(1) FROM products WHERE codigo = @Codigo AND is_deleted = false";
                    parameters = new { Codigo = codigo };
                }

                var count = await connection.QuerySingleAsync<int>(query, parameters);

                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if codigo exists: {Codigo}", codigo);
                throw;
            }
        }
    }
}