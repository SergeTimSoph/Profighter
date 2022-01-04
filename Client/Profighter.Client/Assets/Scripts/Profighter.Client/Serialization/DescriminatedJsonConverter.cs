using System;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Profighter.Client.Serialization
{
    public class DiscriminatedJsonConverter : JsonConverter
    {
        private readonly IDiscriminatorOptions discriminatorOptions;

        public DiscriminatedJsonConverter(Type concreteDiscriminatorOptionsType)
            : this((IDiscriminatorOptions)Activator.CreateInstance(concreteDiscriminatorOptionsType))
        {
        }

        public DiscriminatedJsonConverter(IDiscriminatorOptions discriminatorOptions)
        {
            this.discriminatorOptions = discriminatorOptions ?? throw new ArgumentNullException(nameof(discriminatorOptions));
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => discriminatorOptions.BaseType.IsAssignableFrom(objectType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, Newtonsoft.Json.JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            var jObject = JObject.Load(reader);
            var discriminatorField = jObject.Property(discriminatorOptions.DiscriminatorFieldName);

            string discriminatorFieldValue = discriminatorField != null
                ? discriminatorField.Value.ToString()
                : objectType.Name;

            var targetType =
                discriminatorOptions.GetDiscriminatedTypes().FirstOrDefault(tuple => tuple.TypeName == discriminatorFieldValue).Type
                ?? GetTypeByName(discriminatorFieldValue, objectType.GetTypeInfo())
                ?? objectType;

            if (!discriminatorOptions.SerializeDiscriminator)
            {
                // Remove the discriminator field from the JSON for two possible reasons:
                // 1. the user doesn't want to copy the discriminator value from JSON to the object, only the other way around
                // 2. the object doesn't even have a discriminator property, in which case MissingMemberHandling.Error would throw
                discriminatorField.Remove();
            }

            // There might be a different converter on the 'targetType' type
            // Use Deserialize to let Json.NET choose the next converter
            // Use Populate to ignore any remaining converters (prevents recursion when the next converter is the same as this)
            if (targetType != objectType && targetType.GetTypeInfo().GetCustomAttributes<JsonConverterAttribute>().Any())
            {
                using (var readerCopy = reader.CopyForObject(jObject))
                {
                    return serializer.Deserialize(readerCopy, targetType);
                }
            }

            var value = Activator.CreateInstance(targetType, true);

            using (var newReader = reader.CopyForObject(jObject))
            {
                serializer.Populate(newReader, value);
                return value;
            }
        }

        public override void WriteJson(JsonWriter writer, object value, Newtonsoft.Json.JsonSerializer serializer) =>
            throw new NotImplementedException("DiscriminatedJsonConverter should only be used while deserializing.");

        private static Type GetTypeByName(string typeName, TypeInfo parentType)
        {
            var insideAssembly = parentType.Assembly;

            var parentTypeFullName = parentType.FullName;

            var typeByName = insideAssembly.GetType(typeName) ?? insideAssembly.GetType(typeName.Split(',')[0]);

            if (parentTypeFullName != null && typeByName == null)
            {
                var searchLocation = parentTypeFullName.Substring(0, parentTypeFullName.Length - parentType.Name.Length);
                typeByName = insideAssembly.GetType(searchLocation + typeName, false, true);
            }

            var typeByNameInfo = typeByName?.GetTypeInfo();
            if (typeByNameInfo != null && parentType.IsAssignableFrom(typeByNameInfo))
            {
                return typeByName;
            }

            return null;
        }
    }
}
