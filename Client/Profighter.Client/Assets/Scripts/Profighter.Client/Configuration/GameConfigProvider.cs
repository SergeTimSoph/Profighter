using System.IO;
using Profighter.Client.Serialization;
using UnityEngine;

namespace Profighter.Client.Configuration
{
    public class GameConfigProvider : IGameConfigProvider
    {
        private readonly GameConfig gameConfig;
        private const string GameConfigFileName = "gameConfig.json";
        private readonly IGameConfigValidator validator;

        public GameConfigProvider()
        {
            validator = new GameConfigValidator();
        }

        public GameConfig GetGameConfig()
        {
            var configFilePath = Path.Combine(Application.streamingAssetsPath, GameConfigFileName);
            var validationResult = validator.Validate(configFilePath);

            return validationResult;
        }
    }
}