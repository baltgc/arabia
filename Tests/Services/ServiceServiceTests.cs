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
public class ServiceServiceTests
{
    private Mock<IRepository<Service>> _mockRepository = null!;
    private Mock<IMapper> _mockMapper = null!;
    private ServiceService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IRepository<Service>>();
        _mockMapper = new Mock<IMapper>();
        _service = new ServiceService(_mockRepository.Object, _mockMapper.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldReturnServiceResponse()
    {
        // Arrange
        var request = new CreateServiceRequest
        {
            Name = "Electric",
            Description = "Electrical services",
            BasePrice = 100.00m,
        };

        var service = new Service
        {
            Id = 1,
            Name = request.Name,
            Description = request.Description,
            BasePrice = request.BasePrice,
        };

        var response = new ServiceResponse
        {
            Id = 1,
            Name = service.Name,
            Description = service.Description,
            BasePrice = service.BasePrice,
        };

        _mockMapper.Setup(m => m.Map<Service>(request)).Returns(service);
        _mockRepository.Setup(r => r.AddAsync(service)).ReturnsAsync(service);
        _mockMapper.Setup(m => m.Map<ServiceResponse>(service)).Returns(response);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Name.Should().Be("Electric");
        _mockMapper.Verify(m => m.Map<Service>(request), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Service>()), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenServiceExists_ShouldReturnServiceResponse()
    {
        // Arrange
        var service = new Service { Id = 1, Name = "Electric" };
        var response = new ServiceResponse { Id = 1, Name = "Electric" };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(service);
        _mockMapper.Setup(m => m.Map<ServiceResponse>(service)).Returns(response);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenServiceNotExists_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Service?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllServices()
    {
        // Arrange
        var services = new List<Service>
        {
            new Service { Id = 1, Name = "Electric" },
            new Service { Id = 2, Name = "Plumbing" },
        };

        var responses = new List<ServiceResponse>
        {
            new ServiceResponse { Id = 1, Name = "Electric" },
            new ServiceResponse { Id = 2, Name = "Plumbing" },
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(services);
        _mockMapper.Setup(m => m.Map<IEnumerable<ServiceResponse>>(services)).Returns(responses);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetActiveAsync_ShouldReturnActiveServices()
    {
        // Arrange
        var services = new List<Service>
        {
            new Service
            {
                Id = 1,
                Name = "Electric",
                IsActive = true,
            },
        };

        var responses = new List<ServiceResponse>
        {
            new ServiceResponse
            {
                Id = 1,
                Name = "Electric",
                IsActive = true,
            },
        };

        _mockRepository
            .Setup(r =>
                r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Service, bool>>>())
            )
            .ReturnsAsync(services);
        _mockMapper.Setup(m => m.Map<IEnumerable<ServiceResponse>>(services)).Returns(responses);

        // Act
        var result = await _service.GetActiveAsync();

        // Assert
        result.Should().HaveCount(1);
        _mockRepository.Verify(
            r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Service, bool>>>()),
            Times.Once
        );
    }

    [Test]
    public async Task UpdateAsync_WhenServiceExists_ShouldUpdateAndReturnResponse()
    {
        // Arrange
        var service = new Service
        {
            Id = 1,
            Name = "Electric",
            BasePrice = 100m,
        };
        var request = new UpdateServiceRequest { BasePrice = 120m };
        var response = new ServiceResponse
        {
            Id = 1,
            Name = "Electric",
            BasePrice = 120m,
        };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(service);
        _mockMapper.Setup(m => m.Map(request, service));
        _mockRepository.Setup(r => r.UpdateAsync(service)).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<ServiceResponse>(service)).Returns(response);

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result!.BasePrice.Should().Be(120m);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(service), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenServiceNotExists_ShouldReturnNull()
    {
        // Arrange
        var request = new UpdateServiceRequest { BasePrice = 120m };
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Service?)null);

        // Act
        var result = await _service.UpdateAsync(999, request);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Service>()), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_WhenServiceExists_ShouldReturnTrue()
    {
        // Arrange
        var service = new Service { Id = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(service);
        _mockRepository.Setup(r => r.DeleteAsync(service)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(service), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenServiceNotExists_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Service?)null);

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Service>()), Times.Never);
    }
}
