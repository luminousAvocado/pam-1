using AutoMapper;
using PAM.Models;

namespace PAM.Services
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Employee, Employee>();
            CreateMap<Employee, Requester>();
        }
    }
}
