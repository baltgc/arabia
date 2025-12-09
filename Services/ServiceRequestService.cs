using arabia.DTOs.Requests;
using arabia.DTOs.Responses;
using arabia.Infrastructure.Persistence.Repositories;
using arabia.Models;
using arabia.Services.Interfaces;
using AutoMapper;

namespace arabia.Services;

public class ServiceRequestService : IServiceRequestService
{
    private readonly IServiceRequestRepository _repository;
    private readonly IRepository<Business> _businessRepository;
    private readonly IRepository<Service> _serviceRepository;
    private readonly IRepository<Employee> _employeeRepository;
    private readonly IMapper _mapper;

    public ServiceRequestService(
        IServiceRequestRepository repository,
        IRepository<Business> businessRepository,
        IRepository<Service> serviceRepository,
        IRepository<Employee> employeeRepository,
        IMapper mapper
    )
    {
        _repository = repository;
        _businessRepository = businessRepository;
        _serviceRepository = serviceRepository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<ServiceRequestResponse> CreateAsync(CreateServiceRequestRequest request)
    {
        // Validate business exists
        if (!await _businessRepository.ExistsAsync(request.BusinessId))
            throw new InvalidOperationException(
                $"Business with ID {request.BusinessId} not found."
            );

        // Validate service exists
        if (!await _serviceRepository.ExistsAsync(request.ServiceId))
            throw new InvalidOperationException($"Service with ID {request.ServiceId} not found.");

        var serviceRequest = _mapper.Map<ServiceRequest>(request);

        serviceRequest.Status = "Pending";

        serviceRequest.CreatedAt = DateTime.UtcNow;

        var created = await _repository.AddAsync(serviceRequest);

        return _mapper.Map<ServiceRequestResponse>(created);
    }

    public async Task<ServiceRequestResponse?> GetByIdAsync(int id)
    {
        var serviceRequest = await _repository.GetByIdWithDetailsAsync(id);

        return serviceRequest == null ? null : _mapper.Map<ServiceRequestResponse>(serviceRequest);
    }

    public async Task<IEnumerable<ServiceRequestResponse>> GetAllAsync()
    {
        var serviceRequests = await _repository.GetAllAsync();

        return _mapper.Map<IEnumerable<ServiceRequestResponse>>(serviceRequests);
    }

    public async Task<IEnumerable<ServiceRequestResponse>> GetByBusinessIdAsync(int businessId)
    {
        var serviceRequests = await _repository.GetByBusinessIdAsync(businessId);

        return _mapper.Map<IEnumerable<ServiceRequestResponse>>(serviceRequests);
    }

    public async Task<IEnumerable<ServiceRequestResponse>> GetByEmployeeIdAsync(int employeeId)
    {
        var serviceRequests = await _repository.GetByEmployeeIdAsync(employeeId);

        return _mapper.Map<IEnumerable<ServiceRequestResponse>>(serviceRequests);
    }

    public async Task<IEnumerable<ServiceRequestResponse>> GetByStatusAsync(string status)
    {
        var serviceRequests = await _repository.GetByStatusAsync(status);

        return _mapper.Map<IEnumerable<ServiceRequestResponse>>(serviceRequests);
    }

    public async Task<ServiceRequestResponse?> UpdateAsync(
        int id,
        UpdateServiceRequestRequest request
    )
    {
        var serviceRequest = await _repository.GetByIdWithDetailsAsync(id);
        if (serviceRequest == null)
            return null;

        // Validate employee if provided
        if (
            request.EmployeeId.HasValue
            && !await _employeeRepository.ExistsAsync(request.EmployeeId.Value)
        )
            throw new InvalidOperationException(
                $"Employee with ID {request.EmployeeId.Value} not found."
            );

        _mapper.Map(request, serviceRequest);

        serviceRequest.UpdatedAt = DateTime.UtcNow;

        // Auto-update status based on completion
        if (request.CompletedDate.HasValue && serviceRequest.Status != "Completed")
        {
            serviceRequest.Status = "Completed";
        }
        else if (request.EmployeeId.HasValue && serviceRequest.Status == "Pending")
        {
            serviceRequest.Status = "Assigned";
        }

        await _repository.UpdateAsync(serviceRequest);

        return _mapper.Map<ServiceRequestResponse>(serviceRequest);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var serviceRequest = await _repository.GetByIdAsync(id);

        if (serviceRequest == null)
            return false;

        await _repository.DeleteAsync(serviceRequest);

        return true;
    }
}
