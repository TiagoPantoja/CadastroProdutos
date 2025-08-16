using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using ProductAPI.Data;
using ProductAPI.Models;
using ProductAPI.Services;
using System.Data;
using Xunit;

namespace ProductAPI.Tests.Services
{
    public class DepartmentServiceTests
    {
        private readonly Mock<IDbConnectionFactory> _mockConnectionFactory;
        private readonly Mock<IDbConnection> _mockConnection;
        private readonly Mock<ILogger<DepartmentService>> _mockLogger;
        private readonly DepartmentService _departmentService;

        public DepartmentServiceTests()
        {
            _mockConnectionFactory = new Mock<IDbConnectionFactory>();
            _mockConnection = new Mock<IDbConnection>();
            _mockLogger = new Mock<ILogger<DepartmentService>>();
            
            _mockConnectionFactory.Setup(x => x.CreateConnectionAsync())
                .ReturnsAsync(_mockConnection.Object);
            
            _departmentService = new DepartmentService(_mockConnectionFactory.Object, _mockLogger.Object);
        }

        [Fact]
        public void Department_Properties_ShouldBeSetCorrectly()
        {
            // Arrange & Act
            var department = new Department
            {
                Codigo = "010",
                Descricao = "BEBIDAS"
            };

            // Assert
            department.Codigo.Should().Be("010");
            department.Descricao.Should().Be("BEBIDAS");
        }

        [Theory]
        [InlineData("010", "BEBIDAS")]
        [InlineData("020", "CONGELADOS")]
        [InlineData("030", "LATICINIOS")]
        [InlineData("040", "VEGETAIS")]
        public void Department_ValidDepartments_ShouldHaveCorrectValues(string codigo, string descricao)
        {
            // Arrange & Act
            var department = new Department
            {
                Codigo = codigo,
                Descricao = descricao
            };

            // Assert
            department.Codigo.Should().Be(codigo);
            department.Descricao.Should().Be(descricao);
        }
    }
}