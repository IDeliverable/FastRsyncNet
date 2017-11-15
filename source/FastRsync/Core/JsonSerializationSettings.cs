using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace FastRsync.Core
{
    public class JsonSerializationSettings
    {
        static JsonSerializationSettings()
        {
            JsonSettings = new JsonSerializerSettings
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver(),
                Formatting = Formatting.None,
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        public static JsonSerializerSettings JsonSettings { get; }
    }
}
