using Abp.Authorization;
using JT.Abp.Application.Editions;
using JT.Abp.Application.Features;
using JT.Abp.Authorization;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using JT.Abp.Configuration;
using JT.Abp.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;

namespace JT.Abp.Identity
{
    public static class JTServiceCollectionExtensions
    {
        public static JTIdentityBuilder AddJTIdentity<TTenant, TUser, TRole>(this IServiceCollection services)
           where TTenant : JTTenant<TUser>
           where TRole : JTRole<TUser>, new()
           where TUser : JTUser<TUser>
        {
            return services.AddJTIdentity<TTenant, TUser, TRole>(setupAction: null);
        }

        public static JTIdentityBuilder AddJTIdentity<TTenant, TUser, TRole>(this IServiceCollection services, Action<IdentityOptions> setupAction)
            where TTenant : JTTenant<TUser>
            where TRole : JTRole<TUser>, new()
            where TUser : JTUser<TUser>
        {
            services.AddSingleton<IJTEntityTypes>(new JTEntityTypes
            {
                Tenant = typeof(TTenant),
                Role = typeof(TRole),
                User = typeof(TUser)
            });

            //JTTenantManager
            services.TryAddScoped<JTTenantManager<TTenant, TUser>>();

            //JTEditionManager
            services.TryAddScoped<JTEditionManager>();

            //JTRoleManager
            services.TryAddScoped<JTRoleManager<TRole, TUser>>();
            services.TryAddScoped(typeof(RoleManager<TRole>), provider => provider.GetService(typeof(JTRoleManager<TRole, TUser>)));

            //JTUserManager
            services.TryAddScoped<JTUserManager<TRole, TUser>>();
            services.TryAddScoped(typeof(UserManager<TUser>), provider => provider.GetService(typeof(JTUserManager<TRole, TUser>)));

            //SignInManager
            services.TryAddScoped<JTSignInManager<TTenant, TRole, TUser>>();
            services.TryAddScoped(typeof(SignInManager<TUser>), provider => provider.GetService(typeof(JTSignInManager<TTenant, TRole, TUser>)));

            //JTLogInManager
            services.TryAddScoped<JTLogInManager<TTenant, TRole, TUser>>();

            //JTUserClaimsPrincipalFactory
            services.TryAddScoped<JTUserClaimsPrincipalFactory<TUser, TRole>>();
            services.TryAddScoped(typeof(UserClaimsPrincipalFactory<TUser, TRole>), provider => provider.GetService(typeof(JTUserClaimsPrincipalFactory<TUser, TRole>)));
            services.TryAddScoped(typeof(IUserClaimsPrincipalFactory<TUser>), provider => provider.GetService(typeof(JTUserClaimsPrincipalFactory<TUser, TRole>)));

            //JTSecurityStampValidator
            services.TryAddScoped<JTSecurityStampValidator<TTenant, TRole, TUser>>();
            services.TryAddScoped(typeof(SecurityStampValidator<TUser>), provider => provider.GetService(typeof(JTSecurityStampValidator<TTenant, TRole, TUser>)));
            services.TryAddScoped(typeof(ISecurityStampValidator), provider => provider.GetService(typeof(JTSecurityStampValidator<TTenant, TRole, TUser>)));

            //PermissionChecker
            services.TryAddScoped<JTPermissionChecker<TRole, TUser>>();
            services.TryAddScoped(typeof(IPermissionChecker), provider => provider.GetService(typeof(JTPermissionChecker<TRole, TUser>)));

            //JTUserStore
            services.TryAddScoped<JTUserStore<TRole, TUser>>();
            services.TryAddScoped(typeof(IUserStore<TUser>), provider => provider.GetService(typeof(JTUserStore<TRole, TUser>)));

            //JTRoleStore
            services.TryAddScoped<JTRoleStore<TRole, TUser>>();
            services.TryAddScoped(typeof(IRoleStore<TRole>), provider => provider.GetService(typeof(JTRoleStore<TRole, TUser>)));

            //JTFeatureValueStore
            services.TryAddScoped<JTFeatureValueStore<TTenant, TUser>>();
            services.TryAddScoped(typeof(IJTFeatureValueStore), provider => provider.GetService(typeof(JTFeatureValueStore<TTenant, TUser>)));

            return new JTIdentityBuilder(services.AddIdentity<TUser, TRole>(setupAction), typeof(TTenant));
        }
    }
}
