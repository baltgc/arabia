using arabia.Models;

namespace arabia.Infrastructure.Persistence.Repositories;

public interface IServiceRequestRepository : IRepository<ServiceRequest>
{
    Task<IEnumerable<ServiceRequest>> GetByBusinessIdAsync(int businessId);

    Task<IEnumerable<ServiceRequest>> GetByEmployeeIdAsync(int employeeId);

    Task<IEnumerable<ServiceRequest>> GetByStatusAsync(string status);

    Task<ServiceRequest?> GetByIdWithDetailsAsync(int id);
}
