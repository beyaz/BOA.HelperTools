using System;
using System.IO;
using WhiteStone;
using WhiteStone.Services;

namespace BOAPlugins
{
    [Serializable]
    public class Configuration
    {
        #region Public Properties
        public string CheckInCommentDefaultValue { get; set; }
        public string PluginUpdateDirectory      { get; set; }
        #endregion

        #region Properties
        static ISerializer Serializer => SM.Get<ISerializer>() ?? SM.Set<ISerializer>(new JsonSerializer());
        #endregion

        #region Public Methods
        public static void Initialize()
        {
            var configurationAsString = File.ReadAllText(AssemlyUpdater.PluginDirectory + "BOAPlugins.VSIntegration.Configuration.json");

            var config = Serializer.Deserialize<Configuration>(configurationAsString);

            SM.Set(config);
        }
        #endregion
    }
}