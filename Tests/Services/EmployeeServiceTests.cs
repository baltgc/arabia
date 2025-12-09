using arabia.DTOs.Requests;
using arabia.DTOs.Responses;
using arabia.Infrastructure.Persistence.Repositories;
using arabia.Models;
using arabia.Services;
using AutoMapper;
using FluentAssertions;
using Moq;
using NUnit.Framework;

namespace arabia.Tests.Services;

[TestFixture]
public class EmployeeServiceTests
{
    private Mock<IRepository<Employee>> _mockRepository = null!;
    private Mock<IMapper> _mockMapper = null!;
    private EmployeeService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IRepository<Employee>>();
        _mockMapper = new Mock<IMapper>();
        _service = new EmployeeService(_mockRepository.Object, _mockMapper.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldReturnEmployeeResponse()
    {
        // Arrange
        var request = new CreateEmployeeRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Phone = "555-0100",
            Specialization = "Electric",
            HireDate = DateTime.UtcNow,
        };

        var employee = new Employee
        {
            Id = 1,
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            Phone = request.Phone,
            Specialization = request.Specialization,
            HireDate = request.HireDate,
        };

        var response = new EmployeeResponse
        {
            Id = 1,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            Email = employee.Email,
            Phone = employee.Phone,
            Specialization = employee.Specialization,
            HireDate = employee.HireDate,
        };

        _mockMapper.Setup(m => m.Map<Employee>(request)).Returns(employee);
        _mockRepository.Setup(r => r.AddAsync(employee)).ReturnsAsync(employee);
        _mockMapper.Setup(m => m.Map<EmployeeResponse>(employee)).Returns(response);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.FirstName.Should().Be("John");
        _mockMapper.Verify(m => m.Map<Employee>(request), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Employee>()), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenEmployeeExists_ShouldReturnEmployeeResponse()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
        };
        var response = new EmployeeResponse
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _mockMapper.Setup(m => m.Map<EmployeeResponse>(employee)).Returns(response);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenEmployeeNotExists_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockMapper.Verify(m => m.Map<EmployeeResponse>(It.IsAny<Employee>()), Times.Never);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee { Id = 1, FirstName = "John" },
            new Employee { Id = 2, FirstName = "Jane" },
        };

        var responses = new List<EmployeeResponse>
        {
            new EmployeeResponse { Id = 1, FirstName = "John" },
            new EmployeeResponse { Id = 2, FirstName = "Jane" },
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(employees);
        _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeResponse>>(employees)).Returns(responses);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetBySpecializationAsync_ShouldReturnFilteredEmployees()
    {
        // Arrange
        var employees = new List<Employee>
        {
            new Employee
            {
                Id = 1,
                Specialization = "Electric",
                IsActive = true,
            },
        };

        var responses = new List<EmployeeResponse>
        {
            new EmployeeResponse { Id = 1, Specialization = "Electric" },
        };

        _mockRepository
            .Setup(r =>
                r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>())
            )
            .ReturnsAsync(employees);
        _mockMapper.Setup(m => m.Map<IEnumerable<EmployeeResponse>>(employees)).Returns(responses);

        // Act
        var result = await _service.GetBySpecializationAsync("Electric");

        // Assert
        result.Should().HaveCount(1);
        _mockRepository.Verify(
            r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Employee, bool>>>()),
            Times.Once
        );
    }

    [Test]
    public async Task UpdateAsync_WhenEmployeeExists_ShouldUpdateAndReturnResponse()
    {
        // Arrange
        var employee = new Employee
        {
            Id = 1,
            FirstName = "John",
            LastName = "Doe",
        };
        var request = new UpdateEmployeeRequest { FirstName = "Jane" };
        var response = new EmployeeResponse
        {
            Id = 1,
            FirstName = "Jane",
            LastName = "Doe",
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _mockMapper.Setup(m => m.Map(request, employee));
        _mockRepository.Setup(r => r.UpdateAsync(employee)).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<EmployeeResponse>(employee)).Returns(response);

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result!.FirstName.Should().Be("Jane");
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(employee), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenEmployeeNotExists_ShouldReturnNull()
    {
        // Arrange
        var request = new UpdateEmployeeRequest { FirstName = "Jane" };
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.UpdateAsync(999, request);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Employee>()), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_WhenEmployeeExists_ShouldReturnTrue()
    {
        // Arrange
        var employee = new Employee { Id = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(employee);
        _mockRepository.Setup(r => r.DeleteAsync(employee)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(employee), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenEmployeeNotExists_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Employee?)null);

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Employee>()), Times.Never);
    }
}
