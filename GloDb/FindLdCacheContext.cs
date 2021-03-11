using GloDb.FindLdCache;
using Microsoft.EntityFrameworkCore;

namespace GloDb
{
    public class FindLdCacheContext : DbContext
    {
        private readonly string _dbFilename;

        public FindLdCacheContext(string dbName)
        {
            _dbFilename = dbName;
        }

        public DbSet<FindLdCache.FindLdCache> FindLdCache { get; set; }
        public DbSet<FindLdCacheErrorLog> FindLdCacheErrorLog { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder
                .UseSqlite($@"Data Source={_dbFilename}");
        }
    }
}