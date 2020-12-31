using Abp.EntityFrameworkCore;
using JT.Entities;
using Microsoft.EntityFrameworkCore;

namespace JT.EntityFrameworkCore
{
    public class JTDbContext : AbpDbContext
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
