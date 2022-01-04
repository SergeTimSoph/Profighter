using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Profighter.Client.Serialization
{
    /// <summary>
    /// Specifies the default settings on a <see cref="Newtonsoft.Json.JsonSerializer"/> object.
    /// </summary>
    public sealed class DefaultSerializerSettings : JsonSerializerSettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultSerializerSettings"/> class.
        /// </summary>
        public DefaultSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.Auto;
            PreserveReferencesHandling = PreserveReferencesHandling.None;
            NullValueHandling = NullValueHandling.Ignore;
            DefaultValueHandling = DefaultValueHandling.Populate;
            ObjectCreationHandling = ObjectCreationHandling.Replace;
            ContractResolver = CanWritePropertiesResolver.Instance;
            ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor;

            Converters.Add(new VersionConverter());
        }

        /// <summary>
        /// Updates the provided settings with values from <see cref="DefaultSerializerSettings"/>.
        /// </summary>
        /// <param name="serializerSettings">The setting to update.</param>
        public static void UpdateWithThisSettings(JsonSerializerSettings serializerSettings)
        {
            var updateWith = new DefaultSerializerSettings();

            serializerSettings.TypeNameHandling = updateWith.TypeNameHandling;
            serializerSettings.PreserveReferencesHandling = updateWith.PreserveReferencesHandling;
            serializerSettings.NullValueHandling = updateWith.NullValueHandling;
            serializerSettings.DefaultValueHandling = updateWith.DefaultValueHandling;
            serializerSettings.ObjectCreationHandling = updateWith.ObjectCreationHandling;
            serializerSettings.ContractResolver = updateWith.ContractResolver;
            serializerSettings.ConstructorHandling = updateWith.ConstructorHandling;

            foreach (var converter in updateWith.Converters)
            {
                serializerSettings.Converters.Add(converter);
            }
        }
    }
}
