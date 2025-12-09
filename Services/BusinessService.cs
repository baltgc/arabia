using arabia.DTOs.Requests;
using arabia.DTOs.Responses;
using arabia.Infrastructure.Persistence.Repositories;
using arabia.Models;
using arabia.Services.Interfaces;
using AutoMapper;

namespace arabia.Services;

public class BusinessService : IBusinessService
{
    private readonly IRepository<Business> _repository;
    private readonly IMapper _mapper;

    public BusinessService(IRepository<Business> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BusinessResponse> CreateAsync(CreateBusinessRequest request)
    {
        var business = _mapper.Map<Business>(request);
        business.CreatedAt = DateTime.UtcNow;
        var created = await _repository.AddAsync(business);
        return _mapper.Map<BusinessResponse>(created);
    }

    public async Task<BusinessResponse?> GetByIdAsync(int id)
    {
        var business = await _repository.GetByIdAsync(id);

        return business == null ? null : _mapper.Map<BusinessResponse>(business);
    }

    public async Task<IEnumerable<BusinessResponse>> GetAllAsync()
    {
        var businesses = await _repository.GetAllAsync();

        return _mapper.Map<IEnumerable<BusinessResponse>>(businesses);
    }

    public async Task<IEnumerable<BusinessResponse>> GetActiveAsync()
    {
        var businesses = await _repository.FindAsync(b => b.IsActive);

        return _mapper.Map<IEnumerable<BusinessResponse>>(businesses);
    }

    public async Task<BusinessResponse?> UpdateAsync(int id, UpdateBusinessRequest request)
    {
        var business = await _repository.GetByIdAsync(id);

        if (business == null)
            return null;

        _mapper.Map(request, business);

        business.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(business);

        return _mapper.Map<BusinessResponse>(business);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var business = await _repository.GetByIdAsync(id);

        if (business == null)
            return false;

        await _repository.DeleteAsync(business);

        return true;
    }
}
