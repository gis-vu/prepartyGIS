using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using DotSpatial.Data;
using DotSpatial.Projections;
using DTO;
using Helpers;

namespace PrepartyGIS
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var index = 102;

            
            var features = ReadFeatures($"C:\\Users\\daini\\Documents\\ArcGIS\\Data\\v2\\WIP\\102\\{index}.shp");
            UpdateNeighbours(features);


            var gridElement = ReadDataGridElement(@"C:\Users\daini\Documents\ArcGIS\Data\v2\fishnet3.shp", index, features);

            //var data = FeaturesToGeojsonHelper.ToGeojson(gridElement.BorderFeatures);

            Save(gridElement, @"C:\Users\daini\Desktop\GIS\Duomenys\WIP\102TESTDATA\readydata.txt");
        }

        private static GridElement ReadDataGridElement(string path, int index, RouteFeature[] features)
        {
            var sf = Shapefile.OpenFile(path);
            sf.Reproject(KnownCoordinateSystems.Geographic.World.WGS1984);

            var dataGridElement = new GridElement();

            dataGridElement.Features = features;

            var columns = GetColumns(sf);

            foreach (var feature in sf.Features)
            {
                var attributes = GetAttributes(feature, columns);

                if (attributes["TEXTID"].ToString() == index.ToString())
                {
                    dataGridElement.Border = feature.Coordinates.Select(t => new PointPosition()
                    {
                        Latitude = t.Y,
                        Longitude = t.X
                    }).ToArray();
                    break;
                }
            }

            if (dataGridElement.Border == null)
                throw new Exception("SMTH went wrong");

            dataGridElement.BorderFeatures = GetBorderFeatures(dataGridElement);

            return dataGridElement;
        }

        private static RouteFeature[] GetBorderFeatures(GridElement gridElement)
        {
            var borderFeatures = new List<RouteFeature>();

            foreach (var f in gridElement.Features)
            {
                if (DistanceHelpers.SplitFeatureIntoLineSegments(gridElement.Border).Any(x=> DistanceHelpers.GetDistance(x, f.Data.Coordinates.First()) < DistanceHelpers.DistanceDiff || DistanceHelpers.GetDistance(x, f.Data.Coordinates.Last()) < DistanceHelpers.DistanceDiff))
                {
                    borderFeatures.Add(f);
                }
            }

            return borderFeatures.ToArray();
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
                f.Data.Coordinates = feature.Coordinates.Select(t => new PointPosition()
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
            Console.WriteLine("Started import");

            double amount = 0,temp = 0, all = features.Length;

            for (int i = 0; i < features.Length - 1; i++)
            {
                amount++;
                temp++;
                if (temp > all / 100)
                {
                    Console.WriteLine(Math.Round(amount / all * 100, 2));
                    temp = 0;
                }

                for (int j = i + 1; j < features.Length; j++)
                {

                    if (AreNeighbours(features[i], features[j]))
                    {
                        features[i].Neighbours.Add(features[j]);
                        features[j].Neighbours.Add(features[i]);
                    }
                }
            }

            Console.WriteLine("Finished import");
        }

        private static bool AreNeighbours(RouteFeature routeFeature, RouteFeature testRouteFeature)
        {
            if (routeFeature == testRouteFeature)
                return false;

            var startPoint1 = routeFeature.Data.Coordinates.First().ToArray();
            var endPoint1 = routeFeature.Data.Coordinates.Last().ToArray();

            var startPoint2 = testRouteFeature.Data.Coordinates.First().ToArray();
            var endPoint2 = testRouteFeature.Data.Coordinates.Last().ToArray();

            if (DistanceHelpers.AreClose(startPoint1, startPoint2))
                return true;

            if (DistanceHelpers.AreClose(startPoint1, endPoint2))
                return true;

            if (DistanceHelpers.AreClose(endPoint1, startPoint2))
                return true;

            if (DistanceHelpers.AreClose(endPoint1, endPoint2))
                return true;

            return false;
        }

        private static IDictionary<string, object> GetAttributes(IFeature feature, string[] columns)
        {
            var attributes = new Dictionary<string, dynamic>();

            for (int i = 0; i < feature.DataRow.ItemArray.Length; i++)
            {
                attributes[columns[i]] = feature.DataRow.ItemArray[i];
            }

            return attributes;
        }

        private static string[] GetColumns(Shapefile sf)
        {
            var columns = new List<string>();

            foreach (var c in sf.DataTable.Columns)
            {
                columns.Add(c.ToString());
            }

            return columns.ToArray();
        }
    }
}