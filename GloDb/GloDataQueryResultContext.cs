using System.IO;
using Microsoft.EntityFrameworkCore;

namespace GloDb
{
    public class GloDataQueryResultContext : DbContext
    {
        private readonly string _dbFilename;

        public GloDataQueryResultContext(DirectoryInfo dbDirectory, string dbBaseName)
        {
            if (!dbDirectory.Exists) dbDirectory.Create();
            _dbFilename = Path.Combine(dbDirectory.FullName, dbBaseName);
        }

        public DbSet<GloDataQueryResult.GloDataQueryResult> FindLdResults { get; set; }
    }
}