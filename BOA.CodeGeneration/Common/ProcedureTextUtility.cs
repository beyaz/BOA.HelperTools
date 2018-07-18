namespace BOA.CodeGeneration.Common
{
    public static class ProcedureTextUtility
    {
        #region Public Methods
        public static string ClearProcedureText(string procedureName)
        {
            return procedureName.Replace("[", "").Replace("]", "").Replace(")", "").Replace("(", "").Replace(";", "").Replace('"'.ToString(), "");
        }
        #endregion
    }
}