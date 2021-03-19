using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using FluentResults;
using GloDb.FindLdCache;
using GloDb.GloData;
using NetTopologySuite.Geometries;

namespace GloDb.Queries
{
    public static class GloDataQuery
    {
        public static Result<(bool inCurrentCache, FindLdCache.FindLdCache cacheEntry)> CacheFromPlssDescription(
            this FindLdCacheContext cacheContext, string plssDescription, IProgress<string> progress)
        {
            if (string.IsNullOrWhiteSpace(plssDescription))
                return new Result<(bool inCurrentCache, FindLdCache.FindLdCache cacheEntry)>().WithError(
                    "No valid land description provided?");

            var ownershipAreaResultReturn = FindLdRequest.FindLd(plssDescription);

            if (ownershipAreaResultReturn.IsFailed)
            {
                cacheContext.FindLdCacheErrorLog.Add(new FindLdCacheErrorLog
                {
                    ErrorMessage = string.Join(", ", ownershipAreaResultReturn.Errors.Select(x => x.Message)),
                    PlssIdentifier = plssDescription,
                    ErroredOn = DateTime.Now.ToUniversalTime()
                });

                progress.Report(
                    $"FindLd Boundary Lookup Failed - {string.Join(", ", ownershipAreaResultReturn.Errors.Select(x => x.Message))}");

                return new Result<(bool inCurrentCache, FindLdCache.FindLdCache cacheEntry)>().WithErrors(
                    ownershipAreaResultReturn
                        .Errors);
            }

            var ownershipAreaValue = ownershipAreaResultReturn.Value;

            var rings = ownershipAreaValue.features.SelectMany(x => x.geometry.rings).ToList();

            progress.Report($"Ring Count for {plssDescription}: {rings.Count}");

            var toAdd = new FindLdCache.FindLdCache
                {CachedOn = DateTime.Now.ToUniversalTime(), PlssIdentifier = plssDescription};

            foreach (var loopPoints in rings)
            {
                var coordinateList = new List<Coordinate>();

                foreach (var loopRings in loopPoints)
                {
                    coordinateList.Add(new Coordinate(loopRings[0], loopRings[1]));
                    progress.Report($"Ring for {plssDescription}: Added {loopRings[0]} {loopRings[1]}");
                }

                if (coordinateList.First().X != coordinateList.Last().X ||
                    coordinateList.First().Y != coordinateList.Last().Y)
                {
                    coordinateList.Add(coordinateList.First());
                    progress.Report($"Ring for {plssDescription}: Added {coordinateList[0].X} {coordinateList[0].Y}");
                }

                var geomFactory = new GeometryFactory(new PrecisionModel(), 4326);
                var newPolygon = geomFactory.CreatePolygon(coordinateList.ToArray());

                toAdd.OwnershipArea = newPolygon;
            }

            return new Result<(bool inCurrentCache, FindLdCache.FindLdCache cacheEntry)>().WithValue((false, toAdd));
        }

        public static List<GloDataQueryResult.GloDataQueryResult> FromAccessionNumbers(this GloDataContext context,
            FindLdCacheContext cacheContext, List<string> accessionList,
            IProgress<string> progress)
        {
            progress.Report($"Accession Number Lookups - {accessionList.Count}");

            var returnList = new List<GloDataQueryResult.GloDataQueryResult>();
            var lookupProcessed = 1;

            foreach (var loopAccession in accessionList)
            {
                var landDescriptions = context.LandDescriptions.Where(x => x.AccessionNumber == loopAccession).ToList();

                returnList.AddRange(context.FromLandDescriptions(cacheContext, landDescriptions, progress));

                progress.Report(
                    $"Accession Number {loopAccession} Processed - {lookupProcessed} of {accessionList.Count}");
                lookupProcessed++;
            }

            return returnList;
        }

