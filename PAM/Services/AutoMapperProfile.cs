using AutoMapper;
using PAM.Models;

namespace PAM.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Form, Form>().ForMember(l => l.FormId, opt => opt.Ignore());
            CreateMap<Location, Location>().ForMember(l => l.LocationId, opt => opt.Ignore());
            CreateMap<Bureau, Bureau>().ForMember(b => b.BureauId, opt => opt.Ignore());
            CreateMap<Models.System, Models.System>().ForMember(s => s.SystemId, opt => opt.Ignore());
            CreateMap<Employee, Employee>().ForMember(e => e.EmployeeId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
            CreateMap<Employee, EmployeeBindingModel>();
            CreateMap<EmployeeBindingModel, Employee>()
                .ForMember(e => e.EmployeeId, opt => opt.Ignore())
                .ForMember(e => e.Name, opt => opt.Ignore())
                .ForMember(e => e.Email, opt => opt.Ignore());
            CreateMap<Employee, Requester>();
            CreateMap<Requester, Requester>().ForMember(r => r.RequesterId, opt => opt.Ignore())
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null)); ;
            CreateMap<Request, Request>().ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
        }
    }
}
