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
public class BusinessServiceTests
{
    private Mock<IRepository<Business>> _mockRepository = null!;
    private Mock<IMapper> _mockMapper = null!;
    private BusinessService _service = null!;

    [SetUp]
    public void Setup()
    {
        _mockRepository = new Mock<IRepository<Business>>();
        _mockMapper = new Mock<IMapper>();
        _service = new BusinessService(_mockRepository.Object, _mockMapper.Object);
    }

    [Test]
    public async Task CreateAsync_ShouldReturnBusinessResponse()
    {
        // Arrange
        var request = new CreateBusinessRequest
        {
            Name = "ABC Corp",
            Address = "123 Main St",
            City = "New York",
            State = "NY",
            ZipCode = "10001",
            ContactEmail = "contact@abc.com",
            ContactPhone = "555-0100",
            ContactPerson = "John Doe",
        };

        var business = new Business
        {
            Id = 1,
            Name = request.Name,
            Address = request.Address,
            City = request.City,
            State = request.State,
            ZipCode = request.ZipCode,
            ContactEmail = request.ContactEmail,
            ContactPhone = request.ContactPhone,
            ContactPerson = request.ContactPerson,
        };

        var response = new BusinessResponse { Id = 1, Name = business.Name };

        _mockMapper.Setup(m => m.Map<Business>(request)).Returns(business);
        _mockRepository.Setup(r => r.AddAsync(business)).ReturnsAsync(business);
        _mockMapper.Setup(m => m.Map<BusinessResponse>(business)).Returns(response);

        // Act
        var result = await _service.CreateAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(1);
        _mockMapper.Verify(m => m.Map<Business>(request), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<Business>()), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenBusinessExists_ShouldReturnBusinessResponse()
    {
        // Arrange
        var business = new Business { Id = 1, Name = "ABC Corp" };
        var response = new BusinessResponse { Id = 1, Name = "ABC Corp" };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(business);
        _mockMapper.Setup(m => m.Map<BusinessResponse>(business)).Returns(response);

        // Act
        var result = await _service.GetByIdAsync(1);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(1);
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WhenBusinessNotExists_ShouldReturnNull()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Business?)null);

        // Act
        var result = await _service.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
    }

    [Test]
    public async Task GetAllAsync_ShouldReturnAllBusinesses()
    {
        // Arrange
        var businesses = new List<Business>
        {
            new Business { Id = 1, Name = "ABC Corp" },
            new Business { Id = 2, Name = "XYZ Inc" },
        };

        var responses = new List<BusinessResponse>
        {
            new BusinessResponse { Id = 1, Name = "ABC Corp" },
            new BusinessResponse { Id = 2, Name = "XYZ Inc" },
        };

        _mockRepository.Setup(r => r.GetAllAsync()).ReturnsAsync(businesses);
        _mockMapper.Setup(m => m.Map<IEnumerable<BusinessResponse>>(businesses)).Returns(responses);

        // Act
        var result = await _service.GetAllAsync();

        // Assert
        result.Should().HaveCount(2);
        _mockRepository.Verify(r => r.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task GetActiveAsync_ShouldReturnActiveBusinesses()
    {
        // Arrange
        var businesses = new List<Business>
        {
            new Business
            {
                Id = 1,
                Name = "ABC Corp",
                IsActive = true,
            },
        };

        var responses = new List<BusinessResponse>
        {
            new BusinessResponse
            {
                Id = 1,
                Name = "ABC Corp",
                IsActive = true,
            },
        };

        _mockRepository
            .Setup(r =>
                r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Business, bool>>>())
            )
            .ReturnsAsync(businesses);
        _mockMapper.Setup(m => m.Map<IEnumerable<BusinessResponse>>(businesses)).Returns(responses);

        // Act
        var result = await _service.GetActiveAsync();

        // Assert
        result.Should().HaveCount(1);
        _mockRepository.Verify(
            r => r.FindAsync(It.IsAny<System.Linq.Expressions.Expression<Func<Business, bool>>>()),
            Times.Once
        );
    }

    [Test]
    public async Task UpdateAsync_WhenBusinessExists_ShouldUpdateAndReturnResponse()
    {
        // Arrange
        var business = new Business { Id = 1, Name = "ABC Corp" };
        var request = new UpdateBusinessRequest { Name = "ABC Corporation" };
        var response = new BusinessResponse { Id = 1, Name = "ABC Corporation" };

        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(business);
        _mockMapper.Setup(m => m.Map(request, business));
        _mockRepository.Setup(r => r.UpdateAsync(business)).Returns(Task.CompletedTask);
        _mockMapper.Setup(m => m.Map<BusinessResponse>(business)).Returns(response);

        // Act
        var result = await _service.UpdateAsync(1, request);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("ABC Corporation");
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(business), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WhenBusinessNotExists_ShouldReturnNull()
    {
        // Arrange
        var request = new UpdateBusinessRequest { Name = "New Name" };
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Business?)null);

        // Act
        var result = await _service.UpdateAsync(999, request);

        // Assert
        result.Should().BeNull();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(r => r.UpdateAsync(It.IsAny<Business>()), Times.Never);
    }

    [Test]
    public async Task DeleteAsync_WhenBusinessExists_ShouldReturnTrue()
    {
        // Arrange
        var business = new Business { Id = 1 };
        _mockRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(business);
        _mockRepository.Setup(r => r.DeleteAsync(business)).Returns(Task.CompletedTask);

        // Act
        var result = await _service.DeleteAsync(1);

        // Assert
        result.Should().BeTrue();
        _mockRepository.Verify(r => r.GetByIdAsync(1), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(business), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WhenBusinessNotExists_ShouldReturnFalse()
    {
        // Arrange
        _mockRepository.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((Business?)null);

        // Act
        var result = await _service.DeleteAsync(999);

        // Assert
        result.Should().BeFalse();
        _mockRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<Business>()), Times.Never);
    }
}
