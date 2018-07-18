using BOA.CodeGeneration.Common;

namespace BOA.CodeGeneration.Model
{
    public class CustomUpdateMethod : CustomMethod
    {
        #region Public Properties
        public MemberAccessibility MethodAccessiblity { get; set; }
        public string              UpdateColumnNames  { get; set; }

        public string WhereColumnNames { get; set; }
        #endregion
    }
}