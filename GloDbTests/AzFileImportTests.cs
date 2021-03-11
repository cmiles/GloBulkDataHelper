using System;
using System.IO;
using System.Linq;
using GloDb;
using GloDb.GloCsvFile;
using NUnit.Framework;
using Serilog;

namespace GloDbTests
{
    public class AzFileImportTests : IProgress<string>
    {
        public string DbFileName { get; set; }
        public string TestDirectory { get; set; }
        public string TestFileDirectory { get; set; }

        public void Report(string value)
        {
            Log.Information(value);
        }

        [Test]
        public void A001_DbExists()
        {
            Assert.IsTrue(File.Exists(DbFileName));
        }

        [Test]
        public void B001_AuthorityFileCsvRecordsImport()
        {
            var imports =
                GloCsvFileImporter.AuthorityLookupCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_Authority_Lookup.csv"), this);

            Assert.AreEqual(1041, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new AuthorityLookupCsv
            {
                authority_code = "000000",
                statutory_ref = string.Empty,
                act_treaty = "NA",
                entry_class = "Public Land (no Action)"
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            var midRecord = imports[611];
            var midTestRecord = new AuthorityLookupCsv
            {
                authority_code = "278501",
                statutory_ref = "82 Stat. 870",
                act_treaty = "September 26, 1968",
                entry_class = "Sale-Pls Unintentional"
            };

            Assert.AreEqual(midTestRecord, midRecord);

            var lastRecord = imports.Last();
            var lastTestRecord = new AuthorityLookupCsv
            {
                authority_code = "999999",
                statutory_ref = string.Empty,
                act_treaty = "January 1, 1999",
                entry_class = "No Authority Available"
            };

            Assert.AreEqual(lastTestRecord, lastRecord);
        }


        [SetUp]
        public void Setup()
        {
            TestDirectory = AppDomain.CurrentDomain.BaseDirectory;
            TestFileDirectory = "Az-2021-03-01-TestData";
            DbFileName = Path.Combine(TestDirectory, $"Test-AZ-2021-03-01-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.db");
            Logging.StandardConfiguration("Test-AZ-2021-03-01");
            var contextForSetup = new GloDataContext($"Test-AZ-2021-03-01-{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.db");
            contextForSetup.Database.EnsureCreated();
        }
    }
}