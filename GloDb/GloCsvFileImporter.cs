using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using GloDb.GloCsvFile;
using GloDb.GloData;

namespace GloDb
{
    public static class GloCsvFileImporter
    {
        public static List<AuthorityLookupCsv> AuthorityLookupCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Authority_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<AuthorityLookupCsv>(fileName, progress).ToList();

            progress.Report(
                $"Finished Authority_Lookup Csv Import - {fileRecords.Count} records found - {DateTime.Now}");

            return fileRecords;
        }

        public static AuthorityLookup AuthorityLookupCsvToDbRecord(AuthorityLookupCsv toTransform,
            string stateDataFileCode)
        {
            return new()
            {
                ActTreaty = toTransform.act_treaty,
                AuthorityCode = toTransform.authority_code,
                EntryClass = toTransform.entry_class,
                StateDataFile = stateDataFileCode,
                StatutoryReference = toTransform.statutory_ref
            };
        }

        public static async Task AuthorityLookupToDb(List<AuthorityLookupCsv> toImport, string stateDataFileCode,
            string dbName, IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => AuthorityLookupCsvToDbRecord(x, stateDataFileCode)).ToList()
                .Partition(2500).ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report(
                    $"Authority Lookup DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.AuthorityLookups.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
        }

        public static List<CountyCsv> CountyCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting County Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<CountyCsv>(fileName, progress).ToList();

            progress.Report($"Finished County Csv Import - {fileRecords.Count} records found -  {DateTime.Now}");

