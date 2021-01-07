using Abp.EntityFrameworkCore;
using JT.Abp.Authorization;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using JT.Abp.Configuration;
using Microsoft.EntityFrameworkCore;

namespace JT.AbpCore.EntityFrameworkCore.EntityFramework
{
    public abstract class JTAbpCommonDbContext<TRole, TUser, TSelf> : AbpDbContext
        where TRole : JTRole<TUser>
        where TUser : JTUser<TUser>
        where TSelf : JTAbpCommonDbContext<TRole, TUser, TSelf>
    {
        public virtual DbSet<TRole> Roles { get; set; }
        public virtual DbSet<TUser> Users { get; set; }
        public virtual DbSet<JTUserLogin> UserLogins { get; set; }
        public virtual DbSet<JTUserLoginAttempt> UserLoginAttempts { get; set; }
        public virtual DbSet<JTUserRole> UserRoles { get; set; }
        public virtual DbSet<JTUserClaim> UserClaims { get; set; }
        public virtual DbSet<JTUserToken> UserTokens { get; set; }
        public virtual DbSet<JTRoleClaim> RoleClaims { get; set; }
        public virtual DbSet<JTPermission> Permissions { get; set; }
        public virtual DbSet<JTRolePermission> RolePermissions { get; set; }
        public virtual DbSet<JTUserPermission> UserPermissions { get; set; }
        public virtual DbSet<JTSetting> Settings { get; set; }

        //public virtual DbSet<AuditLog> AuditLogs { get; set; }
        //public virtual DbSet<ApplicationLanguage> Languages { get; set; }
        //public virtual DbSet<ApplicationLanguageText> LanguageTexts { get; set; }
        //public virtual DbSet<OrganizationUnit> OrganizationUnits { get; set; }
        //public virtual DbSet<UserOrganizationUnit> UserOrganizationUnits { get; set; }
        //public virtual DbSet<OrganizationUnitRole> OrganizationUnitRoles { get; set; }
        //public virtual DbSet<TenantNotificationInfo> TenantNotifications { get; set; }
        //public virtual DbSet<UserNotificationInfo> UserNotifications { get; set; }
        //public virtual DbSet<NotificationSubscriptionInfo> NotificationSubscriptions { get; set; }
        //public virtual DbSet<EntityChange> EntityChanges { get; set; }
        //public virtual DbSet<EntityChangeSet> EntityChangeSets { get; set; }
        //public virtual DbSet<EntityPropertyChange> EntityPropertyChanges { get; set; }
        //public virtual DbSet<WebhookEvent> WebhookEvents { get; set; }
        //public virtual DbSet<WebhookSubscriptionInfo> WebhookSubscriptions { get; set; }
        //public virtual DbSet<WebhookSendAttempt> WebhookSendAttempts { get; set; }
        //public virtual DbSet<DynamicProperty> DynamicProperties { get; set; }
        //public virtual DbSet<DynamicPropertyValue> DynamicPropertyValues { get; set; }
        //public virtual DbSet<DynamicEntityProperty> DynamicEntityProperties { get; set; }
        //public virtual DbSet<DynamicEntityPropertyValue> DynamicEntityPropertyValues { get; set; }

        protected JTAbpCommonDbContext(DbContextOptions<TSelf> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TUser>(b =>
            {
                b.Property(u => u.ConcurrencyStamp).IsConcurrencyToken();
                b.HasOne(p => p.DeleterUser)
                    .WithMany()
                    .HasForeignKey(p => p.DeleterUserId);
                b.HasOne(p => p.CreatorUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatorUserId);
                b.HasOne(p => p.LastModifierUser)
                    .WithMany()
                    .HasForeignKey(p => p.LastModifierUserId);
            });

            modelBuilder.Entity<TRole>(b =>
            {
                //b.Property(r => r.ConcurrencyStamp).IsConcurrencyToken();
            });

            modelBuilder.Entity<JTPermission>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Name });
            });

            modelBuilder.Entity<JTRoleClaim>(b =>
            {
                b.HasIndex(e => new { e.RoleId });
                b.HasIndex(e => new { e.TenantId, e.ClaimType });
            });

            modelBuilder.Entity<TRole>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.NormalizedName });
            });

            modelBuilder.Entity<JTSetting>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Name, e.UserId }).IsUnique().HasFilter(null);
            });

            modelBuilder.Entity<JTUserClaim>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.ClaimType });
            });

            modelBuilder.Entity<JTUserLoginAttempt>(b =>
            {
                b.HasIndex(e => new { e.TenancyName, e.UserNameOrEmailAddress, e.Result });
                b.HasIndex(ula => new { ula.UserId, ula.TenantId });
            });

            modelBuilder.Entity<JTUserLogin>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.LoginProvider, e.ProviderKey });
                b.HasIndex(e => new { e.TenantId, e.UserId });
            });

            modelBuilder.Entity<JTUserRole>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
                b.HasIndex(e => new { e.TenantId, e.RoleId });
            });

            modelBuilder.Entity<TUser>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.NormalizedUserName });
                b.HasIndex(e => new { e.TenantId, e.NormalizedEmailAddress });
            });

            modelBuilder.Entity<JTUserToken>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.UserId });
            });
        }
    }
}
