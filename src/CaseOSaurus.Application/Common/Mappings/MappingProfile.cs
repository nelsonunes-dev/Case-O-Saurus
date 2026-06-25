using AutoMapper;
using CaseOSaurus.Application.DTO;
using CaseOSaurus.Domain.Entities;

namespace CaseOSaurus.Application.Common.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<UserCase, CaseResponse>();
    }
}
