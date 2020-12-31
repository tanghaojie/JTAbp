using Abp.EntityFrameworkCore;
using JT.Abp.Common.Configuration;
using Microsoft.EntityFrameworkCore;

namespace JT.AbpCore.EntityFrameworkCore.EntityFramework
{
    public abstract class JTAbpCommonDbContext<TSelf> : AbpDbContext
         where TSelf : JTAbpCommonDbContext<TSelf>
    {
        public virtual DbSet<Setting> Settings { get; set; }

        protected JTAbpCommonDbContext(DbContextOptions<TSelf> options)
           : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Setting>(b =>
            {
                b.HasIndex(e => new { e.TenantId, e.Name, e.UserId }).IsUnique().HasFilter(null);
            });
        }
    }
}
