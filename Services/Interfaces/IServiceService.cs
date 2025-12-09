using arabia.DTOs.Requests;
using arabia.DTOs.Responses;

namespace arabia.Services.Interfaces;

public interface IServiceService
{
    Task<ServiceResponse> CreateAsync(CreateServiceRequest request);

    Task<ServiceResponse?> GetByIdAsync(int id);

    Task<IEnumerable<ServiceResponse>> GetAllAsync();

    Task<IEnumerable<ServiceResponse>> GetActiveAsync();

    Task<ServiceResponse?> UpdateAsync(int id, UpdateServiceRequest request);

    Task<bool> DeleteAsync(int id);
}
