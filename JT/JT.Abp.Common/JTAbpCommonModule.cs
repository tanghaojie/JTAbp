using Abp.Dependency;
using Abp.Modules;
using Abp.MultiTenancy;
using Abp.Reflection.Extensions;
using JT.Abp.Common.Tenancy;
using JT.Abp.Configuration;
using JT.Abp.Configuration.Setting;
using JT.Abp.MultiTenancy;
using Abp.Configuration.Startup;
using JT.Abp.Application.Features;
using Castle.MicroKernel.Registration;
using Abp.Reflection;
using System.Linq;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using System.Reflection;

namespace JT.Abp
{
    public class JTAbpCommonModule : AbpModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IJTEntityTypes, JTEntityTypes>();

            IocManager.Register<IRoleManagementConfig, RoleManagementConfig>();
            IocManager.Register<IUserManagementConfig, UserManagementConfig>();
            IocManager.Register<IJTConfig, JTConfig>();

            Configuration.ReplaceService<ITenantStore, JTTenantStore>(DependencyLifeStyle.Transient);

            Configuration.Settings.Providers.Add<JTSettingProvider>();
        }

        public override void Initialize()
        {
            FillMissingEntityTypes();
            //ReplaceTenantStore();
            IocManager.RegisterAssemblyByConvention(typeof(JTAbpCommonModule).GetAssembly());

            RegisterTenantCache();
        }

        private void FillMissingEntityTypes()
        {
            using (var entityTypes = IocManager.ResolveAsDisposable<IJTEntityTypes>())
            {
                if (entityTypes.Object.User != null &&
                    entityTypes.Object.Role != null &&
                    entityTypes.Object.Tenant != null)
                {
                    return;
                }

                using (var typeFinder = IocManager.ResolveAsDisposable<ITypeFinder>())
                {
                    var types = typeFinder.Object.FindAll();
                    entityTypes.Object.Tenant = types.FirstOrDefault(t => typeof(JTTenantBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
                    entityTypes.Object.Role = types.FirstOrDefault(t => typeof(JTRoleBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
                    entityTypes.Object.User = types.FirstOrDefault(t => typeof(JTUserBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
                }
            }
        }

        private void RegisterTenantCache()
        {
            if (IocManager.IsRegistered<IJTTenantCache>())
            {
                return;
            }

            using (var entityTypes = IocManager.ResolveAsDisposable<IJTEntityTypes>())
            {
                var implType = typeof(JTTenantCache<,>).MakeGenericType(entityTypes.Object.Tenant, entityTypes.Object.User);

                IocManager.Register(typeof(IJTTenantCache), implType, DependencyLifeStyle.Transient);
            }
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
