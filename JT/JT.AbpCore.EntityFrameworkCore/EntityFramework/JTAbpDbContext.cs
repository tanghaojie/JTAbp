using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JT.AbpCore.EntityFrameworkCore.EntityFramework
{
    public abstract class JTAbpDbContext<TSelf> : JTAbpCommonDbContext<TSelf>
         where TSelf : JTAbpDbContext<TSelf>
    {
        protected JTAbpDbContext(DbContextOptions<TSelf> options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
