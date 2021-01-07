using Abp.Authorization;
using JT.Abp.Application.Editions;
using JT.Abp.Application.Features;
using JT.Abp.Authorization;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using JT.Abp.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace JT.Abp.Identity
{
    public static class JTIdentityBuilderExtensions
    {
        public static JTIdentityBuilder AddJTTenantManager<TTenantManager>(this JTIdentityBuilder builder)
           where TTenantManager : class
        {
            var type = typeof(TTenantManager);
            var jtType = typeof(JTTenantManager<,>).MakeGenericType(builder.TenantType, builder.UserType);
            builder.Services.AddScoped(type, provider => provider.GetRequiredService(jtType));
            builder.Services.AddScoped(jtType, type);
            return builder;
        }

        public static JTIdentityBuilder AddJTEditionManager<TEditionManager>(this JTIdentityBuilder builder)
            where TEditionManager : class
        {
            var type = typeof(TEditionManager);
            var jtType = typeof(JTEditionManager);
            builder.Services.AddScoped(type, provider => provider.GetRequiredService(jtType));
            builder.Services.AddScoped(jtType, type);
            return builder;
        }

        public static JTIdentityBuilder AddJTRoleManager<TRoleManager>(this JTIdentityBuilder builder)
            where TRoleManager : class
        {
            var jtType = typeof(JTRoleManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var managerType = typeof(RoleManager<>).MakeGenericType(builder.RoleType);
            builder.Services.AddScoped(jtType, services => services.GetRequiredService(managerType));
            builder.AddRoleManager<TRoleManager>();
            return builder;
        }

        public static JTIdentityBuilder AddJTUserManager<TUserManager>(this JTIdentityBuilder builder)
            where TUserManager : class
        {
            var jtType = typeof(JTUserManager<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var managerType = typeof(UserManager<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(jtType, services => services.GetRequiredService(managerType));
            builder.AddUserManager<TUserManager>();
            return builder;
        }

        public static JTIdentityBuilder AddJTSignInManager<TSignInManager>(this JTIdentityBuilder builder)
            where TSignInManager : class
        {
            var jtType = typeof(JTSignInManager<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType);
            var managerType = typeof(SignInManager<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(jtType, services => services.GetRequiredService(managerType));
            builder.AddSignInManager<TSignInManager>();
            return builder;
        }

        public static JTIdentityBuilder AddJTLogInManager<TLogInManager>(this JTIdentityBuilder builder)
            where TLogInManager : class
        {
            var type = typeof(TLogInManager);
            var jtType = typeof(JTLogInManager<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType);
            builder.Services.AddScoped(type, provider => provider.GetService(jtType));
            builder.Services.AddScoped(jtType, type);
            return builder;
        }

        public static JTIdentityBuilder AddJTUserClaimsPrincipalFactory<TUserClaimsPrincipalFactory>(this JTIdentityBuilder builder)
            where TUserClaimsPrincipalFactory : class
        {
            var type = typeof(TUserClaimsPrincipalFactory);
            builder.Services.AddScoped(typeof(UserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(JTUserClaimsPrincipalFactory<,>).MakeGenericType(builder.UserType, builder.RoleType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(IUserClaimsPrincipalFactory<>).MakeGenericType(builder.UserType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(type);
            return builder;
        }

        public static JTIdentityBuilder AddJTSecurityStampValidator<TSecurityStampValidator>(this JTIdentityBuilder builder)
            where TSecurityStampValidator : class, ISecurityStampValidator
        {
            var type = typeof(TSecurityStampValidator);
            builder.Services.AddScoped(typeof(SecurityStampValidator<>).MakeGenericType(builder.UserType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(JTSecurityStampValidator<,,>).MakeGenericType(builder.TenantType, builder.RoleType, builder.UserType), services => services.GetRequiredService(type));
            builder.Services.AddScoped(typeof(ISecurityStampValidator), services => services.GetRequiredService(type));
            builder.Services.AddScoped(type);
            return builder;
        }

        public static JTIdentityBuilder AddJTPermissionChecker<TPermissionChecker>(this JTIdentityBuilder builder)
            where TPermissionChecker : class
        {
            var type = typeof(TPermissionChecker);
            var checkerType = typeof(JTPermissionChecker<,>).MakeGenericType(builder.RoleType, builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(checkerType, provider => provider.GetService(type));
            builder.Services.AddScoped(typeof(IPermissionChecker), provider => provider.GetService(type));
            return builder;
        }

        public static JTIdentityBuilder AddJTUserStore<TUserStore>(this JTIdentityBuilder builder)
            where TUserStore : class
        {
            var type = typeof(TUserStore);
            var abpStoreType = typeof(JTUserStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var storeType = typeof(IUserStore<>).MakeGenericType(builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(abpStoreType, services => services.GetRequiredService(type));
            builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
            return builder;
        }

        public static JTIdentityBuilder AddJTRoleStore<TRoleStore>(this JTIdentityBuilder builder)
            where TRoleStore : class
        {
            var type = typeof(TRoleStore);
            var abpStoreType = typeof(JTRoleStore<,>).MakeGenericType(builder.RoleType, builder.UserType);
            var storeType = typeof(IRoleStore<>).MakeGenericType(builder.RoleType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(abpStoreType, services => services.GetRequiredService(type));
            builder.Services.AddScoped(storeType, services => services.GetRequiredService(type));
            return builder;
        }

        public static JTIdentityBuilder AddFeatureValueStore<TFeatureValueStore>(this JTIdentityBuilder builder)
            where TFeatureValueStore : class
        {
            var type = typeof(TFeatureValueStore);
            var storeType = typeof(JTFeatureValueStore<,>).MakeGenericType(builder.TenantType, builder.UserType);
            builder.Services.AddScoped(type);
            builder.Services.AddScoped(storeType, provider => provider.GetService(type));
            builder.Services.AddScoped(typeof(IJTFeatureValueStore), provider => provider.GetService(type));
            return builder;
        }
    }
}
