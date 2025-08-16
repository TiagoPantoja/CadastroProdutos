using Microsoft.AspNetCore.Mvc;
using ProductAPI.DTOs;
using ProductAPI.Services;
using System.ComponentModel.DataAnnotations;

namespace ProductAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Produces("application/json")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IDepartmentService _departmentService;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(
            IProductService productService, 
            IDepartmentService departmentService,
            ILogger<ProductsController> logger)
        {
            _productService = productService;
            _departmentService = departmentService;
            _logger = logger;
        }
        
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<ProductResponseDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<IEnumerable<ProductResponseDto>>> GetAll()
        {
            try
            {
                var products = await _productService.GetAllAsync();
                return Ok(products);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all products");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
        
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductResponseDto>> GetById(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "ID não pode ser vazio" });
                }

                var product = await _productService.GetByIdAsync(id);
                
                if (product == null)
                {
                    return NotFound(new { message = "Produto não encontrado" });
                }

                return Ok(product);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving product with ID: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
        
        [HttpPost]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductResponseDto>> Create([FromBody] CreateProductDto createProductDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar se o departamento existe
                if (!await _departmentService.ExistsAsync(createProductDto.Departamento))
                {
                    return BadRequest(new { message = "Departamento não encontrado" });
                }

                var product = await _productService.CreateAsync(createProductDto);
                
                return CreatedAtAction(
                    nameof(GetById), 
                    new { id = product.Id }, 
                    product);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict when creating product");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating product");
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
        
        [HttpPut("{id:guid}")]
        [ProducesResponseType(typeof(ProductResponseDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<ProductResponseDto>> Update(Guid id, [FromBody] UpdateProductDto updateProductDto)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "ID não pode ser vazio" });
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Validar se o departamento existe
                if (!await _departmentService.ExistsAsync(updateProductDto.Departamento))
                {
                    return BadRequest(new { message = "Departamento não encontrado" });
                }

                var product = await _productService.UpdateAsync(id, updateProductDto);
                
                if (product == null)
                {
                    return NotFound(new { message = "Produto não encontrado" });
                }

                return Ok(product);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Conflict when updating product");
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating product with ID: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
        
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                if (id == Guid.Empty)
                {
                    return BadRequest(new { message = "ID não pode ser vazio" });
                }

                var deleted = await _productService.DeleteAsync(id);
                
                if (!deleted)
                {
                    return NotFound(new { message = "Produto não encontrado" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting product with ID: {Id}", id);
                return StatusCode(500, new { message = "Erro interno do servidor" });
            }
        }
    }
}