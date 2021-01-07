using Abp;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using JT.Abp.MultiTenancy;
using System;

namespace JT.Abp.Configuration
{
    public class JTEntityTypes : IJTEntityTypes
    {
        public Type User {
            get { return _user; }
            set {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(JTUserBase).IsAssignableFrom(value))
                {
                    throw new AbpException(value.AssemblyQualifiedName + " should be derived from " + typeof(JTUserBase).AssemblyQualifiedName);
                }

                _user = value;
            }
        }
        private Type _user;

        public Type Role {
            get { return _role; }
            set {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(JTRoleBase).IsAssignableFrom(value))
                {
                    throw new AbpException(value.AssemblyQualifiedName + " should be derived from " + typeof(JTRoleBase).AssemblyQualifiedName);
                }

                _role = value;
            }
        }
        private Type _role;

        public Type Tenant {
            get { return _tenant; }
            set {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                if (!typeof(JTTenantBase).IsAssignableFrom(value))
                {
                    throw new AbpException(value.AssemblyQualifiedName + " should be derived from " + typeof(JTTenantBase).AssemblyQualifiedName);
                }

                _tenant = value;
            }
        }
        private Type _tenant;
    }
}
