using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using GloDb;
using GloDb.GloCsvFile;
using GloDb.GloData;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Serilog;

namespace GloDbTests
{
    public class AzFileToDbImportIntegrationTests : IProgress<string>
    {
        public const string StateDataFileValue = "AZ Test File";
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
        public async Task B001_AuthorityFileCsvRecordsImport()
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

            await GloCsvFileImporter.AuthorityLookupToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord = await context.AuthorityLookups.SingleAsync(x => x.AuthorityCode == "000000");
            var firstDbTestRecord = new AuthorityLookup
            {
                AuthorityCode = "000000",
                StatutoryReference = string.Empty,
                ActTreaty = "NA",
                EntryClass = "Public Land (no Action)",
                StateDataFile = StateDataFileValue
            };
            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord = await context.AuthorityLookups.SingleAsync(x => x.AuthorityCode == "278501");
            var midDbTestRecord = new AuthorityLookup
            {
                AuthorityCode = "278501",
                StatutoryReference = "82 Stat. 870",
                ActTreaty = "September 26, 1968",
                EntryClass = "Sale-Pls Unintentional",
                StateDataFile = StateDataFileValue
            };
            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord = await context.AuthorityLookups.SingleAsync(x => x.AuthorityCode == "999999");
            var lastDbTestRecord = new AuthorityLookup
            {
                AuthorityCode = "999999",
                StatutoryReference = string.Empty,
                ActTreaty = "January 1, 1999",
                EntryClass = "No Authority Available",
                StateDataFile = StateDataFileValue
            };
            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
        }

        [Test]
        public async Task C001_CountyFileCsvRecordsImport()
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

