using System;
using System.Collections.Generic;

namespace BOAPlugins.ExportingModel
{
    [Serializable]
    public class ExportInfo
    {
        #region Public Properties
        public string                      Assembly         { get; set; }
        public ICollection<string> ExportClassNames { get; set; }
        #endregion
    }

    [Serializable]
    public class ExportContract
    {
        #region Public Properties
        public string                          ErrorMessage   { get; set; }
        public ICollection<ExportInfo> ExportInfoList { get; set; }

        public string GeneratedTSCode { get; set; }
        public string TargetFilePath  { get; set; }
        #endregion
    }
}