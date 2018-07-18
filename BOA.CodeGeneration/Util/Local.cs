using System.IO;

namespace BOA.CodeGeneration.Util
{
    public static class Local
    {
        #region Static Fields
        static bool? _isBOAGermany;
        #endregion

        #region Public Properties
        public static bool IsBOAGermany
        {
            get
            {
                if (_isBOAGermany == null)
                {
                    var key  = "srvdedb;";
                    var path = @"D:\BOA\server\Web.config";
                    if (!File.Exists(path))
                    {
                        return false;
                    }

                    var content = File.ReadAllText(path);

                    _isBOAGermany = content.Contains(key);
                }

                return _isBOAGermany.Value;
            }
        }
        #endregion
    }
}