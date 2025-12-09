using arabia.Infrastructure.Persistence;
using arabia.Models;
using Microsoft.EntityFrameworkCore;

namespace arabia.Infrastructure.Persistence.Repositories;

public class ServiceRequestRepository : Repository<ServiceRequest>, IServiceRequestRepository
{
    public ServiceRequestRepository(ApplicationDbContext context)
        : base(context) { }

    public async Task<IEnumerable<ServiceRequest>> GetByBusinessIdAsync(int businessId)
    {
        return await _dbSet
            .Include(sr => sr.Business)
            .Include(sr => sr.Service)
            .Include(sr => sr.Employee)
            .Where(sr => sr.BusinessId == businessId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceRequest>> GetByEmployeeIdAsync(int employeeId)
    {
        return await _dbSet
            .Include(sr => sr.Business)
            .Include(sr => sr.Service)
            .Include(sr => sr.Employee)
            .Where(sr => sr.EmployeeId == employeeId)
            .ToListAsync();
    }

    public async Task<IEnumerable<ServiceRequest>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Include(sr => sr.Business)
            .Include(sr => sr.Service)
            .Include(sr => sr.Employee)
            .Where(sr => sr.Status == status)
            .ToListAsync();
    }

    public async Task<ServiceRequest?> GetByIdWithDetailsAsync(int id)
    {
        return await _dbSet
            .Include(sr => sr.Business)
            .Include(sr => sr.Service)
            .Include(sr => sr.Employee)
            .FirstOrDefaultAsync(sr => sr.Id == id);
    }

    public override async Task<ServiceRequest?> GetByIdAsync(int id)
    {
        return await GetByIdWithDetailsAsync(id);
    }

    public override async Task<IEnumerable<ServiceRequest>> GetAllAsync()
    {
        return await _dbSet
            .Include(sr => sr.Business)
            .Include(sr => sr.Service)
            .Include(sr => sr.Employee)
            .ToListAsync();
    }
}
