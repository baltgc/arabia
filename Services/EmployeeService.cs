using arabia.DTOs.Requests;
using arabia.DTOs.Responses;
using arabia.Infrastructure.Persistence.Repositories;
using arabia.Models;
using arabia.Services.Interfaces;
using AutoMapper;

namespace arabia.Services;

public class EmployeeService : IEmployeeService
{
    private readonly IRepository<Employee> _repository;
    private readonly IMapper _mapper;

    public EmployeeService(IRepository<Employee> repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EmployeeResponse> CreateAsync(CreateEmployeeRequest request)
    {
        var employee = _mapper.Map<Employee>(request);

        employee.CreatedAt = DateTime.UtcNow;

        var created = await _repository.AddAsync(employee);

        return _mapper.Map<EmployeeResponse>(created);
    }

    public async Task<EmployeeResponse?> GetByIdAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id);

        return employee == null ? null : _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<IEnumerable<EmployeeResponse>> GetAllAsync()
    {
        var employees = await _repository.GetAllAsync();

        return _mapper.Map<IEnumerable<EmployeeResponse>>(employees);
    }

    public async Task<IEnumerable<EmployeeResponse>> GetBySpecializationAsync(string specialization)
    {
        var employees = await _repository.FindAsync(e =>
            e.Specialization == specialization && e.IsActive
        );

        return _mapper.Map<IEnumerable<EmployeeResponse>>(employees);
    }

    public async Task<EmployeeResponse?> UpdateAsync(int id, UpdateEmployeeRequest request)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
            return null;

        _mapper.Map(request, employee);

        employee.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(employee);

        return _mapper.Map<EmployeeResponse>(employee);
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var employee = await _repository.GetByIdAsync(id);

        if (employee == null)
            return false;

        await _repository.DeleteAsync(employee);

        return true;
    }
}
