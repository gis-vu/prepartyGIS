
namespace Helpers
{
    public static class FeaturesToGeojsonHelper
    {
        public static string ToGeojson(double[][][] coordinates)
        {
            var data = "{\"type\" : \"FeatureCollection\",\"features\" : [";

            foreach (var coorrs in coordinates)
            {
                data +=
                    "\n{\"type\" : \"Feature\", \"properties\" : {}, \"geometry\" : {\"type\":\"LineString\",\"coordinates\":[";

                foreach (var c in coorrs)
                    data += $"[{c[0].ToString().Replace(',', '.')},{c[1].ToString().Replace(',', '.')}],";

                data = data.Substring(0, data.Length - 1);

                data += "]}},";
            }

            data = data.Substring(0, data.Length - 1);

            data += "]}";

            return data;
        }
    }
}