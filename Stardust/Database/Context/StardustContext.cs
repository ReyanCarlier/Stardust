using Microsoft.EntityFrameworkCore;

namespace Stardust.Database.Context
{
    public class StardustContext : DbContext
    {
        public DbSet<Data.Report> Reports { get; set; }
        public DbSet<Data.Machine> Machines { get; set; }
        public DbSet<Data.StardustUser> StardustUsers { get; set; }
        public DbSet<Data.Script> Scripts { get; set; }

        public DbSet<Data.GlobalScan> GlobalScans { get; set; }
        public DbSet<Data.TaskTodo> TasksTodo { get; set; }

        protected readonly IConfiguration Configuration;

        public StardustContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _ = optionsBuilder.UseSqlite(Configuration.GetConnectionString("Stardust"));
        }
    }
}
