using Abp.Domain.Repositories;
using JT.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JT.Web.Controllers.TestApi
{
    [Route("api/services/app/[controller]")]
    [ApiController]
    public class TestApiController : JTControllerBase
    {
        private readonly IRepository<Test> Repository;
        public TestApiController(IRepository<Test> repository)
        {
            Repository = repository;
        }

        [HttpPost]
        public async Task<TestApiDto> CreateAsync([FromBody] TestApiDto input)
        {
            var entity = ObjectMapper.Map<Test>(input);

            await Repository.InsertOrUpdateAsync(entity);
            await CurrentUnitOfWork.SaveChangesAsync();

            return ObjectMapper.Map<TestApiDto>(entity);
        }

        [HttpGet]
        public async Task<TestApiDto> GetAsync([FromQuery] TestApiDto input)
        {
            var entity = await Repository.GetAsync(input.Id);
            return ObjectMapper.Map<TestApiDto>(entity);
        }

    }
}
