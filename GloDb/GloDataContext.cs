using GloDb.GloData;
using Microsoft.EntityFrameworkCore;

namespace GloDb
{
    public class GloDataContext : DbContext
    {
        public GloDataContext(string dbName)
        {
            DbName = dbName;
        }

        public DbSet<AuthorityLookup> AuthorityLookups { get; set; }
        public DbSet<County> Counties { get; set; }
        public DbSet<CountyLookup> CountyLookups { get; set; }
        public string DbName { get; set; }
        public DbSet<DocumentClassLookup> DocumentClassLookups { get; set; }
        public DbSet<LandDescription> LandDescriptions { get; set; }
        public DbSet<LandOfficeLookup> LandOfficeLookups { get; set; }
        public DbSet<MeridianLookup> MeridianLookups { get; set; }
        public DbSet<Patentee> Patentees { get; set; }
        public DbSet<Patent> Patents { get; set; }
        public DbSet<Warrantee> Warrantees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite($"Data Source={DbName}");
        }
    }
}