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

        [Test]
        public void C001_CountyFileCsvRecordsImport()
        {
            var imports =
                GloCsvFileImporter.CountyCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_County.csv"), this);

            Assert.AreEqual(250212, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new CountyCsv
            {
                accession_nr = "0506-253",
                doc_class_code = "IA",
                descrip_nr = 1,
                state_code = "AZ",
                county_code = "019"
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            var midRecord = imports[59535];
            var midTestRecord = new CountyCsv
            {
                accession_nr = "AZAZAA 011328  01",
                doc_class_code = "SER",
                descrip_nr = 1,
                state_code = "AZ",
                county_code = "019"
            };

            Assert.AreEqual(midTestRecord, midRecord);

            var lastRecord = imports.Last();
            var lastTestRecord = new CountyCsv
            {
                accession_nr = "AZPHX 0086517",
                doc_class_code = "SER",
                descrip_nr = 1,
                state_code = "AZ",
                county_code = "003"
            };

            Assert.AreEqual(lastTestRecord, lastRecord);
        }

        [Test]
        public void D001_CountyLookupFileCsvRecordsImport()
        {
            var imports =
                GloCsvFileImporter.CountyLookupCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_County_Lookup.csv"), this);

            Assert.AreEqual(17, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new CountyLookupCsv
            {
                state_code = "AZ",
                county_code = "000",
                county_name = string.Empty
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            var midRecord = imports[11];
            var midTestRecord = new CountyLookupCsv
            {
                state_code = "AZ",
                county_code = "019",
                county_name = "Pima"
            };

            Assert.AreEqual(midTestRecord, midRecord);

            var lastRecord = imports.Last();
            var lastTestRecord = new CountyLookupCsv
            {
                state_code = "AZ",
                county_code = "030",
                county_name = string.Empty
            };

            Assert.AreEqual(lastTestRecord, lastRecord);
        }

        [Test]
        public void E001_DocClassLookupFileCsvRecordsImport()
        {
            var imports =
                GloCsvFileImporter.DocClassLookupCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_Doc_Class_Lookup.csv"), this);

            Assert.AreEqual(20, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new DocumentClassLookupCsv
            {
                doc_class_code = "AGS",
                document_class_description = "Agricultural Scrip Patent",
                doc_class_display_name = "Agricultural Scrip"
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            var midRecord = imports[6];
            var midTestRecord = new DocumentClassLookupCsv
            {
                doc_class_code = "FLS",
                document_class_description = "Forest Lieu Selection Patent",
                doc_class_display_name = "Forest Lieu Selection"
            };

            Assert.AreEqual(midTestRecord, midRecord);

            var lastRecord = imports.Last();
            var lastTestRecord = new DocumentClassLookupCsv
            {
                doc_class_code = "TC",
                document_class_description = "Timber Culture Patent",
                doc_class_display_name = "Timber Culture"
            };

            Assert.AreEqual(lastTestRecord, lastRecord);
        }

        [Test]
        public void F001_LandDescriptionFileCsvRecordsImport()
        {
            var imports =
                GloCsvFileImporter.LandDescriptionCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_Land_Description.csv"), this);

            //The count is most easily seen in Excel - this CSV file contains
            //multi-line records where crlf is used for the record and lf for
            //the multi-line remarks
            Assert.AreEqual(249553, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new LandDescriptionCsv
            {
                accession_nr = "0137-231",
                doc_class_code = "TC",
                descrip_nr = 1,
                aliquot_parts = "NW",
                section_nr = 32,
                township_nr = 2,
                township_dir = "N",
                range_nr = 3,
                range_dir = "E",
                block_nr = string.Empty,
                fractional_section = "N",
                survey_nr = string.Empty,
                meridian_code = 14,
                ld_remarks = string.Empty,
                state_code = "AZ"
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            //This test row is the first multi-line record in the file
            var midRecord = imports[48737];
            var midTestRecord = new LandDescriptionCsv
            {
                accession_nr = "1189804",
                doc_class_code = "SER",
                descrip_nr = 1,
                aliquot_parts = "1",
                section_nr = 27,
                township_nr = 19,
                township_dir = "S",
                range_nr = 7,
                range_dir = "E",
                block_nr = string.Empty,
                fractional_section = "N",
                survey_nr = string.Empty,
                meridian_code = 14,
                ld_remarks = "LOT 1 OR SOUTH HALF OF SE QUARTER\nSUBJECT TO RIGHTS AND R-O-W S",
                state_code = "AZ"
            };

            Assert.AreEqual(midTestRecord, midRecord);

            //Test a record beyond the first multi-line record
            var midNextRecord = imports[139999];
            var midNextTestRecord = new LandDescriptionCsv
            {
                accession_nr = "AZA    006727",
                doc_class_code = "SER",
                descrip_nr = 1,
                aliquot_parts = "N�N�",
                section_nr = 8,
                township_nr = 1,
                township_dir = "N",
                range_nr = 8,
                range_dir = "E",
                block_nr = string.Empty,
                fractional_section = "N",
                survey_nr = string.Empty,
                meridian_code = 14,
                ld_remarks = "",
                state_code = "AZ"
            };

            Assert.AreEqual(midNextTestRecord, midNextRecord);

            var lastRecord = imports.Last();
            var lastTestRecord = new LandDescriptionCsv
            {
                accession_nr = "AZPHX 0086517",
                doc_class_code = "SER",
                descrip_nr = 1,
                aliquot_parts = "7",
                section_nr = 6,
                township_nr = 24,
                township_dir = "S",
                range_nr = 29,
                range_dir = "E",
                block_nr = string.Empty,
                fractional_section = "N",
                survey_nr = string.Empty,
                meridian_code = 14,
                ld_remarks = "LOT 7 OR NESW QUARTER",
                state_code = "AZ"
            };

            Assert.AreEqual(lastTestRecord, lastRecord);
        }

        [Test]
        public void G001_LandOfficeLookupFileCsvRecordsImport()
        {
            var imports =
                GloCsvFileImporter.LandOfficeLookupCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_Land_Office_Lookup.csv"), this);

            Assert.AreEqual(11, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new LandOfficeLookupCsv
            {
                state_code = "AZ",
                l_o_code = 1,
                l_o_description = "Arizona"
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            var midRecord = imports[5];
            var midTestRecord = new LandOfficeLookupCsv
            {
                state_code = "AZ",
                l_o_code = 4,
                l_o_description = "Gen Land Office"
            };

            Assert.AreEqual(midTestRecord, midRecord);

            var lastRecord = imports.Last();
            var lastTestRecord = new LandOfficeLookupCsv
            {
                state_code = "AZ",
                l_o_code = 9,
                l_o_description = "Tucson"
            };

            Assert.AreEqual(lastTestRecord, lastRecord);
        }

        [Test]
        public void H001_AzMeridianLookupImport()
        {
            var imports =
                GloCsvFileImporter.MeridianLookupCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_Meridian_Lookup.csv"), this);

            Assert.AreEqual(4, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new MeridianLookupCsv
            {
                state_code = "AZ",
                meridian_code = 14,
                meridian_name = "Gila-Salt River",
                state_default = string.Empty
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            var midRecord = imports[1];
            var midTestRecord = new MeridianLookupCsv
            {
                state_code = "AZ",
                meridian_code = 22,
                meridian_name = "Navajo",
                state_default = string.Empty
            };

            Assert.AreEqual(midTestRecord, midRecord);

            var lastRecord = imports.Last();
            var lastTestRecord = new MeridianLookupCsv
            {
                state_code = "AZ",
                meridian_code = 99,
                meridian_name = "No Meridian Available",
                state_default = string.Empty
            };

            Assert.AreEqual(lastTestRecord, lastRecord);
        }

        [Test]
        public void I001_AzPatentLookupImport()
        {
            var imports =
                GloCsvFileImporter.PatentCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_Patent.csv"), this);

            Assert.AreEqual(76961, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new PatentCsv
            {
                accession_nr = "0506-253",
                doc_class_code = "IA",
                state_code = "AZ",
                blm_serial_nr = string.Empty,
                authority_code = "253000",
                document_nr = "0",
                misc_document_nr = string.Empty,
                indian_allotment_nr = string.Empty,
                tribe = "PAPAGO OR SAN XAVIER",
                l_o_code = 2,
                signature_present = true,
                signature_date = DateTime.Parse("10/17/1891 12:00:00 AM"),
                subsurface_reserved = false,
                metes_bounds = false,
                survey_date = null,
                us_reservations = false,
                cancelled_doc = false,
                geographic_name = string.Empty,
                total_acres = 256,
                remarks = string.Empty,
                verify_flag = true,
                image_page_nr = 1,
                military_rank = string.Empty,
                militia = string.Empty,
                alt_accession_nr = null,
                state_in_favor_of = string.Empty,
                supreme_court_script_nr = string.Empty,
                certificate_of_location = string.Empty,
                coal_entry_nr = string.Empty
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            var midRecord = imports[29515];
            var midTestRecord = new PatentCsv
            {
                accession_nr = "02-91-0001",
                doc_class_code = "SER",
                state_code = "AZ",
                blm_serial_nr = "A-23240",
                authority_code = "255000",
                document_nr = "0",
                misc_document_nr = string.Empty,
                indian_allotment_nr = string.Empty,
                tribe = string.Empty,
                l_o_code = 8,
                signature_present = true,
                signature_date = DateTime.Parse("10/18/1990 0:00"),
                subsurface_reserved = false,
                metes_bounds = false,
                survey_date = null,
                us_reservations = true,
                cancelled_doc = false,
                geographic_name = "LAURA MAE #1, #2, #3 AND #4",
                total_acres = 71.7920M,
                remarks = "HARBORLITE MILLSITE #1A AND #2A",
                verify_flag = true,
                image_page_nr = 3,
                military_rank = string.Empty,
                militia = string.Empty,
                alt_accession_nr = null,
                state_in_favor_of = string.Empty,
                supreme_court_script_nr = string.Empty,
                certificate_of_location = string.Empty,
                coal_entry_nr = string.Empty
            };

            Assert.AreEqual(midTestRecord, midRecord);

            var midRecordTwo = imports[29877];
            var midTestRecordTwo = new PatentCsv
            {
                accession_nr = "0557-120",
                doc_class_code = "MW",
                state_code = "AZ",
                blm_serial_nr = string.Empty,
                authority_code = "261006",
                document_nr = "89393",
                misc_document_nr = string.Empty,
                indian_allotment_nr = string.Empty,
                tribe = string.Empty,
                l_o_code = 9,
                signature_present = true,
                signature_date = DateTime.Parse("12/24/1903  12:00:00 AM"),
                subsurface_reserved = false,
                metes_bounds = false,
                survey_date = null,
                us_reservations = true,
                cancelled_doc = false,
                geographic_name = string.Empty,
                total_acres = 120,
                remarks = string.Empty,
                verify_flag = true,
                image_page_nr = 1,
                military_rank = "WARRIOR",
                militia = "CAPTAIN SPOAKOKEE MICCOS COMPANY CREEK VOLUNTEERS",
                alt_accession_nr = null,
                state_in_favor_of = string.Empty,
                supreme_court_script_nr = string.Empty,
                certificate_of_location = string.Empty,
                coal_entry_nr = string.Empty
            };

            Assert.AreEqual(midTestRecordTwo, midRecordTwo);

            var lastRecord = imports.Last();
            var lastTestRecord = new PatentCsv
            {
                accession_nr = "AZPHX 0086517",
                doc_class_code = "SER",
                state_code = "AZ",
                blm_serial_nr = "AZPHX 0086517",
                authority_code = "262200",
                document_nr = string.Empty,
                misc_document_nr = string.Empty,
                indian_allotment_nr = string.Empty,
                tribe = string.Empty,
                l_o_code = 8,
                signature_present = true,
                signature_date = DateTime.Parse("10/5/1951  12:00:00 AM"),
                subsurface_reserved = false,
                metes_bounds = false,
                survey_date = null,
                us_reservations = true,
                cancelled_doc = false,
                geographic_name = string.Empty,
                total_acres = 40,
                remarks = string.Empty,
                verify_flag = false,
                image_page_nr = 0,
                military_rank = string.Empty,
                militia = string.Empty,
                alt_accession_nr = null,
                state_in_favor_of = string.Empty,
                supreme_court_script_nr = string.Empty,
                certificate_of_location = string.Empty,
                coal_entry_nr = string.Empty
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