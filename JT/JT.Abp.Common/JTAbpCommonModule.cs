using Abp.Dependency;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using JT.Abp.Common.Tenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Common
{
    public class JTAbpCommonModule : AbpModule
    {
        public override void PreInitialize()
        {
        }

        public override void Initialize()
        {
            ReplaceTenantStore();
            IocManager.RegisterAssemblyByConvention(typeof(JTAbpCommonModule).GetAssembly());
        }

        public override void PostInitialize()
        {
        }

        private void ReplaceTenantStore()
        {
            if (Configuration.MultiTenancy.IsEnabled)
            {
                IocManager.Register<ITenantStore, TenantStore>(DependencyLifeStyle.Transient);
            }
            else
            {
                IocManager.Register<ITenantStore, NoneTenantStore>(DependencyLifeStyle.Singleton);
            }
        }
    }
}