            return fileRecords;
        }

        public static County CountyCsvToDbRecord(CountyCsv toTransform, string stateDataFileCode)
        {
            return new()
            {
                AccessionNumber = toTransform.accession_nr,
                CountyCode = toTransform.county_code,
                DescriptionNumber = toTransform.descrip_nr,
                DocumentClassCode = toTransform.doc_class_code,
                StateCode = toTransform.state_code,
                StateDataFile = stateDataFileCode
            };
        }

        public static List<CountyLookupCsv> CountyLookupCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting County_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<CountyLookupCsv>(fileName, progress).ToList();

            progress.Report($"Finished County_Lookup Csv Import - {fileRecords.Count} records found -  {DateTime.Now}");

            return fileRecords;
        }

        public static CountyLookup CountyLookupCsvToDbRecord(CountyLookupCsv toTransform, string stateDataFileCode)
        {
            return new()
            {
                CountyCode = toTransform.county_code,
                CountyName = toTransform.county_name,
                StateCode = toTransform.state_code,
                StateDataFile = stateDataFileCode
            };
        }

        public static async Task CountyLookupToDb(List<CountyLookupCsv> toImport, string stateDataFileCode,
            string dbName, IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => CountyLookupCsvToDbRecord(x, stateDataFileCode)).ToList()
                .Partition(2500).ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report($"County Lookup DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.CountyLookups.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
        }

        public static async Task CountyToDb(List<CountyCsv> toImport, string stateDataFileCode, string dbName,
            IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => CountyCsvToDbRecord(x, stateDataFileCode)).ToList().Partition(2500)
                .ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report($"County DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.Counties.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
        }

        public static List<DocumentClassLookupCsv> DocumentClassLookupCsvRecords(string fileName,
            IProgress<string> progress)
        {
            progress.Report($"Starting Doc_Class_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<DocumentClassLookupCsv>(fileName, progress).ToList();

            progress.Report(
                $"Finished Doc_Class_Lookup Csv Import - {fileRecords.Count} records found -  {DateTime.Now}");

            return fileRecords;
        }

        public static DocumentClassLookup DocumentClassLookupCsvToDbRecord(DocumentClassLookupCsv toTransform,
            string stateDataFileCode)
        {
            return new()
            {
                DocumentClassCode = toTransform.doc_class_code,
                DocumentClassDescription = toTransform.document_class_description,
                DocumentClassDisplayName = toTransform.doc_class_display_name,
                StateDataFile = stateDataFileCode
            };
        }

        public static async Task DocumentClassLookupToDb(List<DocumentClassLookupCsv> toImport,
            string stateDataFileCode, string dbName, IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => DocumentClassLookupCsvToDbRecord(x, stateDataFileCode)).ToList()
                .Partition(2500).ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report(
                    $"Document Class Lookup DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.DocumentClassLookups.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
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

        public static List<LandDescriptionCsv> LandDescriptionCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Land_Description Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<LandDescriptionCsv>(fileName, progress).ToList();

            progress.Report(
                $"Finished Land_Description Csv Import - {fileRecords.Count} records found -  {DateTime.Now}");

            return fileRecords;
        }

        public static LandDescription LandDescriptionCsvToDbRecord(LandDescriptionCsv toTransform,
            string stateDataFileCode)
        {
            return new()
            {
                AccessionNumber = toTransform.accession_nr,
                AliquotParts = toTransform.aliquot_parts,
                BlockNumber = toTransform.block_nr,
                DescriptionNumber = toTransform.descrip_nr,
                DocumentClassCode = toTransform.doc_class_code,
                FractionalSection = toTransform.fractional_section,
                LandDescriptionRemarks = toTransform.ld_remarks,
                MeridianCode = toTransform.meridian_code,
                RangeDirection = toTransform.range_dir,
                RangeNumber = toTransform.range_nr,
                SectionNumber = toTransform.section_nr,
                StateCode = toTransform.state_code,
                SurveyNumber = toTransform.survey_nr,
                TownshipDirection = toTransform.township_dir,
                TownshipNumber = toTransform.township_nr,
                StateDataFile = stateDataFileCode
            };
        }

        public static async Task LandDescriptionToDb(List<LandDescriptionCsv> toImport, string stateDataFileCode,
            string dbName, IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => LandDescriptionCsvToDbRecord(x, stateDataFileCode)).ToList()
                .Partition(2500).ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report(
                    $"Land Description DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.LandDescriptions.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
        }

        public static List<LandOfficeLookupCsv> LandOfficeLookupCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Land_Office_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<LandOfficeLookupCsv>(fileName, progress).ToList();

            progress.Report(
                $"Finished Land_Office_Lookup Csv Import - {fileRecords.Count} records found -  {DateTime.Now}");

            return fileRecords;
        }

        public static LandOfficeLookup LandOfficeLookupCsvToDbRecord(LandOfficeLookupCsv toTransform,
            string stateDataFileCode)
        {
            return new()
            {
                LandOfficeCode = toTransform.l_o_code,
                LandOfficeDescription = toTransform.l_o_description,
                StateCode = toTransform.state_code,
                StateDataFile = stateDataFileCode
            };
        }

        public static async Task LandOfficeLookupToDb(List<LandOfficeLookupCsv> toImport, string stateDataFileCode,
            string dbName, IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => LandOfficeLookupCsvToDbRecord(x, stateDataFileCode)).ToList()
                .Partition(2500).ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report(
                    $"Land Office Lookup DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.LandOfficeLookups.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
        }

        public static List<MeridianLookupCsv> MeridianLookupCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Meridian_Lookup Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<MeridianLookupCsv>(fileName, progress).ToList();

            progress.Report(
                $"Finished Meridian_Lookup Csv Import - {fileRecords.Count} records found -  {DateTime.Now}");

            return fileRecords;
        }

        public static MeridianLookup MeridianLookupCsvToDbRecord(MeridianLookupCsv toTransform,
            string stateDataFileCode)
        {
            return new()
            {
                MeridianCode = toTransform.meridian_code,
                MeridianName = toTransform.meridian_name,
                StateCode = toTransform.state_code,
                StateDefault = toTransform.state_default,
                StateDataFile = stateDataFileCode
            };
        }

        public static async Task MeridianLookupToDb(List<MeridianLookupCsv> toImport, string stateDataFileCode,
            string dbName, IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => MeridianLookupCsvToDbRecord(x, stateDataFileCode)).ToList()
                .Partition(2500).ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report(
                    $"Meridian Lookup DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.MeridianLookups.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
        }

        public static List<PatentCsv> PatentCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Patent Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<PatentCsv>(fileName, progress).ToList();

            progress.Report($"Finished Patent Csv Import - {fileRecords.Count} records found -  {DateTime.Now}");

            return fileRecords;
        }

        public static Patent PatentCsvToDbRecord(PatentCsv toTransform, string stateDataFileCode)
        {
            return new()
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
                LandOfficeCode = toTransform.l_o_code,
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

        public static List<PatenteeCsv> PatenteeCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Patentee Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<PatenteeCsv>(fileName, progress).ToList();

            progress.Report($"Finished Patentee Csv Import - {fileRecords.Count} records found -  {DateTime.Now}");

            return fileRecords;
        }

        public static Patentee PatenteeCsvToDbRecord(PatenteeCsv toTransform, string stateDataFileCode)
        {
            return new()
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

        public static async Task PatenteeToDb(List<PatenteeCsv> toImport, string stateDataFileCode, string dbName,
            IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => PatenteeCsvToDbRecord(x, stateDataFileCode)).ToList()
                .Partition(2500).ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report($"Patentee DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.Patentees.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
        }

        public static async Task PatentToDb(List<PatentCsv> toImport, string stateDataFileCode, string dbName,
            IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => PatentCsvToDbRecord(x, stateDataFileCode)).ToList().Partition(2500)
                .ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report($"Patent DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.Patents.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
        }

        public static List<WarranteeCsv> WarranteeCsvRecords(string fileName, IProgress<string> progress)
        {
            progress.Report($"Starting Warrantee Csv Import - {DateTime.Now}");

            var fileRecords = FileRecords<WarranteeCsv>(fileName, progress).ToList();

            progress.Report($"Finished Warrantee Csv Import - {fileRecords.Count} records found -  {DateTime.Now}");

            return fileRecords;
        }

        public static Warrantee WarranteeCsvToDbRecord(WarranteeCsv toTransform, string stateDataFileCode)
        {
            return new()
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

        public static async Task WarranteeToDb(List<WarranteeCsv> toImport, string stateDataFileCode, string dbName,
            IProgress<string> progress)
        {
            var importBatches = toImport.Select(x => WarranteeCsvToDbRecord(x, stateDataFileCode)).ToList()
                .Partition(2500).ToList();

            var batchCounter = 1;

            foreach (var importBatch in importBatches)
            {
                progress.Report($"Warrantee DB Import - Importing Batch {batchCounter++} of {importBatches.Count}");

                var context = new GloDataContext(dbName);

                context.Warrantees.AddRange(importBatch);

                await context.SaveChangesAsync(true);
            }
        }
    }
}