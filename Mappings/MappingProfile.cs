using arabia.DTOs.Requests;
using arabia.DTOs.Responses;
using arabia.Models;
using AutoMapper;

namespace arabia.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Employee mappings
        CreateMap<CreateEmployeeRequest, Employee>();
        CreateMap<UpdateEmployeeRequest, Employee>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Employee, EmployeeResponse>();

        // Business mappings
        CreateMap<CreateBusinessRequest, Business>();
        CreateMap<UpdateBusinessRequest, Business>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Business, BusinessResponse>();

        // Service mappings
        CreateMap<CreateServiceRequest, Service>();
        CreateMap<UpdateServiceRequest, Service>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Service, ServiceResponse>();

        // ServiceRequest mappings
        CreateMap<CreateServiceRequestRequest, Models.ServiceRequest>();
        CreateMap<UpdateServiceRequestRequest, Models.ServiceRequest>()
            .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        CreateMap<Models.ServiceRequest, ServiceRequestResponse>()
            .ForMember(dest => dest.BusinessName, opt => opt.MapFrom(src => src.Business.Name))
            .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.Service.Name))
            .ForMember(
                dest => dest.EmployeeName,
                opt =>
                    opt.MapFrom(src =>
                        src.Employee != null
                            ? $"{src.Employee.FirstName} {src.Employee.LastName}"
                            : null
                    )
            );

        // User mappings
        CreateMap<User, UserResponse>();
    }
}
