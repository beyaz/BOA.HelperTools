using System.IO;

namespace BOAPlugins
{
    /// <summary>
    /// The assemly updater
    /// </summary>
    public class AssemlyUpdater
    {
        #region Public Properties
        /// <summary>
        /// Gets the plugin directory.
        /// </summary>
        public static string PluginDirectory => Path.GetDirectoryName(typeof(AssemlyUpdater).Assembly.Location) + Path.DirectorySeparatorChar;
        #endregion
    }
}