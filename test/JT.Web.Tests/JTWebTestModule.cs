using Abp.AspNetCore.TestBase;
using Abp.Modules;
using Abp.Reflection.Extensions;
using JT.Web.Startup;
namespace JT.Web.Tests
{
    [DependsOn(
        typeof(JTWebModule),
        typeof(AbpAspNetCoreTestBaseModule)
        )]
    public class JTWebTestModule : AbpModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.IsTransactional = false; //EF Core InMemory DB does not support transactions.
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(JTWebTestModule).GetAssembly());
        }
    }
}