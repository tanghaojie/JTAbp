using Abp.Domain.Entities;
using Abp.Domain.Entities.Auditing;
using Abp.Extensions;
using JT.Abp.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JT.Abp.Authorization.Users
{
    public abstract class JTUserBase : FullAuditedEntity<long>, IMayHaveTenant, IPassivable
    {
        public const int MaxUserNameLength = 256;

        public const int MaxEmailAddressLength = 256;

        public const int MaxNameLength = 64;

        public const int MaxSurnameLength = 64;

        public const int MaxAuthenticationSourceLength = 64;

        public const string AdminUserName = "admin";

        public const int MaxPasswordLength = 128;

        public const int MaxPlainPasswordLength = 32;

        public const int MaxEmailConfirmationCodeLength = 328;

        public const int MaxPasswordResetCodeLength = 328;

        public const int MaxPhoneNumberLength = 32;

        public const int MaxSecurityStampLength = 128;

        [StringLength(MaxAuthenticationSourceLength)]
        public virtual string AuthenticationSource { get; set; }

        [Required]
        [StringLength(MaxUserNameLength)]
        public virtual string UserName { get; set; }

        public virtual int? TenantId { get; set; }

        [Required]
        [StringLength(MaxEmailAddressLength)]
        public virtual string EmailAddress { get; set; }

        [Required]
        [StringLength(MaxNameLength)]
        public virtual string Name { get; set; }

        [Required]
        [StringLength(MaxSurnameLength)]
        public virtual string Surname { get; set; }

        [NotMapped]
        public virtual string FullName { get { return this.Name + " " + this.Surname; } }

        [Required]
        [StringLength(MaxPasswordLength)]
        public virtual string Password { get; set; }

        [StringLength(MaxEmailConfirmationCodeLength)]
        public virtual string EmailConfirmationCode { get; set; }

        [StringLength(MaxPasswordResetCodeLength)]
        public virtual string PasswordResetCode { get; set; }

        public virtual DateTime? LockoutEndDateUtc { get; set; }

        public virtual int AccessFailedCount { get; set; }

        public virtual bool IsLockoutEnabled { get; set; }

        [StringLength(MaxPhoneNumberLength)]
        public virtual string PhoneNumber { get; set; }

        public virtual bool IsPhoneNumberConfirmed { get; set; }

        [StringLength(MaxSecurityStampLength)]
        public virtual string SecurityStamp { get; set; }

        public virtual bool IsTwoFactorEnabled { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<JTUserLogin> Logins { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<JTUserRole> Roles { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<JTUserClaim> Claims { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<JTUserPermission> Permissions { get; set; }

        [ForeignKey("UserId")]
        public virtual ICollection<JTSetting> Settings { get; set; }

        public virtual bool IsEmailConfirmed { get; set; }

        public virtual bool IsActive { get; set; }

        protected JTUserBase()
        {
            IsActive = true;
        }

        public virtual void SetNewPasswordResetCode()
        {
            PasswordResetCode = Guid.NewGuid().ToString("N").Truncate(MaxPasswordResetCodeLength);
        }

        public virtual void SetNewEmailConfirmationCode()
        {
            EmailConfirmationCode = Guid.NewGuid().ToString("N").Truncate(MaxEmailConfirmationCodeLength);
        }

        public override string ToString()
        {
            return $"[User {Id}] {UserName}";
        }
    }
}
