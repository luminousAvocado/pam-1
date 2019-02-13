using AutoMapper;
using PAM.Models;

namespace PAM.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Employee, Employee>().ForMember(e => e.EmployeeId, opt => opt.Ignore());
            CreateMap<Employee, Requester>();
            CreateMap<Requester, Requester>()
                .ForMember(r => r.RequesterId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); ;
            CreateMap<Request, Request>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
