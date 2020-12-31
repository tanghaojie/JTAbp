using Abp.MultiTenancy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.Abp.Common.Tenancy
{
    public static class NoneTenancyConsts
    {
        /// <summary>
        /// Abp define tenant id = 1 when disable multi tenant.
        /// </summary>
        public const int DefaultTenantId = MultiTenancyConsts.DefaultTenantId;
        public const string DefaultTenantName = "DefaultSingleTenancyName";
    }
}
