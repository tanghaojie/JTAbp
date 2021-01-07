using System;

namespace JT.Abp.Configuration
{
    public interface IJTEntityTypes
    {
        Type User { get; set; }

        Type Role { get; set; }

        Type Tenant { get; set; }
    }
}
