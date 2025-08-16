using Dapper;
using ProductAPI.Data;
using ProductAPI.Models;

namespace ProductAPI.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDbConnectionFactory _connectionFactory;
        private readonly ILogger<DepartmentService> _logger;

        public DepartmentService(IDbConnectionFactory connectionFactory, ILogger<DepartmentService> logger)
        {
            _connectionFactory = connectionFactory;
            _logger = logger;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var query = "SELECT codigo, descricao FROM departments ORDER BY codigo";
                
                var departments = await connection.QueryAsync<Department>(query);
                
                _logger.LogInformation("Retrieved {Count} departments", departments.Count());
                return departments;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                throw;
            }
        }

        public async Task<Department?> GetByCodigoAsync(string codigo)
        {
            try
            {
                _logger.LogInformation("=== DEBUG: GetByCodigoAsync called ===");
                _logger.LogInformation("Received codigo: '{Codigo}' (Length: {Length})", codigo, codigo?.Length ?? 0);
        
                using var connection = await _connectionFactory.CreateConnectionAsync();
        
                var query = "SELECT codigo, descricao FROM departments WHERE codigo = @Codigo";
                _logger.LogInformation("Executing query: {Query} with parameter: @Codigo = '{Codigo}'", query, codigo);
        
                var department = await connection.QueryAsync<Department>(query, new { Codigo = codigo });
                var departmentList = department.ToList();
        
                _logger.LogInformation("Query returned {Count} results", departmentList.Count);
        
                if (departmentList.Any())
                {
                    var first = departmentList.First();
                    _logger.LogInformation("Found department: Codigo='{Codigo}', Descricao='{Descricao}'", first.Codigo, first.Descricao);
                    return first;
                }
                else
                {
                    _logger.LogWarning("No department found with codigo: '{Codigo}'", codigo);
            
                    // Debug: vamos ver todos os departamentos
                    var allDepts = await connection.QueryAsync<Department>("SELECT codigo, descricao FROM departments");
                    _logger.LogInformation("All departments in database:");
                    foreach (var dept in allDepts)
                    {
                        _logger.LogInformation("  - '{Codigo}' = '{Descricao}'", dept.Codigo, dept.Descricao);
                    }
            
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving department with code: {Codigo}", codigo);
                throw;
            }
        }

        public async Task<bool> ExistsAsync(string codigo)
        {
            try
            {
                using var connection = await _connectionFactory.CreateConnectionAsync();
                
                var query = "SELECT COUNT(1) FROM departments WHERE codigo = @Codigo";
                var count = await connection.QuerySingleAsync<int>(query, new { Codigo = codigo });
                
                return count > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error checking if department exists with code: {Codigo}", codigo);
                throw;
            }
        }
    }
}