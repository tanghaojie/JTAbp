using Microsoft.EntityFrameworkCore;

namespace JT.EntityFrameworkCore
{
    public static class DbContextOptionsConfigurer
    {
        public static void Configure(
            DbContextOptionsBuilder<JTDbContext> dbContextOptions, 
            string connectionString
            )
        {
            /* This is the single point to configure DbContextOptions for JTDbContext */
            dbContextOptions.UseNpgsql(connectionString);
        }
    }
}
