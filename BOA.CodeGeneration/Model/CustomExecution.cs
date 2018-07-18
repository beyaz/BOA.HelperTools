using System;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.SQLParser;
using BOA.DatabaseAccess;

namespace BOA.CodeGeneration.Model
{
    public class CustomExecution : CustomMethod
    {
        #region Fields
        [field: NonSerialized] public ITypeDefinition ReturnValueType;
        #endregion

        #region Public Properties
        public IDatabase Database { get; set; }

        public string DatabaseEnumName { get; set; }

        public ExecutionType ExecutionType { get; set; }

        public string               GenericResponseMethodReturnType => ReturnValueType.Name;
        public string               ProcedureDefinitionScript       { get; set; }
        public string               ProcedureFullName               { get; set; }
        public IProcedureDefinition ProcedureInfo                   { get; set; }

        public bool ReturnOnlyOneRecord { get; set; }
        #endregion
    }
}