        public static List<GloDataQueryResult.GloDataQueryResult> FromLandDescriptions(this GloDataContext context,
            FindLdCacheContext cacheContext,
            List<LandDescription> landDescriptions, IProgress<string> progress)
        {
            var returnList = new List<GloDataQueryResult.GloDataQueryResult>();
            var ldProcessed = 1;

            var ownershipAreaCache = cacheContext.PopulateCacheFromLandDescriptions(landDescriptions, progress);

            var accessionList = landDescriptions.Select(x => x.AccessionNumber).Distinct().ToList();

            var accessionCache = context.Patents.Where(x => accessionList.Contains(x.AccessionNumber)).ToList();
            var patenteesCache = context.Patentees.Where(x => accessionList.Contains(x.AccessionNumber)).ToList();

            foreach (var loopLand in landDescriptions)
            {
                progress.Report("Starting next loop");
                var toAdd = new GloDataQueryResult.GloDataQueryResult {AccessionNumber = loopLand.AccessionNumber};

                var patentees = patenteesCache.Where(x => x.AccessionNumber == loopLand.AccessionNumber).ToList();

                progress.Report($"{patentees.Count} Patentees");

                toAdd.Patentee = string.Join(";",
                    patentees.Select(x =>
                        $"{x.PatenteeLastName}, {x.PatenteeFirstName} {x.PatenteeMiddleName}".Trim()));

                toAdd.PlssDescription = FindLdRequest.PlssString(loopLand);

                var possibleOwnershipArea =
                    ownershipAreaCache.SingleOrDefault(x => x.PlssIdentifier == toAdd.PlssDescription);

                if (possibleOwnershipArea != null) toAdd.OwnershipArea = possibleOwnershipArea.OwnershipArea;

                var patent = accessionCache.SingleOrDefault(x => x.AccessionNumber == loopLand.AccessionNumber);

                if (patent != null)
                    toAdd.PatentYear = patent.SignatureDate?.Date;
                else
                    toAdd.ErrorLog += "No Patent Found";

                progress.Report(
                    $"Land Description {loopLand.Id} - {ldProcessed} of {landDescriptions.Count} Processed");

                returnList.Add(toAdd);

                ldProcessed++;
            }

            return returnList;
        }

        public static List<GloDataQueryResult.GloDataQueryResult> LookupByMeridianTownshipRange(
            this GloDataContext context, FindLdCacheContext cacheContext, int meridian,
            int minimumTownship, int maximumTownship, string townshipDirection, int minimumRange, int maximumRange,
            string rangeDirection, IProgress<string> progress)
        {
            var landDescriptions = context.LandDescriptions.Where(x =>
                    x.MeridianCode == meridian && x.TownshipNumber != null && x.TownshipNumber >= minimumTownship &&
                    x.TownshipNumber <= maximumTownship && x.TownshipDirection == townshipDirection &&
                    x.RangeNumber != null &&
                    x.RangeNumber >= minimumRange && x.RangeNumber <= maximumRange &&
                    x.RangeDirection == rangeDirection)
                .ToList();

            if (!landDescriptions.Any()) return new List<GloDataQueryResult.GloDataQueryResult>();

            progress.Report($"Meridian, Township, Range Lookup found {landDescriptions.Count} Land Descriptions");

            return context.FromLandDescriptions(cacheContext, landDescriptions, progress);
        }

        public static List<GloDataQueryResult.GloDataQueryResult> LookupByPatentee(this GloDataContext context,
            FindLdCacheContext cacheContext,
            Expression<Func<Patentee, bool>> patenteeFilter, IProgress<string> progress)
        {
            var patentees = context.Patentees.Where(patenteeFilter).ToList();

            if (!patentees.Any()) return new List<GloDataQueryResult.GloDataQueryResult>();

            var accessionNumberList = patentees.GroupBy(x => x.AccessionNumber).Select(x => x.Key).ToList();

            progress.Report(
                $"Patentee Lookup found {patentees.Count} Patentees with {accessionNumberList.Count} unique Accession Numbers");

            return context.FromAccessionNumbers(cacheContext, accessionNumberList, progress);
        }

        public static List<FindLdCache.FindLdCache> PopulateCacheFromLandDescriptions(this FindLdCacheContext context,
            List<LandDescription> landDescriptions,
            IProgress<string> progress)
        {
            var allPlssIdentifierList = landDescriptions.Select(FindLdRequest.PlssString).Distinct().ToList();

            var returnList = new List<(bool, FindLdCache.FindLdCache)>();

            foreach (var loopCacheCheckGroups in allPlssIdentifierList.Partition(500))
            {
                var existingItems = context.FindLdCache.Where(x => loopCacheCheckGroups.Contains(x.PlssIdentifier))
                    .ToList();
                returnList.AddRange(existingItems.Select(x => (true, x)));
            }

            var plssIdentifiersNotCached =
                allPlssIdentifierList.Except(returnList.Select(x => x.Item2.PlssIdentifier)).ToList();

            var loopCounter = 1;

            foreach (var toAdd in plssIdentifiersNotCached)
            {
                progress.Report($"Caching List - plss {toAdd} - {loopCounter} of {plssIdentifiersNotCached.Count}");

                var lookupResultReturn = context.CacheFromPlssDescription(toAdd, progress);

                if (lookupResultReturn.IsFailed)
                {
                    loopCounter++;
                    continue;
                }

                var lookupResult = lookupResultReturn.Value;

                returnList.Add(lookupResult);

                loopCounter++;
            }

            var toInsert = returnList.Where(x => !x.Item1).Select(x => x.Item2);

            context.FindLdCache.AddRange(toInsert);

            context.SaveChanges(true);

            return returnList.Select(x => x.Item2).ToList();
        }
    }
}