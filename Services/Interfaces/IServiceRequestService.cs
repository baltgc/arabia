using arabia.DTOs.Requests;
using arabia.DTOs.Responses;

namespace arabia.Services.Interfaces;

public interface IServiceRequestService
{
    Task<ServiceRequestResponse> CreateAsync(CreateServiceRequestRequest request);

    Task<ServiceRequestResponse?> GetByIdAsync(int id);

    Task<IEnumerable<ServiceRequestResponse>> GetAllAsync();

    Task<IEnumerable<ServiceRequestResponse>> GetByBusinessIdAsync(int businessId);

    Task<IEnumerable<ServiceRequestResponse>> GetByEmployeeIdAsync(int employeeId);

    Task<IEnumerable<ServiceRequestResponse>> GetByStatusAsync(string status);

    Task<ServiceRequestResponse?> UpdateAsync(int id, UpdateServiceRequestRequest request);

    Task<bool> DeleteAsync(int id);
}
