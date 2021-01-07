using Microsoft.AspNetCore.Identity;
using System;

namespace JT.Abp.Identity
{
    public class JTIdentityBuilder : IdentityBuilder
    {
        public Type TenantType { get; }

        public JTIdentityBuilder(IdentityBuilder identityBuilder, Type tenantType)
            : base(identityBuilder.UserType, identityBuilder.RoleType, identityBuilder.Services)
        {
            TenantType = tenantType;
        }
    }
}
