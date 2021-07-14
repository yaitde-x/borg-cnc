namespace Utilities
{
    public static class Serialization {
        public static string ToJson<T>(this T value) {
            return Newtonsoft.Json.JsonConvert.SerializeObject(value);
        }

        public static T FromJson<T>(this string value) {
            return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(value);
        }
    }

}