using Microsoft.EntityFrameworkCore;
using NezChu.Database.Entities;

namespace NezChu.Database
{
    public class NezChuDbContext : DbContext
    {
        #region Constructors

        public NezChuDbContext() { }

        public NezChuDbContext(DbContextOptions<NezChuDbContext> options) : base(options) { }

        #endregion

        #region DbSets
        public DbSet<IpLog> IpLogs { get; set; }
        #endregion
    }
}
