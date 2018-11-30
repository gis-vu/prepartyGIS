using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DotSpatial.Data;
using DotSpatial.Projections;
using Helpers;
using Models;

namespace PrepartyGIS
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var grid = GetGrid(@"C:\Users\daini\Documents\ArcGIS\Data\v2\fishnet3.shp");

            var index = 103;

            var cellData = GetCellData(index, "C:\\Users\\daini\\Documents\\ArcGIS\\Data\\v2\\WIP\\103\\", grid);

            //var data = FeaturesToGeojsonHelper.ToGeojson(cellData.BorderFeatures
            //    .Select(x => x.Data.Coordinates.Select(y => y.ToDoubleArray()).ToDoubleArray()).ToDoubleArray());

            Save(cellData, @"C:\Users\daini\Desktop\GIS\Duomenys\WIP\102TESTDATA\" + index + ".txt");
            Save(grid, @"C:\Users\daini\Desktop\GIS\Duomenys\WIP\102TESTDATA\grid.txt");
        }

        private static CellData GetCellData(int index, string diretoryPath, GridCell[] grid)
        {
            var features = ReadFeatures(diretoryPath + index + "_okV3.shp");

            UpdateNeighbours(features);

            var borderFeatures =
                GetBorderFeatures(features, grid.First(x => x.Index == index.ToString()).Border);

            return new CellData
            {
                BorderFeatures = borderFeatures,
                Features = features
            };
        }

        private static GridCell[] GetGrid(string path)
        {
            var sf = Shapefile.OpenFile(path);
            sf.Reproject(KnownCoordinateSystems.Geographic.World.WGS1984);

            var columns = GetColumns(sf);

            return (from feature in sf.Features
                let attributes = GetAttributes(feature, columns)
                select new GridCell
                {
                    Border = feature.Coordinates.Select(t => new PointPosition {Latitude = t.Y, Longitude = t.X})
                        .ToArray(),
                    Index = attributes["TEXTID"].ToString()
                }).ToArray();
        }

        
        private static RouteFeature[] GetBorderFeatures(RouteFeature[] features, PointPosition[] border)
        {
            return features.Where(f =>
                DistanceHelpers.SplitFeatureIntoLineSegments(border.Select(x => x.ToDoubleArray()).ToArray()).Any(x =>
                    DistanceHelpers.IsCloseToLine(x, f.Data.Coordinates.First().ToDoubleArray()) ||
                    DistanceHelpers.IsCloseToLine(x, f.Data.Coordinates.Last().ToDoubleArray()))).ToArray();
        }

        private static RouteFeature[] ReadFeatures(string path)
        {
            var sf = Shapefile.OpenFile(path);
            sf.Reproject(KnownCoordinateSystems.Geographic.World.WGS1984);

            var features = new List<RouteFeature>();

            var columns = GetColumns(sf);

            foreach (var feature in sf.Features)
            {
                var f = new RouteFeature();
                f.Data.Coordinates = feature.Coordinates.Select(t => new PointPosition
                {
                    Latitude = t.Y,
                    Longitude = t.X
                }).ToArray();

                f.Data.Properties = GetAttributes(feature, columns);
                features.Add(f);
            }

            return features.ToArray();
        }

        private static void Save(object data, string path)
        {
            var formatter = new BinaryFormatter();

            using (var fileStream = new FileStream(path, FileMode.Create))
            {
                formatter.Serialize(fileStream, data);
            }
        }

        private static void UpdateNeighbours(RouteFeature[] features)
        {
            Console.WriteLine("Updating feature neigbours");

            double amount = 0, temp = 0, all = features.Length;

            for (var i = 0; i < features.Length - 1; i++)
            {
                amount++;
                temp++;
                if (temp > all / 100)
                {
                    Console.WriteLine(Math.Round(amount / all * 100, 2));
                    temp = 0;
                }

                for (var j = i + 1; j < features.Length; j++)
                    if (DistanceHelpers.AreNeighbours(features[i].Data.Coordinates.Select(x=>x.ToDoubleArray()).ToArray(), features[j].Data.Coordinates.Select(x=>x.ToDoubleArray()).ToArray()))
                    {
                        features[i].Neighbours.Add(features[j]);
                        features[j].Neighbours.Add(features[i]);
                    }
            }
        }

        private static IDictionary<string, object> GetAttributes(IFeature feature, string[] columns)
        {
            var attributes = new Dictionary<string, dynamic>();

            for (var i = 0; i < feature.DataRow.ItemArray.Length; i++)
                attributes[columns[i]] = feature.DataRow.ItemArray[i];

            return attributes;
        }

        private static string[] GetColumns(Shapefile sf)
        {
            var columns = new List<string>();

            foreach (var c in sf.DataTable.Columns) columns.Add(c.ToString());

            return columns.ToArray();
        }
    }
}