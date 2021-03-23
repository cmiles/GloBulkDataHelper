using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ProjNet.CoordinateSystems;
using ProjNet.CoordinateSystems.Transformations;

// ReSharper disable InconsistentNaming

namespace GloBulkDataHelper.BlmNavigatorService
{
    //https://github.com/NetTopologySuite/ProjNet4GeoAPI/wiki/Loading-a-projection-by-Spatial-Reference-ID
    //
    //3/22/2021 - The SRID.csv is available in GitHub from ProjNet4GeoAPI/test/ProjNet.Tests/ - the tests
    //in ProjNet4GeoAPI/test/ProjNet.Tests/WKT/PostGisSpatialRefSysTableParserTest.cs/ show that this file
    //can be generated from PostGIS:
    //
    //[Test]//, Ignore("Only run this if you want a new SRID.csv file")]
    //public void TestCreateSridCsv()
    //{
    //    if (string.IsNullOrWhiteSpace(ConnectionString))
    //        throw new IgnoreException("No Connection string provided or provided connection string invalid.");

    //    if (File.Exists("SRID.csv")) File.Delete("SRID.csv");

    //    using (var sw = new StreamWriter(File.OpenWrite("SRID.csv")))
    //    using (var cn = new NpgsqlConnection(ConnectionString))
    //    {
    //        cn.Open();
    //        var cm = cn.CreateCommand();
    //        cm.CommandText = "SELECT \"srid\", \"srtext\" FROM \"public\".\"spatial_ref_sys\" ORDER BY srid;";
    //        using (var dr = cm.ExecuteReader(CommandBehavior.SequentialAccess))
    //        {
    //            while (dr.Read())
    //            {
    //                int srid = dr.GetInt32(0);
    //                string srtext = dr.GetString(1);
    //                int bracketIndex = srtext.IndexOf('[');
    //                if (bracketIndex < 0)
    //                {
    //                    continue;
    //                }

    //                switch (srtext.Substring(0, bracketIndex))
    //                {
    //                    case "PROJCS":
    //                    case "GEOGCS":
    //                    case "GEOCCS":
    //                        sw.WriteLine($"{srid};{srtext}");
    //                        break;
    //                }
    //            }
    //        }
    //        cm.Dispose();
    //    }
    //}

    public static class CoordinateHelpers
    {
        /// <summary>Gets a coordinate system from the SRID.csv file</summary>
        /// <param name="id">EPSG ID</param>
        /// <returns>Coordinate system, or null if SRID was not found.</returns>
        public static CoordinateSystem CoordinateSystemFromSrid(int id)
        {
            return (from wkt in SridWktFromReferenceFile()
                where wkt.WKID == id
                let x = new CoordinateSystemFactory()
                select x.CreateFromWkt(wkt.WKT)).FirstOrDefault();
        }

        public static ICoordinateTransformation CoordinateTransform(int fromSrid, int toSrid)
        {
            return new CoordinateTransformationFactory().CreateFromCoordinateSystems(CoordinateSystemFromSrid(fromSrid),
                CoordinateSystemFromSrid(toSrid));
        }

        public static ICoordinateTransformation CoordinateTransformTo4326(int fromSrid)
        {
            return new CoordinateTransformationFactory().CreateFromCoordinateSystems(CoordinateSystemFromSrid(fromSrid),
                CoordinateSystemFromSrid(4326));
        }

        /// <summary>Enumerates all SRIDs in the SRID.csv file.</summary>
        /// <returns>Enumerator</returns>
        private static IEnumerable<WktString> SridWktFromReferenceFile()
        {
            using var sr = File.OpenText(Path.Combine(AppContext.BaseDirectory, "SRID.csv"));
            while (!sr.EndOfStream)
            {
                var line = sr.ReadLine() ?? string.Empty;
                var split = line.IndexOf(';');
                if (split <= -1) continue;

                var wkt = new WktString
                {
                    WKID = int.Parse(line[..split]), WKT = line[(split + 1)..]
                };
                yield return wkt;
            }

            sr.Close();
        }

        public struct WktString
        {
            /// <summary>Well-known ID</summary>
            public int WKID;

            /// <summary>Well-known Text</summary>
            public string WKT;
        }
    }
}