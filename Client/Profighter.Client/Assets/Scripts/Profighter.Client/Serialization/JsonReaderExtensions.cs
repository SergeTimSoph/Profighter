using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Profighter.Client.Serialization
{
    public static class JsonReaderExtensions
    {
        /// <summary>
        /// Creates a new reader for the specified jObject by copying the settings from an existing reader.
        /// </summary>
        /// <param name="reader">The reader whose settings should be copied.</param>
        /// <param name="jToken">The jToken to create a new reader for.</param>
        public static JsonReader CopyForObject(this JsonReader reader, JToken jToken)
        {
            JsonReader jTokenReader = jToken.CreateReader();
            jTokenReader.Culture = reader.Culture;
            jTokenReader.DateFormatString = reader.DateFormatString;
            jTokenReader.DateParseHandling = reader.DateParseHandling;
            jTokenReader.DateTimeZoneHandling = reader.DateTimeZoneHandling;
            jTokenReader.FloatParseHandling = reader.FloatParseHandling;
            jTokenReader.MaxDepth = reader.MaxDepth;
            jTokenReader.SupportMultipleContent = reader.SupportMultipleContent;

            return jTokenReader;
        }
    }
}