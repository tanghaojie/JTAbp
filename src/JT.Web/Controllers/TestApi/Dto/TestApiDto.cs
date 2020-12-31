using Abp.Application.Services.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JT.Web.Controllers.TestApi
{
    public class TestApiDto : EntityDto
    {
        public string Name { get; set; }

        public string XX { get; set; }
    }
}
