using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Data;
using ProductAPI.DTOs;
using ProductAPI.Services;
using System.Data;
using Xunit;

namespace ProductAPI.Tests.Services
{
    public class ProductServiceTests
    {
        private readonly Mock<IDbConnectionFactory> _mockConnectionFactory;
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly Mock<ILogger<ProductService>> _mockLogger;
        private readonly ProductService _productService;

        public ProductServiceTests()
        {
            _mockConnectionFactory = new Mock<IDbConnectionFactory>();
            _mockConnection = new Mock<IDbConnection>();
            _mockLogger = new Mock<ILogger<ProductService>>();
            
            _mockConnectionFactory.Setup(x => x.CreateConnectionAsync())
                .ReturnsAsync(_mockConnection.Object);
            
            _productService = new ProductService(_mockConnectionFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public async Task CreateAsync_ValidProduct_ShouldReturnProductResponseDto()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Codigo = "PROD001",
                Descricao = "Produto Teste",
                Departamento = "010",
                Preco = 10.50m,
                Status = true
            };

            // Act
            var result = await _productService.CreateAsync(createProductDto);

            // Assert
            result.Should().NotBeNull();
            result.Codigo.Should().Be(createProductDto.Codigo);
            result.Descricao.Should().Be(createProductDto.Descricao);
            result.Departamento.Should().Be(createProductDto.Departamento);
            result.Preco.Should().Be(createProductDto.Preco);
            result.Status.Should().Be(createProductDto.Status);
            result.Id.Should().NotBe(Guid.Empty);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        [InlineData("   ")]
        public async Task CreateAsync_InvalidCodigo_ShouldThrowException(string invalidCodigo)
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Codigo = invalidCodigo,
                Descricao = "Produto Teste",
                Departamento = "010",
                Preco = 10.50m,
                Status = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateAsync(createProductDto));
        }

        [Fact]
        public async Task CreateAsync_NegativePrice_ShouldThrowException()
        {
            // Arrange
            var createProductDto = new CreateProductDto
            {
                Codigo = "PROD001",
                Descricao = "Produto Teste",
                Departamento = "010",
                Preco = -10.50m,
                Status = true
            };

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => _productService.CreateAsync(createProductDto));
        }

        [Fact]
        public void ProductResponseDto_Properties_ShouldBeSetCorrectly()
        {
            // Arrange
            var id = Guid.NewGuid();
            var now = DateTime.UtcNow;

            // Act
            var product = new ProductResponseDto
            {
                Id = id,
                Codigo = "PROD001",
                Descricao = "Produto Teste",
                Departamento = "010",
                Preco = 15.75m,
                Status = true,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Assert
            product.Id.Should().Be(id);
            product.Codigo.Should().Be("PROD001");
            product.Descricao.Should().Be("Produto Teste");
            product.Departamento.Should().Be("010");
            product.Preco.Should().Be(15.75m);
            product.Status.Should().BeTrue();
            product.CreatedAt.Should().Be(now);
            product.UpdatedAt.Should().Be(now);
        }
    }
}