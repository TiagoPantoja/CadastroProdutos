using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Controllers;
using ProductAPI.DTOs;
using ProductAPI.Services;
using Xunit;

namespace ProductAPI.Tests.Controllers
{
    public class ProductsControllerTests
    {
        private readonly Mock<IProductService> _mockProductService;
        private readonly Mock<IDepartmentService> _mockDepartmentService;
        private readonly Mock<ILogger<ProductsController>> _mockLogger;
        private readonly ProductsController _controller;

        public ProductsControllerTests()
        {
            _mockProductService = new Mock<IProductService>();
            _mockDepartmentService = new Mock<IDepartmentService>();
            _mockLogger = new Mock<ILogger<ProductsController>>();
            
            _controller = new ProductsController(
                _mockProductService.Object, 
                _mockDepartmentService.Object, 
                _mockLogger.Object);
        }

        [Fact]
        public async Task GetAll_ShouldReturnOkWithProducts()
        {
            // Arrange
            var products = new List<ProductResponseDto>
            {
                new() { Id = Guid.NewGuid(), Codigo = "PROD001", Descricao = "Produto 1", Departamento = "010", Preco = 10.50m, Status = true },
                new() { Id = Guid.NewGuid(), Codigo = "PROD002", Descricao = "Produto 2", Departamento = "020", Preco = 20.75m, Status = true }
            };

            _mockProductService.Setup(x => x.GetAllAsync()).ReturnsAsync(products);

            // Act
            var result = await _controller.GetAll();

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedProducts = okResult.Value.Should().BeAssignableTo<IEnumerable<ProductResponseDto>>().Subject;
            returnedProducts.Should().HaveCount(2);
        }

        [Fact]
        public async Task GetById_ExistingId_ShouldReturnOkWithProduct()
        {
            // Arrange
            var productId = Guid.NewGuid();
            var product = new ProductResponseDto 
            { 
                Id = productId, 
                Codigo = "PROD001", 
                Descricao = "Produto Teste", 
                Departamento = "010", 
                Preco = 15.50m, 
                Status = true 
            };

            _mockProductService.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync(product);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            var okResult = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var returnedProduct = okResult.Value.Should().BeOfType<ProductResponseDto>().Subject;
            returnedProduct.Id.Should().Be(productId);
        }

        [Fact]
        public async Task GetById_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductService.Setup(x => x.GetByIdAsync(productId)).ReturnsAsync((ProductResponseDto?)null);

            // Act
            var result = await _controller.GetById(productId);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        [Fact]
        public async Task GetById_EmptyGuid_ShouldReturnBadRequest()
        {
            // Act
            var result = await _controller.GetById(Guid.Empty);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        [Fact]
        public async Task Create_ValidProduct_ShouldReturnCreated()
        {
            // Arrange
            var createDto = new CreateProductDto
            {
                Codigo = "PROD001",
                Descricao = "Produto Teste",
                Departamento = "010",
                Preco = 25.50m,
                Status = true
            };

            var createdProduct = new ProductResponseDto
            {
                Id = Guid.NewGuid(),
                Codigo = createDto.Codigo,
                Descricao = createDto.Descricao,
                Departamento = createDto.Departamento,
                Preco = createDto.Preco,
                Status = createDto.Status,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            _mockDepartmentService.Setup(x => x.ExistsAsync(createDto.Departamento)).ReturnsAsync(true);
            _mockProductService.Setup(x => x.CreateAsync(createDto)).ReturnsAsync(createdProduct);

            // Act
            var result = await _controller.Create(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            var returnedProduct = createdResult.Value.Should().BeOfType<ProductResponseDto>().Subject;
            returnedProduct.Codigo.Should().Be(createDto.Codigo);
        }

        [Fact]
        public async Task Delete_ExistingId_ShouldReturnNoContent()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductService.Setup(x => x.DeleteAsync(productId)).ReturnsAsync(true);

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        [Fact]
        public async Task Delete_NonExistingId_ShouldReturnNotFound()
        {
            // Arrange
            var productId = Guid.NewGuid();
            _mockProductService.Setup(x => x.DeleteAsync(productId)).ReturnsAsync(false);

            // Act
            var result = await _controller.Delete(productId);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}