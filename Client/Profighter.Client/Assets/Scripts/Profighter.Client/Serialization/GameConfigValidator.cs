using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Profighter.Client.Configuration;
using Profighter.Client.SceneManagement;

namespace Profighter.Client.Serialization
{
    public class GameConfigValidator : IGameConfigValidator
    {
        public GameConfig Validate(string filePath)
        {
            var configJson = File.ReadAllText(filePath);
            using var reader = new JsonTextReader(new StringReader(configJson));

            try
            {
                var settings = new DefaultSerializerSettings();
                settings.Converters.Add(new DiscriminatedJsonConverter(new ContentDiscriminatorOptions()));
                var jsonSerializer = Newtonsoft.Json.JsonSerializer.Create(settings);

                return jsonSerializer.Deserialize<GameConfig>(reader);
            }
            catch (JsonException ex)
            {
                throw new Exception($"Json validation exceptions {ex}");
            }
        }

        private abstract class DiscriminatorOptions : IDiscriminatorOptions
        {
            public string DiscriminatorFieldName => "_t";

            public abstract Type BaseType { get; }

            public bool SerializeDiscriminator => true;

            public IEnumerable<(string TypeName, Type Type)> GetDiscriminatedTypes() => Enumerable.Empty<(string, Type)>();
        }

        private class ContentDiscriminatorOptions : DiscriminatorOptions
        {
            public override Type BaseType => typeof(IItemIdentity);
        }
    }
}
