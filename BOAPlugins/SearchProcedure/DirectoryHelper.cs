using System.IO;

namespace BOAPlugins
{
    public static class DirectoryHelper
    {
        #region Public Properties

        public static string PluginDirectory
        {
            get
            {
                var path = typeof(DirectoryHelper).Assembly.Location;
                path = Path.GetDirectoryName(path) + Path.DirectorySeparatorChar;
                return path;
            }
        }
        #endregion
    }
}