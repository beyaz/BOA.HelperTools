using System;

namespace BOA.CodeGeneration.Model
{
    [Serializable]
    public class Confiuration
    {
        #region Public Properties
        public bool CanGenerateSetNoCountOnInsert          { get; set; }
        public bool CanGenerateSetNoCountOnUpdate          { get; set; }
        public bool CanGenerateSetNoCountOnWhenDeleteByKey { get; set; }
        public bool CanGenerateSetNoCountOnWhenSelectByKey { get; set; }
        public bool CanUpdateProcedureOnTargetDatabase     { get; set; }
        #endregion
    }
}