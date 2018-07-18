using System.IO;
using BOA.CodeGeneration.Services;
using BOA.CodeGeneration.Util;

namespace BOAPlugins
{
    public static class Util
    {
        #region Public Methods
        public static void WriteFileIfContentNotEqual(string path, string content)
        {
            if (!File.Exists(path))
            {
                File.WriteAllText(path, content);
                return;
            }

            var existingData = File.ReadAllText(path);

            var isEqual = SpaceCaseInsensitiveComparator.Compare(existingData, content);

            if (!isEqual)
            {
                new TFSAccessForBOA().CheckoutFile(path);
                File.WriteAllText(path, content);
            }
        }
        #endregion
    }
}