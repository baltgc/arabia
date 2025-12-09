using arabia.DTOs.Requests;
using arabia.DTOs.Responses;

namespace arabia.Services.Interfaces;

public interface IEmployeeService
{
    Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request);

    Task<EmployeeResponse?> GetByIdAsync(int id);

    Task<IEnumerable<EmployeeResponse>> GetAllAsync();

    Task<IEnumerable<EmployeeResponse>> GetBySpecializationAsync(string specialization);

    Task<EmployeeResponse?> UpdateAsync(int id, UpdateEmployeeRequest request);

    Task<bool> DeleteAsync(int id);
}
