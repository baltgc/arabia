using arabia.DTOs.Requests;
using arabia.DTOs.Responses;

namespace arabia.Services.Interfaces;

public interface IBusinessService
{
    Task<BusinessResponse> CreateAsync(CreateBusinessRequest request);

    Task<BusinessResponse?> GetByIdAsync(int id);

    Task<IEnumerable<BusinessResponse>> GetAllAsync();

    Task<IEnumerable<BusinessResponse>> GetActiveAsync();

    Task<BusinessResponse?> UpdateAsync(int id, UpdateBusinessRequest request);

    Task<bool> DeleteAsync(int id);
}
