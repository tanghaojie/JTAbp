using Abp.EntityFrameworkCore;
using JT.AbpCore.EntityFrameworkCore.EntityFramework;
using JT.Authorization.Roles;
using JT.Authorization.Users;
using JT.Entities;
using JT.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace JT.EntityFrameworkCore
{
    public class JTDbContext : JTAbpDbContext<Tenant, Role, User, JTDbContext>
    {
        //Add DbSet properties for your entities...

        public DbSet<Test> Tests { get; set; }

        public JTDbContext(DbContextOptions<JTDbContext> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
