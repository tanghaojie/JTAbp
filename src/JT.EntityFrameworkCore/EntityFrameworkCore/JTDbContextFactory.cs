using JT.Configuration;
using JT.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace JT.EntityFrameworkCore
{
    /* This class is needed to run EF Core PMC commands. Not used anywhere else */
    public class JTDbContextFactory : IDesignTimeDbContextFactory<JTDbContext>
    {
        public JTDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<JTDbContext>();
            var configuration = AppConfigurations.Get(WebContentDirectoryFinder.CalculateContentRootFolder());

            DbContextOptionsConfigurer.Configure(
                builder,
                configuration.GetConnectionString(JTConsts.ConnectionStringName)
            );

            return new JTDbContext(builder.Options);
        }
    }
}