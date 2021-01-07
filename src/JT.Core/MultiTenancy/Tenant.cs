using Abp.MultiTenancy;
using JT.Abp.MultiTenancy;
using JT.Authorization.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.MultiTenancy
{
    public class Tenant : JTTenant<User>
    {
        public Tenant()
        {
        }

        public Tenant(string tenancyName, string name)
            : base(tenancyName, name)
        {
        }
    }
}
