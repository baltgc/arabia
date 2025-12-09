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
public class ServiceRequestServiceTests
{
    private Mock<IServiceRequestRepository> _mockRepository = null!;
    private Mock<IRepository<Business>> _mockBusinessRepository = null!;
    private Mock<IRepository<Service>> _mockServiceRepository = null!;
    private Mock<IRepository<Employee>> _mockEmployeeRepository = null!;
    private Mock<IMapper> _mockMapper = null!;
    private ServiceRequestService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IServiceRequestRepository>();
        _mockBusinessRepository = new Mock<IRepository<Business>>();
        _mockServiceRepository = new Mock<IRepository<Service>>();
        _mockEmployeeRepository = new Mock<IRepository<Employee>>();
        _mockMapper = new Mock<IMapper>();
        _service = new ServiceRequestService(
            _mockRepository.Object,
            _mockBusinessRepository.Object,
            _mockServiceRepository.Object,
            _mockEmployeeRepository.Object,
            _mockMapper.Object
        );
    }

    [Test]
    public async Task CreateAsync_WhenBusinessAndServiceExist_ShouldReturnServiceRequestResponse()
    {
        // Arrange
        var request = new CreateServiceRequestRequest
        {
            BusinessId = 1,
            ServiceId = 1,
            RequestedDate = DateTime.UtcNow,
            Description = "Fix outlet",
        };

        var serviceRequest = new ServiceRequest
        {
            Id = 1,
            BusinessId = request.BusinessId,
            ServiceId = request.ServiceId,
            RequestedDate = request.RequestedDate,
            Description = request.Description,
            Status = "Pending",
        };

        var response = new ServiceRequestResponse
        {
            Id = 1,
            BusinessId = 1,
            ServiceId = 1,
            Status = "Pending",
        };

        _mockBusinessRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockServiceRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockMapper.Setup(m => m.Map<ServiceRequest>(request)).Returns(serviceRequest);
        _mockRepository.Setup(r => r.AddAsync(serviceRequest)).ReturnsAsync(serviceRequest);
        _mockMapper.Setup(m => m.Map<ServiceRequestResponse>(serviceRequest)).Returns(response);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        result.Status.Should().Be("Pending");
        _mockBusinessRepository.Verify(r => r.ExistsAsync(1), Times.Once);
        _mockServiceRepository.Verify(r => r.ExistsAsync(1), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<ServiceRequest>()), Times.Once);
    }

    [Test]
    public void CreateAsync_WhenBusinessNotExists_ShouldThrowException()
    {
        // Arrange
        var request = new CreateServiceRequestRequest { BusinessId = 999, ServiceId = 1 };
        _mockBusinessRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        AsyncTestDelegate act = async () => await _service.CreateAsync(request);
        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.That(exception, Is.Not.Null);
        _mockBusinessRepository.Verify(r => r.ExistsAsync(999), Times.Once);
        _mockServiceRepository.Verify(r => r.ExistsAsync(It.IsAny<int>()), Times.Never);
    }

    [Test]
    public void CreateAsync_WhenServiceNotExists_ShouldThrowException()
    {
        // Arrange
        var request = new CreateServiceRequestRequest { BusinessId = 1, ServiceId = 999 };
        _mockBusinessRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockServiceRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        AsyncTestDelegate act = async () => await _service.CreateAsync(request);
        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.That(exception, Is.Not.Null);
        _mockBusinessRepository.Verify(r => r.ExistsAsync(1), Times.Once);
        _mockServiceRepository.Verify(r => r.ExistsAsync(999), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenServiceRequestExists_ShouldReturnResponse()
    {
        // Arrange
        var serviceRequest = new ServiceRequest { Id = 1, Status = "Pending" };
        var response = new ServiceRequestResponse { Id = 1, Status = "Pending" };

        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(serviceRequest);
        _mockMapper.Setup(m => m.Map<ServiceRequestResponse>(serviceRequest)).Returns(response);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        _mockRepository.Verify(r => r.GetByIdWithDetailsAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenServiceRequestNotExists_ShouldReturnNull()
    {
        // Arrange
        _mockRepository
            .Setup(r => r.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((ServiceRequest?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdWithDetailsAsync(999), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllServiceRequests()
    {
        // Arrange
        var serviceRequests = new List<ServiceRequest>
        {
            new ServiceRequest { Id = 1 },
            new ServiceRequest { Id = 2 },
        };

        var responses = new List<ServiceRequestResponse>
        {
            new ServiceRequestResponse { Id = 1 },
            new ServiceRequestResponse { Id = 2 },
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(serviceRequests);
        _mockMapper
            .Setup(m => m.Map<IEnumerable<ServiceRequestResponse>>(serviceRequests))
            .Returns(responses);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetByBusinessIdAsync_ShouldReturnFilteredServiceRequests()
    {
        // Arrange
        var serviceRequests = new List<ServiceRequest>
        {
            new ServiceRequest { Id = 1, BusinessId = 1 },
        };
        var responses = new List<ServiceRequestResponse>
        {
            new ServiceRequestResponse { Id = 1, BusinessId = 1 },
        };

        _mockRepository.Setup(r => r.GetByBusinessIdAsync(1)).ReturnsAsync(serviceRequests);
        _mockMapper
            .Setup(m => m.Map<IEnumerable<ServiceRequestResponse>>(serviceRequests))
            .Returns(responses);

        // Act
        var result = await _service.GetByBusinessIdAsync(1);

        // Assert
        result.Should().HaveCount(1);
        _mockRepository.Verify(r => r.GetByBusinessIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByEmployeeIdAsync_ShouldReturnFilteredServiceRequests()
    {
        // Arrange
        var serviceRequests = new List<ServiceRequest>
        {
            new ServiceRequest { Id = 1, EmployeeId = 1 },
        };
        var responses = new List<ServiceRequestResponse>
        {
            new ServiceRequestResponse { Id = 1, EmployeeId = 1 },
        };

        _mockRepository.Setup(r => r.GetByEmployeeIdAsync(1)).ReturnsAsync(serviceRequests);
        _mockMapper
            .Setup(m => m.Map<IEnumerable<ServiceRequestResponse>>(serviceRequests))
            .Returns(responses);

        // Act
        var result = await _service.GetByEmployeeIdAsync(1);

        // Assert
        result.Should().HaveCount(1);
        _mockRepository.Verify(r => r.GetByEmployeeIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByStatusAsync_ShouldReturnFilteredServiceRequests()
    {
        // Arrange
        var serviceRequests = new List<ServiceRequest>
        {
            new ServiceRequest { Id = 1, Status = "Pending" },
        };
        var responses = new List<ServiceRequestResponse>
        {
            new ServiceRequestResponse { Id = 1, Status = "Pending" },
        };

        _mockRepository.Setup(r => r.GetByStatusAsync("Pending")).ReturnsAsync(serviceRequests);
        _mockMapper
            .Setup(m => m.Map<IEnumerable<ServiceRequestResponse>>(serviceRequests))
            .Returns(responses);

        // Act
        var result = await _service.GetByStatusAsync("Pending");

        // Assert
        result.Should().HaveCount(1);
        _mockRepository.Verify(r => r.GetByStatusAsync("Pending"), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenServiceRequestExists_ShouldUpdateAndReturnResponse()
    {
        // Arrange
        var serviceRequest = new ServiceRequest { Id = 1, Status = "Pending" };
        var request = new UpdateServiceRequestRequest { Status = "Assigned", EmployeeId = 1 };
        var response = new ServiceRequestResponse { Id = 1, Status = "Assigned" };

        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(serviceRequest);
        _mockEmployeeRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockMapper.Setup(m => m.Map(request, serviceRequest));
        _mockRepository.Setup(r => r.UpdateAsync(serviceRequest)).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<ServiceRequestResponse>(serviceRequest)).Returns(response);

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result!.Status.Should().Be("Assigned");
        _mockRepository.Verify(r => r.GetByIdWithDetailsAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(serviceRequest), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenServiceRequestNotExists_ShouldReturnNull()
    {
        // Arrange
        var request = new UpdateServiceRequestRequest { Status = "Assigned" };
        _mockRepository
            .Setup(r => r.GetByIdWithDetailsAsync(999))
            .ReturnsAsync((ServiceRequest?)null);

        // Act
        var result = await _service.UpdateAsync(999, request);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdWithDetailsAsync(999), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<ServiceRequest>()), Times.Never);
    }

    [Test]
    public void UpdateAsync_WhenEmployeeNotExists_ShouldThrowException()
    {
        // Arrange
        var serviceRequest = new ServiceRequest { Id = 1 };
        var request = new UpdateServiceRequestRequest { EmployeeId = 999 };
        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(serviceRequest);
        _mockEmployeeRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // Act & Assert
        AsyncTestDelegate act = async () => await _service.UpdateAsync(1, request);
        InvalidOperationException exception = Assert.ThrowsAsync<InvalidOperationException>(act);
        Assert.That(exception, Is.Not.Null);
        _mockEmployeeRepository.Verify(r => r.ExistsAsync(999), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenCompletedDateProvided_ShouldSetStatusToCompleted()
    {
        // Arrange
        var serviceRequest = new ServiceRequest { Id = 1, Status = "InProgress" };
        var request = new UpdateServiceRequestRequest { CompletedDate = DateTime.UtcNow };
        var response = new ServiceRequestResponse { Id = 1, Status = "Completed" };

        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(serviceRequest);
        _mockMapper.Setup(m => m.Map(request, serviceRequest));
        _mockRepository.Setup(r => r.UpdateAsync(serviceRequest)).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<ServiceRequestResponse>(serviceRequest)).Returns(response);

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        serviceRequest.Status.Should().Be("Completed");
    }

    [Test]
    public async Task UpdateAsync_WhenEmployeeAssignedToPending_ShouldSetStatusToAssigned()
    {
        // Arrange
        var serviceRequest = new ServiceRequest { Id = 1, Status = "Pending" };
        var request = new UpdateServiceRequestRequest { EmployeeId = 1 };
        var response = new ServiceRequestResponse { Id = 1, Status = "Assigned" };

        _mockRepository.Setup(r => r.GetByIdWithDetailsAsync(1)).ReturnsAsync(serviceRequest);
        _mockEmployeeRepository.Setup(r => r.ExistsAsync(1)).ReturnsAsync(true);
        _mockMapper.Setup(m => m.Map(request, serviceRequest));
        _mockRepository.Setup(r => r.UpdateAsync(serviceRequest)).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<ServiceRequestResponse>(serviceRequest)).Returns(response);

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        serviceRequest.Status.Should().Be("Assigned");
    }

    [Test]
    public async Task DeleteAsync_WhenServiceRequestExists_ShouldReturnTrue()
    {
        // Arrange
        var serviceRequest = new ServiceRequest { Id = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(serviceRequest);
        _mockRepository.Setup(r => r.DeleteAsync(serviceRequest)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(serviceRequest), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenServiceRequestNotExists_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((ServiceRequest?)null);

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<ServiceRequest>()), Times.Never);
    }
}
