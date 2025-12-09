using arabia.DTOs.Requests;
using arabia.DTOs.Responses;
using arabia.Infrastructure.Persistence.Repositories;
using arabia.Models;
using arabia.Services.Interfaces;
using AutoMapper;

namespace arabia.Services;

public class ServiceService : IServiceService
{
    private readonly IRepository<Service> _repository;
    private readonly IMapper _mapper;

    public ServiceService(IRepository<Service> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ServiceResponse> CreateAsync(CreateServiceRequest request)
    {
        var service = _mapper.Map<Service>(request);

        service.CreatedAt = DateTime.UtcNow;

        var created = await _repository.AddAsync(service);

        return _mapper.Map<ServiceResponse>(created);
    }

    public async Task<ServiceResponse?> GetByIdAsync(int id)
    {
        var service = await _repository.GetByIdAsync(id);

        return service == null ? null : _mapper.Map<ServiceResponse>(service);
    }

    public async Task<IEnumerable<ServiceResponse>> GetAllAsync()
    {
        var services = await _repository.GetAllAsync();

        return _mapper.Map<IEnumerable<ServiceResponse>>(services);
    }

    public async Task<IEnumerable<ServiceResponse>> GetActiveAsync()
    {
        var services = await _repository.FindAsync(s => s.IsActive);

        return _mapper.Map<IEnumerable<ServiceResponse>>(services);
    }

    public async Task<ServiceResponse?> UpdateAsync(int id, UpdateServiceRequest request)
    {
        var service = await _repository.GetByIdAsync(id);

        if (service == null)
            return null;

        _mapper.Map(request, service);

        service.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(service);

        return _mapper.Map<ServiceResponse>(service);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var service = await _repository.GetByIdAsync(id);

        if (service == null)
            return false;

        await _repository.DeleteAsync(service);

        return true;
    }
}
