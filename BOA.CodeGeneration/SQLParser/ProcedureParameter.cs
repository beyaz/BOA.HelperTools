using BOA.Common.Helpers;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace BOA.CodeGeneration.SQLParser
{
    public interface IProcedureParameter
    {
        #region Public Properties
        bool   IsNullable  { get; }
        string Name        { get; }
        string SqlDataType { get; }
        #endregion
    }

    class ProcedureParameter : IProcedureParameter
    {
        #region Fields
        readonly Microsoft.SqlServer.TransactSql.ScriptDom.ProcedureParameter _parameter;
        #endregion

        #region Constructors
        public ProcedureParameter(Microsoft.SqlServer.TransactSql.ScriptDom.ProcedureParameter parameter)
        {
            _parameter = parameter;
        }
        #endregion

        #region Public Properties
        public bool IsNullable => _parameter.Value is NullLiteral;

        public string Name => _parameter.VariableName.Value.Substring(1);

        public string SqlDataType => ((SqlDataTypeReference) _parameter.DataType).SqlDataTypeOption.ToString().ToUpperEN();
        #endregion
    }
}