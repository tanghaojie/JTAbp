using Abp.Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.TestService
{
    public interface ITestAppService : IApplicationService, IAsyncCrudAppService<TestDto>
    {
    }
}
