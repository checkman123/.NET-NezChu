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
        #endregion

        #region DbSets
        public DbSet<IpLog> IpLogs { get; set; }
        #endregion
    }
}
