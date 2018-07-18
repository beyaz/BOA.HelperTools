using System;
using System.Collections.Generic;
using System.IO;
using BOA.CodeGeneration.Common;
using WhiteStone.Common;
using WhiteStone.Helpers;
using WhiteStone.Services;

namespace BOA.CodeGeneration.Model
{
    public class TableConfig : ContractBase
    {
        #region Static Fields
        static Confiuration _confiuration;
        #endregion

        #region Constructors
        public TableConfig()
        {
            CustomSelects = new List<CustomSelectMethod>();

            CustomExists = new List<CustomSelectMethod>();

            CustomUpdates = new List<CustomUpdateMethod>();

            MethodParameterCannotBeNullMessage = "BOA.Messages.Kernel.ParameterNotNull";

            TypeContractIsExportable = true;

            if (Confiuration != null)
            {
                Confiuration.GetType().GetProperties().ForEach(p => GetType().GetProperty(p.Name)?.SetValue(this, p.GetValue(Confiuration)));
            }
        }
        #endregion

        #region Public Properties
        public Action<NamingModel> AfterNamingModelCreated { get; set; }
        public string              BaseConfig              { get; set; }
        public bool                CanGenerateDelete       { get; set; }

        public bool CanGenerateInsert           { get; set; }
        public bool CanGenerateInsertStructured { get; set; }

        public bool CanGenerateSelectByKey        { get; set; }
        public bool CanGenerateSelectByKeyList    { get; set; }
        public bool CanGenerateSetNoCountOnInsert { get; set; }
        public bool CanGenerateSetNoCountOnUpdate { get; set; }

        public bool? CanGenerateSetNoCountOnWhenDeleteByKey { get; set; }

        public bool? CanGenerateSetNoCountOnWhenSelectByKey { get; set; }

        public bool CanGenerateUpdate                  { get; set; }
        public bool CanUpdateProcedureOnTargetDatabase { get; set; }

        public string ClassNameOfBusiness { get; set; }

        public string ConnectionStringForTakeTableInformation { get; set; }

        public bool ContractIsSealed { get; set; }

        public string ContractName { get; set; }

        public string PrefixOfFieldOfContractProperty { get; set; } = "_";

        public IReadOnlyList<CustomMethod> CustomExecutions { get; set; }

        public IReadOnlyList<CustomSelectMethod> CustomExists { get; set; }

        public IReadOnlyList<CustomSelectMethod> CustomSelects { get; set; }

        public IReadOnlyList<CustomUpdateMethod> CustomUpdates { get; set; }

        public string DatabaseEnumName { get; set; }

        public string DatabaseName { get; set; }

        public string DatabaseTableFullPath
        {
            get
            {
                if (ServerName == null)
                {
                    return DatabaseName + "." + SchemaName + "." + TableName;
                }

                return ServerName + "." + DatabaseName + "." + SchemaName + "." + TableName;
            }
        }

        public string DatabaseTargetSchemaForProcedureNames { get; set; }

        public bool DoCompressionForVarBinaryColumns { get; set; }

        public IReadOnlyList<string> DoNotTrimColumns { get; set; }

        public string ExcludeColumns { get; set; }

        public string FolderPathForExtractBusinessFiles { get; set; }

        public virtual string FolderPathForExtractOrchestrationFiles { get; set; }

        public string FolderPathForExtractStoredProcedureFiles { get; set; }

        public string FolderPathForExtractTypesFiles { get; set; }

        public bool HasOriginalTable { get; set; }

        public bool InsertMethodMustBePrivate { get; set; }

        public string MethodParameterCannotBeNullMessage { get; set; }

        public string NamespaceName { get; set; }

        public string NamespaceNameForOrchestrationClass { get; set; }

        public string NamespaceNameForTypeClass { get; set; }

        public bool NoNeedToGenerateMethodReadContract { get; set; }

        public Action<List<string>> OnUsingNamespacesWillbeGenerateInBusinessClassDesignerFile { get; set; }

        public Dictionary<string, string> ReadContractSpecificReads { get; set; }

        /// <summary>
        ///     Gets or sets the read value from data reader.
        /// </summary>
        public Func<CustomReadValueFromIDataReaderInput, string> ReadValueFromDataReader { get; set; }

        public string SchemaName { get; set; }

        public List<string> SecureColumns { get; set; }

        public string SelectByKeyListMethodName { get; set; }

        public bool SelectByKeyMustBeReturnReadonlyContract { get; set; }

        public string ServerName { get; set; }

        public string ServerNameForTakeTableInformation { get; set; }

        public string TableName { get; set; }

        /// <summary>
        ///     Returns SchemaName + TableName
        /// </summary>
        public string TablePathForSqlScript
        {
            get
            {
                if (ServerName == null)
                {
                    var databaseNameMustBeWrite = DatabaseName == DatabaseNames.dbkuveyt2;
                    if (databaseNameMustBeWrite)
                    {
                        return DatabaseName + "." + SchemaName + "." + TableName;
                    }

                    return $"[{SchemaName}].[{TableName}]";
                }

                return ServerName + "." + DatabaseName + "." + SchemaName + "." + TableName;
            }
        }

        public bool TypeContractIsExportable { get; set; }
        #endregion

        #region Properties
        static string ConfigurationFilePath => "BOA.CodeGeneration.config.json";

        // static string ConfigurationFilePath => Path.GetDirectoryName(typeof(TableConfig).Assembly.Location) + Path.DirectorySeparatorChar + "BOA.CodeGeneration.config.json";
        static Confiuration Confiuration
        {
            get
            {
                if (_confiuration != null)
                {
                    return _confiuration;
                }

                if (!File.Exists(ConfigurationFilePath))
                {
                    return null;
                }

                _confiuration = new JsonSerializer().Deserialize<Confiuration>(File.ReadAllText(ConfigurationFilePath));

                return _confiuration;
            }
        }
        #endregion

        #region Public Methods
        public bool IsSecureColumn(string columnName)
        {
            var secureColumns = SecureColumns;
            if (secureColumns == null)
            {
                return false;
            }

            return secureColumns.Contains(columnName);
        }
        #endregion
    }

    public class CustomReadValueFromIDataReaderInput
    {
        #region Public Properties
        public string PropertyName       { get; set; }
        public string ReaderArgumentName { get; set; }
        public string ReaderMethodName   { get; set; }
        #endregion
    }
}