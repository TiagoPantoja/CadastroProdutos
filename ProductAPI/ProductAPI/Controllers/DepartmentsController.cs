using Microsoft.AspNetCore.Mvc;
using ProductAPI.Models;
using ProductAPI.Services;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class DepartmentsController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<DepartmentsController> _logger;

        public DepartmentsController(IDepartmentService departmentService, ILogger<DepartmentsController> logger)
        {
            _departmentService = departmentService;
            _logger = logger;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Department>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<Department>>> GetAll()
        {
            try
            {
                var departments = await _departmentService.GetAllAsync();
                return Ok(departments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving departments");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
        
        [HttpGet("{codigo}")]
        public async Task<ActionResult<Department>> GetByCodigo(string codigo)
        {
            try
            {
                _logger.LogInformation("=== CONTROLLER: GetByCodigo called ===");
                _logger.LogInformation("URL parameter 'codigo': '{Codigo}' (Type: {Type}, Length: {Length})", 
                    codigo, codigo?.GetType().Name, codigo?.Length ?? 0);
        
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    _logger.LogWarning("Codigo is null or whitespace");
                    return BadRequest(new { message = "Código não pode ser vazio" });
                }

                var department = await _departmentService.GetByCodigoAsync(codigo);
        
                if (department == null)
                {
                    _logger.LogWarning("Service returned null for codigo: '{Codigo}'", codigo);
                    return NotFound(new { message = "Departamento não encontrado" });
                }

                _logger.LogInformation("Returning department: {Codigo} - {Descricao}", department.Codigo, department.Descricao);
                return Ok(department);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByCodigo with codigo: {Codigo}", codigo);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        } 
    }
}