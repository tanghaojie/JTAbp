using JT.Abp.Application.Editions;
using JT.Abp.Application.Features;
using JT.Abp.Authorization.Roles;
using JT.Abp.Authorization.Users;
using JT.Abp.MultiTenancy;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.AbpCore.EntityFrameworkCore.EntityFramework
{
    public abstract class JTAbpDbContext<TTenant, TRole, TUser, TSelf> : JTAbpCommonDbContext<TRole, TUser, TSelf>
        where TTenant : JTTenant<TUser>
        where TRole : JTRole<TUser>
        where TUser : JTUser<TUser>
        where TSelf : JTAbpDbContext<TTenant, TRole, TUser, TSelf>
    {
        public virtual DbSet<TTenant> Tenants { get; set; }
        public virtual DbSet<JTEdition> Editions { get; set; }
        public virtual DbSet<JTFeature> Features { get; set; }
        public virtual DbSet<JTTenantFeature> TenantFeatures { get; set; }
        public virtual DbSet<JTEditionFeature> EditionFeatures { get; set; }

        //public virtual DbSet<BackgroundJobInfo> BackgroundJobs { get; set; }
        //public virtual DbSet<UserAccount> UserAccounts { get; set; }
        //public virtual DbSet<NotificationInfo> Notifications { get; set; }

        protected JTAbpDbContext(DbContextOptions<TSelf> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<JTTenantFeature>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Name });
            });

            modelBuilder.Entity<JTEditionFeature>(b =>
            {
                b.HasIndex(e => new { e.EditionId, e.Name });
            });

            modelBuilder.Entity<TTenant>(b =>
            {
                b.HasOne(p => p.DeleterUser)
                    .WithMany()
                    .HasForeignKey(p => p.DeleterUserId);
                b.HasOne(p => p.CreatorUser)
                    .WithMany()
                    .HasForeignKey(p => p.CreatorUserId);
                b.HasOne(p => p.LastModifierUser)
                    .WithMany()
                    .HasForeignKey(p => p.LastModifierUserId);
                b.HasIndex(e => e.TenancyName);
            });
        }
    }
}