            await GloCsvFileImporter.CountyToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord = await context.Counties.SingleAsync(x => x.AccessionNumber == "0506-253"
                                                                        && x.DocumentClassCode == "IA" &&
                                                                        x.DescriptionNumber == 1 &&
                                                                        x.CountyCode == "019");
            var firstDbTestRecord = new County
            {
                AccessionNumber = "0506-253",
                DocumentClassCode = "IA",
                DescriptionNumber = 1,
                StateCode = "AZ",
                CountyCode = "019",
                StateDataFile = StateDataFileValue
            };

            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord = await context.Counties.SingleAsync(x => x.AccessionNumber == "AZAZAA 011328  01"
                                                                      && x.DocumentClassCode == "SER" &&
                                                                      x.DescriptionNumber == 1 &&
                                                                      x.CountyCode == "019");
            var midDbTestRecord = new County
            {
                AccessionNumber = "AZAZAA 011328  01",
                DocumentClassCode = "SER",
                DescriptionNumber = 1,
                StateCode = "AZ",
                CountyCode = "019",
                StateDataFile = StateDataFileValue
            };

            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord = await context.Counties.SingleAsync(x => x.AccessionNumber == "AZPHX 0086517"
                                                                       && x.DocumentClassCode == "SER" &&
                                                                       x.DescriptionNumber == 1 &&
                                                                       x.CountyCode == "003");
            var lastDbTestRecord = new County
            {
                AccessionNumber = "AZPHX 0086517",
                DocumentClassCode = "SER",
                DescriptionNumber = 1,
                StateCode = "AZ",
                CountyCode = "003",
                StateDataFile = StateDataFileValue
            };

            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
        }

        [Test]
        public async Task D001_CountyLookupFileCsvRecordsImport()
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

            await GloCsvFileImporter.CountyLookupToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord = await context.CountyLookups.SingleAsync(x => x.CountyCode == "000");
            var firstDbTestRecord = new CountyLookup
            {
                StateCode = "AZ",
                CountyCode = "000",
                CountyName = string.Empty,
                StateDataFile = StateDataFileValue
            };

            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord = await context.CountyLookups.SingleAsync(x => x.CountyCode == "019");
            var midDbTestRecord = new CountyLookup
            {
                StateCode = "AZ",
                CountyCode = "019",
                CountyName = "Pima",
                StateDataFile = StateDataFileValue
            };

            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord = await context.CountyLookups.SingleAsync(x => x.CountyCode == "030");
            var lastDbTestRecord = new CountyLookup
            {
                StateCode = "AZ",
                CountyCode = "030",
                CountyName = string.Empty,
                StateDataFile = StateDataFileValue
            };

            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
        }

        [Test]
        public async Task E001_DocClassLookupFileCsvRecordsImport()
        {
            var imports =
                GloCsvFileImporter.DocumentClassLookupCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
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

            await GloCsvFileImporter.DocumentClassLookupToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord = await context.DocumentClassLookups.SingleAsync(x => x.DocumentClassCode == "AGS");
            var firstDbTestRecord = new DocumentClassLookup
            {
                DocumentClassCode = "AGS",
                DocumentClassDescription = "Agricultural Scrip Patent",
                DocumentClassDisplayName = "Agricultural Scrip",
                StateDataFile = StateDataFileValue
            };

            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord = await context.DocumentClassLookups.SingleAsync(x => x.DocumentClassCode == "FLS");
            var midDbTestRecord = new DocumentClassLookup
            {
                DocumentClassCode = "FLS",
                DocumentClassDescription = "Forest Lieu Selection Patent",
                DocumentClassDisplayName = "Forest Lieu Selection",
                StateDataFile = StateDataFileValue
            };

            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord = await context.DocumentClassLookups.SingleAsync(x => x.DocumentClassCode == "TC");
            var lastDbTestRecord = new DocumentClassLookup
            {
                DocumentClassCode = "TC",
                DocumentClassDescription = "Timber Culture Patent",
                DocumentClassDisplayName = "Timber Culture",
                StateDataFile = StateDataFileValue
            };

            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
        }

        [Test]
        public async Task F001_LandDescriptionFileCsvRecordsImport()
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
                aliquot_parts = "N½N½",
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

            await GloCsvFileImporter.LandDescriptionToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord = await context.LandDescriptions.SingleAsync(x =>
                x.AccessionNumber == "0137-231" && x.DocumentClassCode == "TC" && x.DescriptionNumber == 1 &&
                x.AliquotParts == "NW");
            var firstDbTestRecord = new LandDescription
            {
                AccessionNumber = "0137-231",
                DocumentClassCode = "TC",
                DescriptionNumber = 1,
                AliquotParts = "NW",
                SectionNumber = 32,
                TownshipNumber = 2,
                TownshipDirection = "N",
                RangeNumber = 3,
                RangeDirection = "E",
                BlockNumber = string.Empty,
                FractionalSection = "N",
                SurveyNumber = string.Empty,
                MeridianCode = 14,
                LandDescriptionRemarks = string.Empty,
                StateCode = "AZ",
                StateDataFile = StateDataFileValue
            };

            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord = await context.LandDescriptions.SingleAsync(x =>
                x.AccessionNumber == "1189804" && x.DocumentClassCode == "SER" && x.DescriptionNumber == 1 &&
                x.AliquotParts == "1");
            var midDbTestRecord = new LandDescription
            {
                AccessionNumber = "1189804",
                DocumentClassCode = "SER",
                DescriptionNumber = 1,
                AliquotParts = "1",
                SectionNumber = 27,
                TownshipNumber = 19,
                TownshipDirection = "S",
                RangeNumber = 7,
                RangeDirection = "E",
                BlockNumber = string.Empty,
                FractionalSection = "N",
                SurveyNumber = string.Empty,
                MeridianCode = 14,
                LandDescriptionRemarks = "LOT 1 OR SOUTH HALF OF SE QUARTER\nSUBJECT TO RIGHTS AND R-O-W S",
                StateCode = "AZ",
                StateDataFile = StateDataFileValue
            };

            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbNextSingleRecord = await context.LandDescriptions.SingleAsync(x =>
                x.AccessionNumber == "AZPHX 0086517" && x.DocumentClassCode == "SER" && x.DescriptionNumber == 1 &&
                x.AliquotParts == "7");
            var midDbNextTestRecord = new LandDescription
            {
                AccessionNumber = "AZPHX 0086517",
                DocumentClassCode = "SER",
                DescriptionNumber = 1,
                AliquotParts = "7",
                SectionNumber = 6,
                TownshipNumber = 24,
                TownshipDirection = "S",
                RangeNumber = 29,
                RangeDirection = "E",
                BlockNumber = string.Empty,
                FractionalSection = "N",
                SurveyNumber = string.Empty,
                MeridianCode = 14,
                LandDescriptionRemarks = "LOT 7 OR NESW QUARTER",
                StateCode = "AZ",
                StateDataFile = StateDataFileValue
            };

            midDbNextTestRecord.Should().BeEquivalentTo(midDbNextSingleRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord = await context.LandDescriptions.SingleAsync(x =>
                x.AccessionNumber == "AZPHX 0086517" && x.DocumentClassCode == "SER" && x.DescriptionNumber == 1 &&
                x.AliquotParts == "7");
            var lastDbTestRecord = new LandDescription
            {
                AccessionNumber = "AZPHX 0086517",
                DocumentClassCode = "SER",
                DescriptionNumber = 1,
                AliquotParts = "7",
                SectionNumber = 6,
                TownshipNumber = 24,
                TownshipDirection = "S",
                RangeNumber = 29,
                RangeDirection = "E",
                BlockNumber = string.Empty,
                FractionalSection = "N",
                SurveyNumber = string.Empty,
                MeridianCode = 14,
                LandDescriptionRemarks = "LOT 7 OR NESW QUARTER",
                StateCode = "AZ",
                StateDataFile = StateDataFileValue
            };

            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
        }

        [Test]
        public async Task G001_LandOfficeLookupFileCsvRecordsImport()
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

            await GloCsvFileImporter.LandOfficeLookupToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord = await context.LandOfficeLookups.SingleAsync(x => x.LandOfficeCode == 1);
            var firstDbTestRecord = new LandOfficeLookup
            {
                StateCode = "AZ",
                LandOfficeCode = 1,
                LandOfficeDescription = "Arizona",
                StateDataFile = StateDataFileValue
            };

            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord = await context.LandOfficeLookups.SingleAsync(x => x.LandOfficeCode == 4);
            var midDbTestRecord = new LandOfficeLookup
            {
                StateCode = "AZ",
                LandOfficeCode = 4,
                LandOfficeDescription = "Gen Land Office",
                StateDataFile = StateDataFileValue
            };

            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord = await context.LandOfficeLookups.SingleAsync(x => x.LandOfficeCode == 9);
            var lastDbTestRecord = new LandOfficeLookup
            {
                StateCode = "AZ",
                LandOfficeCode = 9,
                LandOfficeDescription = "Tucson",
                StateDataFile = StateDataFileValue
            };

            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
        }

        [Test]
        public async Task H001_AzMeridianLookupImport()
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

            await GloCsvFileImporter.MeridianLookupToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord = await context.MeridianLookups.SingleAsync(x => x.MeridianCode == 14);
            var firstDbTestRecord = new MeridianLookup
            {
                StateCode = "AZ",
                MeridianCode = 14,
                MeridianName = "Gila-Salt River",
                StateDefault = string.Empty,
                StateDataFile = StateDataFileValue
            };

            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord = await context.MeridianLookups.SingleAsync(x => x.MeridianCode == 22);
            var midDbTestRecord = new MeridianLookup
            {
                StateCode = "AZ",
                MeridianCode = 22,
                MeridianName = "Navajo",
                StateDefault = string.Empty,
                StateDataFile = StateDataFileValue
            };

            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord = await context.MeridianLookups.SingleAsync(x => x.MeridianCode == 99);
            var lastDbTestRecord = new MeridianLookup
            {
                StateCode = "AZ",
                MeridianCode = 99,
                MeridianName = "No Meridian Available",
                StateDefault = string.Empty,
                StateDataFile = StateDataFileValue
            };

            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
        }

        [Test]
        public async Task I001_AzPatentImport()
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

            await GloCsvFileImporter.PatentToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord = await context.Patents.SingleAsync(x => x.AccessionNumber == "0506-253");
            var firstDbTestRecord = new Patent
            {
                AccessionNumber = "0506-253",
                DocumentClassCode = "IA",
                StateCode = "AZ",
                BlmSerialNumber = string.Empty,
                AuthorityCode = "253000",
                DocumentNumber = "0",
                MiscellaneousDocumentNumber = string.Empty,
                IndianAllotmentNumber = string.Empty,
                Tribe = "PAPAGO OR SAN XAVIER",
                LandOfficeCode = 2,
                SignaturePresent = true,
                SignatureDate = DateTime.Parse("10/17/1891 12:00:00 AM"),
                SubsurfaceReserved = false,
                MetesBounds = false,
                SurveyDate = null,
                UsReservations = false,
                CancelledDocument = false,
                GeographicName = string.Empty,
                TotalAcres = 256,
                Remarks = string.Empty,
                VerifyFlag = true,
                ImagePageNumber = 1,
                MilitaryRank = string.Empty,
                Militia = string.Empty,
                AltAccessionNumber = null,
                StateInFavorOf = string.Empty,
                SupremeCourtScriptNumber = string.Empty,
                CertificateOfLocation = string.Empty,
                CoalEntryNumber = string.Empty,
                StateDataFile = StateDataFileValue
            };

            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord = await context.Patents.SingleAsync(x => x.AccessionNumber == "02-91-0001");
            var midDbTestRecord = new Patent
            {
                AccessionNumber = "02-91-0001",
                DocumentClassCode = "SER",
                StateCode = "AZ",
                BlmSerialNumber = "A-23240",
                AuthorityCode = "255000",
                DocumentNumber = "0",
                MiscellaneousDocumentNumber = string.Empty,
                IndianAllotmentNumber = string.Empty,
                Tribe = string.Empty,
                LandOfficeCode = 8,
                SignaturePresent = true,
                SignatureDate = DateTime.Parse("10/18/1990 0:00"),
                SubsurfaceReserved = false,
                MetesBounds = false,
                SurveyDate = null,
                UsReservations = true,
                CancelledDocument = false,
                GeographicName = "LAURA MAE #1, #2, #3 AND #4",
                TotalAcres = 71.7920M,
                Remarks = "HARBORLITE MILLSITE #1A AND #2A",
                VerifyFlag = true,
                ImagePageNumber = 3,
                MilitaryRank = string.Empty,
                Militia = string.Empty,
                AltAccessionNumber = null,
                StateInFavorOf = string.Empty,
                SupremeCourtScriptNumber = string.Empty,
                CertificateOfLocation = string.Empty,
                CoalEntryNumber = string.Empty,
                StateDataFile = StateDataFileValue
            };

            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midTwoDbRecord = await context.Patents.SingleAsync(x => x.AccessionNumber == "0557-120");
            var midTwoDbTestRecord = new Patent
            {
                AccessionNumber = "0557-120",
                DocumentClassCode = "MW",
                StateCode = "AZ",
                BlmSerialNumber = string.Empty,
                AuthorityCode = "261006",
                DocumentNumber = "89393",
                MiscellaneousDocumentNumber = string.Empty,
                IndianAllotmentNumber = string.Empty,
                Tribe = string.Empty,
                LandOfficeCode = 9,
                SignaturePresent = true,
                SignatureDate = DateTime.Parse("12/24/1903  12:00:00 AM"),
                SubsurfaceReserved = false,
                MetesBounds = false,
                SurveyDate = null,
                UsReservations = true,
                CancelledDocument = false,
                GeographicName = string.Empty,
                TotalAcres = 120,
                Remarks = string.Empty,
                VerifyFlag = true,
                ImagePageNumber = 1,
                MilitaryRank = "WARRIOR",
                Militia = "CAPTAIN SPOAKOKEE MICCOS COMPANY CREEK VOLUNTEERS",
                AltAccessionNumber = null,
                StateInFavorOf = string.Empty,
                SupremeCourtScriptNumber = string.Empty,
                CertificateOfLocation = string.Empty,
                CoalEntryNumber = string.Empty,
                StateDataFile = StateDataFileValue
            };

            midTwoDbTestRecord.Should().BeEquivalentTo(midTwoDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord = await context.Patents.SingleAsync(x => x.AccessionNumber == "AZPHX 0086517");
            var lastDbTestRecord = new Patent
            {
                AccessionNumber = "AZPHX 0086517",
                DocumentClassCode = "SER",
                StateCode = "AZ",
                BlmSerialNumber = "AZPHX 0086517",
                AuthorityCode = "262200",
                DocumentNumber = string.Empty,
                MiscellaneousDocumentNumber = string.Empty,
                IndianAllotmentNumber = string.Empty,
                Tribe = string.Empty,
                LandOfficeCode = 8,
                SignaturePresent = true,
                SignatureDate = DateTime.Parse("10/5/1951  12:00:00 AM"),
                SubsurfaceReserved = false,
                MetesBounds = false,
                SurveyDate = null,
                UsReservations = true,
                CancelledDocument = false,
                GeographicName = string.Empty,
                TotalAcres = 40,
                Remarks = string.Empty,
                VerifyFlag = false,
                ImagePageNumber = 0,
                MilitaryRank = string.Empty,
                Militia = string.Empty,
                AltAccessionNumber = null,
                StateInFavorOf = string.Empty,
                SupremeCourtScriptNumber = string.Empty,
                CertificateOfLocation = string.Empty,
                CoalEntryNumber = string.Empty,
                StateDataFile = StateDataFileValue
            };

            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
        }

        [Test]
        public async Task J001_AzPatenteeImport()
        {
            var imports =
                GloCsvFileImporter.PatenteeCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_Patentee.csv"), this);

            Assert.AreEqual(81978, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new PatenteeCsv
            {
                accession_nr = "0137-231",
                doc_class_code = "TC",
                patentee_seq_nr = 1,
                patentee_last_name = "HOLCOMB",
                patentee_first_name = "JAMES",
                patentee_middle_name = "P"
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            var midRecord = imports[40021];
            var midTestRecord = new PatenteeCsv
            {
                accession_nr = "881526",
                doc_class_code = "SER",
                patentee_seq_nr = 3,
                patentee_last_name = "BALDWIN",
                patentee_first_name = "CLARENCE",
                patentee_middle_name = "A"
            };

            Assert.AreEqual(midTestRecord, midRecord);

            var lastRecord = imports.Last();
            var lastTestRecord = new PatenteeCsv
            {
                accession_nr = "AZPHX 0086517",
                doc_class_code = "SER",
                patentee_seq_nr = 1,
                patentee_last_name = "ARIZONA STATE OF",
                patentee_first_name = string.Empty,
                patentee_middle_name = string.Empty
            };

            Assert.AreEqual(lastTestRecord, lastRecord);

            await GloCsvFileImporter.PatenteeToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord =
                await context.Patentees.SingleAsync(x =>
                    x.AccessionNumber == "0137-231" && x.PatenteeSequenceNumber == 1);
            var firstDbTestRecord = new Patentee
            {
                AccessionNumber = "0137-231",
                DocumentClassCode = "TC",
                PatenteeSequenceNumber = 1,
                PatenteeLastName = "HOLCOMB",
                PatenteeFirstName = "JAMES",
                PatenteeMiddleName = "P",
                StateDataFile = StateDataFileValue
            };

            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord =
                await context.Patentees.SingleAsync(x =>
                    x.AccessionNumber == "881526" && x.PatenteeSequenceNumber == 3);
            var midDbTestRecord = new Patentee
            {
                AccessionNumber = "881526",
                DocumentClassCode = "SER",
                PatenteeSequenceNumber = 3,
                PatenteeLastName = "BALDWIN",
                PatenteeFirstName = "CLARENCE",
                PatenteeMiddleName = "A",
                StateDataFile = StateDataFileValue
            };

            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord = await context.Patentees.SingleAsync(x =>
                x.AccessionNumber == "AZPHX 0086517" && x.PatenteeSequenceNumber == 1);
            var lastDbTestRecord = new Patentee
            {
                AccessionNumber = "AZPHX 0086517",
                DocumentClassCode = "SER",
                PatenteeSequenceNumber = 1,
                PatenteeLastName = "ARIZONA STATE OF",
                PatenteeFirstName = string.Empty,
                PatenteeMiddleName = string.Empty,
                StateDataFile = StateDataFileValue
            };

            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
        }

        [Test]
        public async Task J001_AzWarranteeImport()
        {
            var imports =
                GloCsvFileImporter.WarranteeCsvRecords(Path.Combine(TestDirectory, TestFileDirectory,
                    "AZ_Warrantee.csv"), this);

            Assert.AreEqual(37, imports.Count);

            var firstRecord = imports.First();
            var firstTestRecord = new WarranteeCsv
            {
                accession_nr = "0557-120",
                doc_class_code = "MW",
                warrantee_seq_nr = 1,
                warrantee_last_name = "ARARTE",
                warrantee_first_name = string.Empty,
                warrantee_middle_name = string.Empty
            };

            Assert.AreEqual(firstTestRecord, firstRecord);

            var midRecord = imports[16];
            var midTestRecord = new WarranteeCsv
            {
                accession_nr = "0564-467",
                doc_class_code = "MW",
                warrantee_seq_nr = 1,
                warrantee_last_name = "VAN ALSTINE",
                warrantee_first_name = "NELSON",
                warrantee_middle_name = string.Empty
            };

            Assert.AreEqual(midTestRecord, midRecord);

            var lastRecord = imports.Last();
            var lastTestRecord = new WarranteeCsv
            {
                accession_nr = "846372",
                doc_class_code = "SER",
                warrantee_seq_nr = 1,
                warrantee_last_name = "HAWKE",
                warrantee_first_name = "WILLIAM",
                warrantee_middle_name = "A"
            };

            Assert.AreEqual(lastTestRecord, lastRecord);

            await GloCsvFileImporter.WarranteeToDb(imports, StateDataFileValue, DbFileName, this);

            var context = new GloDataContext(DbFileName);

            var firstDbRecord = await context.Warrantees.SingleAsync(x =>
                x.AccessionNumber == "0557-120" && x.WarranteeSequenceNumber == 1);
            var firstDbTestRecord = new Warrantee
            {
                AccessionNumber = "0557-120",
                DocumentClassCode = "MW",
                WarranteeSequenceNumber = 1,
                WarranteeLastName = "ARARTE",
                WarranteeFirstName = string.Empty,
                WarranteeMiddleName = string.Empty,
                StateDataFile = StateDataFileValue
            };

            firstDbTestRecord.Should().BeEquivalentTo(firstDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var midDbRecord =
                await context.Warrantees.SingleAsync(x =>
                    x.AccessionNumber == "0564-467" && x.WarranteeSequenceNumber == 1);
            var midDbTestRecord = new Warrantee
            {
                AccessionNumber = "0564-467",
                DocumentClassCode = "MW",
                WarranteeSequenceNumber = 1,
                WarranteeLastName = "VAN ALSTINE",
                WarranteeFirstName = "NELSON",
                WarranteeMiddleName = string.Empty,
                StateDataFile = StateDataFileValue
            };

            midDbTestRecord.Should().BeEquivalentTo(midDbRecord, option => option
                .Excluding(x => x.Path == "Id"));

            var lastDbRecord =
                await context.Warrantees.SingleAsync(x =>
                    x.AccessionNumber == "846372" && x.WarranteeSequenceNumber == 1);
            var lastDbTestRecord = new Warrantee
            {
                AccessionNumber = "846372",
                DocumentClassCode = "SER",
                WarranteeSequenceNumber = 1,
                WarranteeLastName = "HAWKE",
                WarranteeFirstName = "WILLIAM",
                WarranteeMiddleName = "A",
                StateDataFile = StateDataFileValue
            };

            lastDbTestRecord.Should().BeEquivalentTo(lastDbRecord, option => option
                .Excluding(x => x.Path == "Id"));
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