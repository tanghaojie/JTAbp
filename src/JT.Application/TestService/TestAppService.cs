using Abp.Application.Services;
using Abp.Application.Services.Dto;
using Abp.Domain.Repositories;
using JT.Entities;
using System.Linq;

namespace JT.TestService
{
    public class TestAppService : AsyncCrudAppService<Test, TestDto>, ITestAppService
    {
        public TestAppService(IRepository<Test> repository) : base(repository)
        {
           
        }

        protected override IQueryable<Test> CreateFilteredQuery(PagedAndSortedResultRequestDto input)
        {
            return base.CreateFilteredQuery(input);
        }

    }
}
