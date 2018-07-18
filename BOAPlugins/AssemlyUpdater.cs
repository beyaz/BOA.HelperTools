using System.IO;

namespace BOAPlugins
{
    public class AssemlyUpdater
    {
        #region Public Properties
        public static string PluginDirectory => Path.GetDirectoryName(typeof(AssemlyUpdater).Assembly.Location) + Path.DirectorySeparatorChar;
        #endregion
    }
}