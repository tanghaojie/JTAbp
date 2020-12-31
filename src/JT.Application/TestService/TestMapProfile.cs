using AutoMapper;
using JT.Entities;

namespace JT.TestService
{
    public class TestMapProfile : Profile
    {
        public TestMapProfile()
        {
            CreateMap<Test, TestDto>();
            CreateMap<TestDto, Test>();
        }
    }
}
