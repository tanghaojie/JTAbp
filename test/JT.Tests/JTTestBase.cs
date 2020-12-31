using System;
using System.Threading.Tasks;
using Abp.TestBase;
using JT.EntityFrameworkCore;
using JT.Tests.TestDatas;

namespace JT.Tests
{
    public class JTTestBase : AbpIntegratedTestBase<JTTestModule>
    {
        public JTTestBase()
        {
            UsingDbContext(context => new TestDataBuilder(context).Build());
        }

        protected virtual void UsingDbContext(Action<JTDbContext> action)
        {
            using (var context = LocalIocManager.Resolve<JTDbContext>())
            {
                action(context);
                context.SaveChanges();
            }
        }

        protected virtual T UsingDbContext<T>(Func<JTDbContext, T> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<JTDbContext>())
            {
                result = func(context);
                context.SaveChanges();
            }

            return result;
        }

        protected virtual async Task UsingDbContextAsync(Func<JTDbContext, Task> action)
        {
            using (var context = LocalIocManager.Resolve<JTDbContext>())
            {
                await action(context);
                await context.SaveChangesAsync(true);
            }
        }

        protected virtual async Task<T> UsingDbContextAsync<T>(Func<JTDbContext, Task<T>> func)
        {
            T result;

            using (var context = LocalIocManager.Resolve<JTDbContext>())
            {
                result = await func(context);
                context.SaveChanges();
            }

            return result;
        }
    }
}
