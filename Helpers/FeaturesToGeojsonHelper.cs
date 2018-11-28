using DTO;

namespace Helpers
{
    public static class FeaturesToGeojsonHelper
    {
        public static string ToGeojson(RouteFeature[] features)
        {
            var data = "{\"type\" : \"FeatureCollection\",\"features\" : [";

            foreach (var f in features)
            {
                data +=
                    "\n{\"type\" : \"Feature\", \"properties\" : {}, \"geometry\" : {\"type\":\"LineString\",\"coordinates\":[";

                foreach (var c in f.Data.Coordinates)
                    data += $"[{c.Longitude.ToString().Replace(',', '.')},{c.Latitude.ToString().Replace(',', '.')}],";

                data = data.Substring(0, data.Length - 1);

                data += "]}},";
            }

            data = data.Substring(0, data.Length - 1);

            data += "]}";

            return data;
        }
    }
}