using Abp.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace JT.Abp.Authorization.Users
{
    public class JTUserLogin : Entity<long>, IMayHaveTenant
    {
        public const int MaxLoginProviderLength = 128;

        public const int MaxProviderKeyLength = 256;

        public virtual int? TenantId { get; set; }

        public virtual long UserId { get; set; }

        [Required]
        [StringLength(MaxLoginProviderLength)]
        public virtual string LoginProvider { get; set; }

        [Required]
        [StringLength(MaxProviderKeyLength)]
        public virtual string ProviderKey { get; set; }

        public JTUserLogin()
        {
        }

        public JTUserLogin(int? tenantId, long userId, string loginProvider, string providerKey)
        {
            TenantId = tenantId;
            UserId = userId;
            LoginProvider = loginProvider;
            ProviderKey = providerKey;
        }
    }
}
