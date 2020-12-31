using AutoMapper;
using JT.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JT.Web.Controllers.TestApi
{
    public class TestApiMapProfile : Profile
    {
        public TestApiMapProfile()
        {
            CreateMap<Test, TestApiDto>();
            CreateMap<TestApiDto, Test>();
        }
    }
}
