using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NezChu.Database.Entities;

namespace NezChu.Database
{
    public class NezChuDbContext : DbContext
    {
        private readonly IConfiguration _configuration;
        #region Constructors
        public NezChuDbContext(DbContextOptions<NezChuDbContext> options, IConfiguration configuration) : base(options) 
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(_configuration["SupabaseConnectionString"], builder =>
            {
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        }
        #endregion

        #region DbSets
        public DbSet<IpLog> IpLogs { get; set; }
        #endregion
    }
}
