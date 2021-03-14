using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GloDb.GloCsvFile;
using GloDb.GloData;
using Omu.ValueInjecter;

namespace GloDb
{
    public static class GloCsvFileImporter
    {
        public static AuthorityLookup AuthorityLookupCsvToDbRecord(AuthorityLookupCsv toTransform, string stateDataFileCode)
        {
            return new AuthorityLookup
            {
                ActTreaty = toTransform.act_treaty,
                AuthorityCode = toTransform.authority_code,
                EntryClass = toTransform.entry_class,
                StateDataFile = stateDataFileCode,
                StatutoryReference = toTransform.statutory_ref
            };
        }

        public static List<AuthorityLookupCsv> AuthorityLookupCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Authority_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<AuthorityLookupCsv>(fileName, progress).ToList();

            progress.Report($"Finished Authority_Lookup Csv Import - {DateTime.Now}");

            return fileRecords;
        }

        public static County CountyCsvToDbRecord(CountyCsv toTransform, string stateDataFileCode)
        {
            return new County()
            {
                AccessionNumber = toTransform.accession_nr,
                CountyCode = toTransform.county_code,
                DescriptionNumber = toTransform.descrip_nr,
                DocClassCode = toTransform.doc_class_code,
                StateCode = toTransform.state_code,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<CountyCsv> CountyCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting County Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<CountyCsv>(fileName, progress).ToList();

            progress.Report($"Finished County Csv Import - {DateTime.Now}");

            return fileRecords;
        }

        public static CountyLookup CountyLookupCsvToDbRecord(CountyLookupCsv toTransform, string stateDataFileCode)
        {
            return new CountyLookup()
            {
                CountyCode = toTransform.county_code,
                CountyName = toTransform.county_name,
                StateCode = toTransform.state_code,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<CountyLookupCsv> CountyLookupCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting County_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<CountyLookupCsv>(fileName, progress).ToList();

            progress.Report($"Finished County_Lookup Csv Import - {DateTime.Now}");

            return fileRecords;
        }

        public static DocumentClassLookup DocumentClassLookupCsvToDbRecord(DocumentClassLookupCsv toTransform, string stateDataFileCode)
        {
            return new DocumentClassLookup()
            {
                DocumentClassCode = toTransform.doc_class_code,
                DocumentClassDescription = toTransform.document_class_description,
                DocumentClassDisplayName = toTransform.doc_class_display_name,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<DocumentClassLookupCsv> DocClassLookupCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Doc_Class_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<DocumentClassLookupCsv>(fileName, progress).ToList();

            progress.Report($"Finished Doc_Class_Lookup Csv Import - {DateTime.Now}");

            return fileRecords;
        }

        private static IEnumerable<List<T>> FileRecordBatches<T>(string fileName, IProgress<string> progress)
            where T : class, new()
        {
            progress.Report($"Starting {fileName} - {DateTime.Now:T}");

            var totalSize = File.ReadLines(fileName).Count() - 1;

            var batchSize = 2500;

            var prototypeObject = new T();

            var properties = prototypeObject.GetType().GetProperties();
            var headerNumberPropertyMatches = new List<(int columnNumber, PropertyInfo property)>();

            var currentCount = 1;
            var yieldReturnList = new List<T>();
            var startTime = DateTime.Now;
            var batchTime = DateTime.Now;

            bool YesNoBoolConversion(string toParse)
            {
                var trueList = new List<string> {"yes", "true", "y"};

                foreach (var loopTrueList in trueList)
                    if (toParse.IndexOf(loopTrueList, StringComparison.InvariantCultureIgnoreCase) >= 0)
                        return true;

                return false;
            }

            var accumulate = false;
            var accumulatedLine = string.Empty;

            var headerRow = true;

            foreach (var loopFileLine in File.ReadLines(fileName))
            {
                if (headerRow)
                {
                    var headers = loopFileLine.Split(',');

                    for (var i = 0; i < headers.Length; i++)
                        headerNumberPropertyMatches.Add((i, properties.Single(x => x.Name == headers[i])));

                    headerNumberPropertyMatches = headerNumberPropertyMatches.OrderBy(x => x.columnNumber).ToList();

                    headerRow = false;
                    continue;
                }

                var fileLine = loopFileLine;

                if (!fileLine.EndsWith("\""))
                {
                    accumulate = true;
                    accumulatedLine = accumulatedLine + fileLine + "\n";
                    totalSize -= 1;
                    continue;
                }

                if (accumulate)
                {
                    fileLine = accumulatedLine + fileLine;
                    accumulate = false;
                    accumulatedLine = string.Empty;
                }

                if (currentCount % batchSize == 0)
                {
                    var frozenNow = DateTime.Now;
                    var currentLoopTime = frozenNow.Subtract(batchTime);
                    var currentTotalTime = frozenNow.Subtract(startTime);
                    var remainingTimeEstimate =
                        new TimeSpan((totalSize - currentCount) * (currentTotalTime.Ticks / currentCount));
                    progress.Report(
                        $"{fileName} - {currentCount} of {totalSize} processed in {currentLoopTime.TotalSeconds} seconds - estimating {remainingTimeEstimate:hh\\:mm\\:ss} remaining.");

                    yield return yieldReturnList;

                    yieldReturnList = new List<T>();
                    batchTime = DateTime.Now;
                }

                var valueList = new List<string>();
                var remainingString = fileLine;

                while (remainingString.IndexOf("\",\"", StringComparison.Ordinal) > 0)
                {
                    var index = remainingString.IndexOf("\",\"", StringComparison.Ordinal);
                    var unwrappedValue = remainingString.Substring(1, index - 1);
                    valueList.Add(unwrappedValue);
                    remainingString = remainingString[(index + 2)..];
                }

                valueList.Add(remainingString[1..^1]);

                //Support for omitting the last comma if there is no last value
                if (valueList.Count == headerNumberPropertyMatches.Count - 1) valueList.Add(string.Empty);

                var newObject = new T();

                foreach (var loopProperties in headerNumberPropertyMatches)
                {
                    var relatedValue = valueList[loopProperties.Item1];

                    relatedValue = string.IsNullOrWhiteSpace(relatedValue) ? null : relatedValue.Trim();

                    if (loopProperties.property.PropertyType == typeof(string))
                        loopProperties.property.SetValue(newObject, relatedValue ?? string.Empty);
                    if (loopProperties.property.PropertyType == typeof(int))
                        loopProperties.property.SetValue(newObject, int.Parse(valueList[loopProperties.columnNumber]));
                    if (loopProperties.property.PropertyType == typeof(int?))
                        loopProperties.property.SetValue(newObject,
                            relatedValue == null ? null : int.Parse(valueList[loopProperties.Item1]));
                    if (loopProperties.property.PropertyType == typeof(DateTime))
                        loopProperties.property.SetValue(newObject, DateTime.Parse(valueList[loopProperties.Item1]));
                    if (loopProperties.property.PropertyType == typeof(DateTime?))
                        loopProperties.property.SetValue(newObject,
                            relatedValue == null ? null : DateTime.Parse(valueList[loopProperties.Item1]));
                    if (loopProperties.property.PropertyType == typeof(decimal))
                        loopProperties.property.SetValue(newObject,
                            decimal.Parse(valueList[loopProperties.columnNumber]));
                    if (loopProperties.property.PropertyType == typeof(decimal?))
                        loopProperties.property.SetValue(newObject,
                            relatedValue == null ? null : decimal.Parse(valueList[loopProperties.Item1]));
                    if (loopProperties.property.PropertyType == typeof(bool))
                        loopProperties.property.SetValue(newObject,
                            YesNoBoolConversion(valueList[loopProperties.columnNumber]));
                    if (loopProperties.property.PropertyType == typeof(bool?))
                        loopProperties.property.SetValue(newObject,
                            relatedValue == null ? null : YesNoBoolConversion(valueList[loopProperties.Item1]));
                }

                yieldReturnList.Add(newObject);
                currentCount++;
            }

            yield return yieldReturnList;
        }

        private static List<T> FileRecords<T>(string fileName, IProgress<string> progress) where T : class, new()
        {
            return FileRecordBatches<T>(fileName, progress).ToList().SelectMany(x => x).ToList();
        }

        public static async Task Import(string dbName, IProgress<string> progress, DirectoryInfo folder,
            string stateCodeFilter)
        {
            List<FileInfo> FileMatches(string primaryFileFilter)
            {
                var initialFiles = folder.EnumerateFiles().Where(x =>
                    x.Name.IndexOf(primaryFileFilter, StringComparison.InvariantCultureIgnoreCase) >= 0);
                return string.IsNullOrWhiteSpace(stateCodeFilter)
                    ? initialFiles.ToList()
                    : initialFiles.Where(x =>
                        x.Name.IndexOf(stateCodeFilter, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
            }

            progress.Report(
                $"Found {FileMatches(stateCodeFilter).Count} Files {(string.IsNullOrWhiteSpace(stateCodeFilter) ? string.Empty : $" - filtering for '{stateCodeFilter}'")}");

            var context = new GloDataContext(dbName);
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            var authorityFiles = FileMatches("Authority_Lookup.csv");

            var authorityRecords = new List<AuthorityLookupCsv>();

            authorityFiles.ForEach(x =>
                authorityRecords.AddRange(FileRecords<AuthorityLookupCsv>(x.FullName, progress)));

            context.AuthorityLookups.AddRange(authorityRecords.Select(x => new AuthorityLookup().InjectFrom(x))
                .Cast<AuthorityLookup>());

            await context.SaveChangesAsync(true);

            progress.Report($"Finished Authority_Lookup - {DateTime.Now}");

            context = new GloDataContext(dbName);
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            var countyLookupFiles = FileMatches("County_Lookup.csv");

            var countyLookupRecords = new List<CountyLookupCsv>();

            countyLookupFiles.ForEach(x =>
                countyLookupRecords.AddRange(FileRecords<CountyLookupCsv>(x.FullName, progress)));

            context.CountyLookups.AddRange(countyLookupRecords.Select(x => new CountyLookup().InjectFrom(x))
                .Cast<CountyLookup>());

            await context.SaveChangesAsync(true);

            progress.Report($"Finished County_Lookup - {DateTime.Now}");

            context = new GloDataContext(dbName);
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            var docClassLookupFiles = FileMatches("Doc_Class_Lookup.csv");

            var docClassLookupRecords = new List<DocumentClassLookupCsv>();

            docClassLookupFiles.ForEach(x =>
                docClassLookupRecords.AddRange(FileRecords<DocumentClassLookupCsv>(x.FullName, progress)));

            context.DocClassLookups.AddRange(docClassLookupRecords.Select(x => new DocumentClassLookup().InjectFrom(x))
                .Cast<DocumentClassLookup>());

            await context.SaveChangesAsync(true);

            progress.Report($"Finished Doc_Class - {DateTime.Now}");

            context = new GloDataContext(dbName);
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            var meridianLookupFiles = FileMatches("Meridian_Lookup.csv");

            var meridianLookupRecords = new List<MeridianLookupCsv>();

            meridianLookupFiles.ForEach(x =>
                meridianLookupRecords.AddRange(FileRecords<MeridianLookupCsv>(x.FullName, progress)));

            context.MeridianLookups.AddRange(meridianLookupRecords.Select(x => new MeridianLookup().InjectFrom(x))
                .Cast<MeridianLookup>());

            await context.SaveChangesAsync(true);

            progress.Report($"Finished Meridian_Lookup - {DateTime.Now}");

            var landDescriptionFiles = FileMatches("Land_Description.csv");

            foreach (var loopLandDescriptionFiles in landDescriptionFiles)
            foreach (var fileRecordBatch in FileRecordBatches<LandDescriptionCsv>(
                loopLandDescriptionFiles.FullName, progress))
            {
                context = new GloDataContext(dbName);
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                context.LandDescriptions.AddRange(fileRecordBatch.Select(x => new LandDescription().InjectFrom(x))
                    .Cast<LandDescription>());

                await context.SaveChangesAsync(true);
            }

            progress.Report($"Finished Land_Description - {DateTime.Now}");

            var patenteeFiles = FileMatches("Patentee.csv");

            foreach (var loopPatenteeFiles in patenteeFiles)
            foreach (var fileRecordBatch in FileRecordBatches<PatenteeCsv>(
                loopPatenteeFiles.FullName, progress))
            {
                context = new GloDataContext(dbName);
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                context.Patentees.AddRange(fileRecordBatch.Select(x => new Patentee().InjectFrom(x))
                    .Cast<Patentee>());

                await context.SaveChangesAsync(true);
            }

            progress.Report($"Finished Patentee - {DateTime.Now}");

            context = new GloDataContext(dbName);
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            var warranteeFiles = FileMatches("Warrantee.csv");

            var warranteeRecords = new List<WarranteeCsv>();

            warranteeFiles.ForEach(x => warranteeRecords.AddRange(FileRecords<WarranteeCsv>(x.FullName, progress)));

            context.Warrantees.AddRange(warranteeRecords.Select(x => new Warrantee().InjectFrom(x)).Cast<Warrantee>());

            await context.SaveChangesAsync(true);

            progress.Report($"Finished Warrantee - {DateTime.Now}");

            context = new GloDataContext(dbName);
            context.ChangeTracker.AutoDetectChangesEnabled = false;

            var landOfficeLookupFiles = FileMatches("Land_Office_Lookup.csv");

            var landOfficeLookupRecords = new List<LandOfficeLookupCsv>();

            landOfficeLookupFiles.ForEach(x =>
                landOfficeLookupRecords.AddRange(FileRecords<LandOfficeLookupCsv>(x.FullName, progress)));

            context.LandOfficeLookups.AddRange(landOfficeLookupRecords
                .Select(x => new LandOfficeLookup().InjectFrom(x)).Cast<LandOfficeLookup>());

            await context.SaveChangesAsync(true);

            progress.Report($"Finished Land_Office_Lookup - {DateTime.Now}");

            var patentFiles = FileMatches("Patent.csv");

            foreach (var loopPatentFiles in patentFiles)
            foreach (var fileRecordBatch in FileRecordBatches<PatentCsv>(
                loopPatentFiles.FullName, progress))
            {
                context = new GloDataContext(dbName);
                context.ChangeTracker.AutoDetectChangesEnabled = false;

                context.Patents.AddRange(fileRecordBatch.Select(x => new Patent().InjectFrom(x))
                    .Cast<Patent>());

                await context.SaveChangesAsync(true);
            }

            progress.Report($"Finished Patent - {DateTime.Now}");
        }

        public static LandDescription LandDescriptionCsvToDbRecord(LandDescriptionCsv toTransform, string stateDataFileCode)
        {
            return new LandDescription()
            {
                AccessionNumber = toTransform.accession_nr,
                AliquotParts = toTransform.aliquot_parts,
                BlockNumber = toTransform.block_nr,
                DescriptionNumber = toTransform.descrip_nr,
                DocumentClassCode = toTransform.doc_class_code,
                FractionalSection = toTransform.fractional_section,
                LdRemarks = toTransform.ld_remarks,
                MeridianCode = toTransform.meridian_code,
                RangeDir = toTransform.range_dir,
                RangeNumber = toTransform.range_nr,
                SectionNumber = toTransform.section_nr,
                StateCode = toTransform.state_code,
                SurveyNumber = toTransform.survey_nr,
                TownshipDir = toTransform.township_dir,
                TownshipNumber = toTransform.township_nr,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<LandDescriptionCsv> LandDescriptionCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Land_Description Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<LandDescriptionCsv>(fileName, progress).ToList();

            progress.Report($"Finished Land_Description Csv Import - {DateTime.Now}");

            return fileRecords;
        }

        public static LandOfficeLookup DocumentClassLookupCsvToDbRecord(LandOfficeLookupCsv toTransform, string stateDataFileCode)
        {
            return new LandOfficeLookup()
            {
                LandOfficeCode = toTransform.l_o_code,
                LandOfficeDescription = toTransform.l_o_description,
                StateCode = toTransform.state_code,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<LandOfficeLookupCsv> LandOfficeLookupCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Land_Office_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<LandOfficeLookupCsv>(fileName, progress).ToList();

            progress.Report($"Finished Land_Office_Lookup Csv Import - {DateTime.Now}");

            return fileRecords;
        }
        
        public static MeridianLookup MeridianLookupCsvToDbRecord(MeridianLookupCsv toTransform, string stateDataFileCode)
        {
            return new MeridianLookup()
            {
                MeridianCode = toTransform.meridian_code,
                MeridianName = toTransform.meridian_name,
                StateCode = toTransform.state_code,
                StateDefault = toTransform.state_default,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<MeridianLookupCsv> MeridianLookupCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Meridian_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<MeridianLookupCsv>(fileName, progress).ToList();

            progress.Report($"Finished Meridian_Lookup Csv Import - {DateTime.Now}");

            return fileRecords;
        }

        public static Patent PatentCsvToDbRecord(PatentCsv toTransform, string stateDataFileCode)
        {
            return new Patent()
            {
                AccessionNumber = toTransform.accession_nr,
                AltAccessionNumber = toTransform.alt_accession_nr,
                AuthorityCode = toTransform.authority_code,
                BlmSerialNumber = toTransform.blm_serial_nr,
                CancelledDocument = toTransform.cancelled_doc,
                CertificateOfLocation = toTransform.certificate_of_location,
                CoalEntryNumber = toTransform.coal_entry_nr,
                DocumentClassCode = toTransform.doc_class_code,
                DocumentNumber = toTransform.document_nr,
                GeographicName = toTransform.geographic_name,
                ImagePageNumber = toTransform.image_page_nr,
                IndianAllotmentNumber = toTransform.indian_allotment_nr,
                LOCode = toTransform.l_o_code,
                MetesBounds = toTransform.metes_bounds,
                MilitaryRank = toTransform.military_rank,
                Militia = toTransform.militia,
                MiscellaneousDocumentNumber = toTransform.misc_document_nr,
                Remarks = toTransform.remarks,
                SignatureDate = toTransform.signature_date,
                SignaturePresent = toTransform.signature_present,
                StateCode = toTransform.state_code,
                StateInFavorOf = toTransform.state_in_favor_of,
                SubsurfaceReserved = toTransform.subsurface_reserved,
                SupremeCourtScriptNumber = toTransform.supreme_court_script_nr,
                SurveyDate = toTransform.survey_date,
                TotalAcres = toTransform.total_acres,
                Tribe = toTransform.tribe,
                UsReservations = toTransform.us_reservations,
                VerifyFlag = toTransform.verify_flag,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<PatentCsv> PatentCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Patent Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<PatentCsv>(fileName, progress).ToList();

            progress.Report($"Finished Patent Csv Import - {DateTime.Now}");

            return fileRecords;
        }

        public static Patentee PatenteeCsvToDbRecord(PatenteeCsv toTransform, string stateDataFileCode)
        {
            return new Patentee()
            {
                AccessionNumber = toTransform.accession_nr,
                DocumentClassCode = toTransform.doc_class_code,
                PatenteeFirstName = toTransform.patentee_first_name,
                PatenteeLastName = toTransform.patentee_last_name,
                PatenteeMiddleName = toTransform.patentee_middle_name,
                PatenteeSequenceNumber = toTransform.patentee_seq_nr,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<PatenteeCsv> PatenteeCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Patentee Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<PatenteeCsv>(fileName, progress).ToList();

            progress.Report($"Finished Patentee Csv Import - {DateTime.Now}");

            return fileRecords;
        }

        public static Warrantee WarranteeCsvToDbRecord(WarranteeCsv toTransform, string stateDataFileCode)
        {
            return new Warrantee()
            {
                AccessionNumber = toTransform.accession_nr,
                DocumentClassCode = toTransform.doc_class_code,
                WarranteeFirstName = toTransform.warrantee_first_name,
                WarranteeLastName = toTransform.warrantee_last_name,
                WarranteeMiddleName = toTransform.warrantee_middle_name,
                WarranteeSequenceNumber = toTransform.warrantee_seq_nr,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<WarranteeCsv> WarranteeCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Warrantee Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<WarranteeCsv>(fileName, progress).ToList();

            progress.Report($"Finished Warrantee Csv Import - {DateTime.Now}");

            return fileRecords;
        }
    }
}