using WhiteStone.Common;

namespace BOA.CodeGeneration.Model
{
    public class NamingModel : ContractBase
    {
        #region Public Properties
        public string ClassNameOfBusiness { get; set; }

        public string ClassNameOfOrchestration { get; set; }

        public string ContractName { get; set; }

        public string NameOfDotNetMethodDelete { get; set; }

        public string NameOfDotNetMethodInsert { get; set; }

        public string NameOfDotNetMethodSelectByColumns { get; set; }

        public string NameOfDotNetMethodSelectByKey { get; set; }

        public string NameOfDotNetMethodSelectByKeyList { get; set; }

        public string NameOfDotNetMethodSelectByValueListFormat { get; set; }

        public string NameOfDotNetMethodUpdate { get; set; }

        public string NameOfSqlProcedureSelectByKey { get; set; }

        public string NameOfSqlProcedureSelectByKeyList { get; set; }

        public string NameOfSqlProcedureSelectByKeyListWithSchema { get; set; }

        public string NameOfSqlProcedureSelectByKeyWithSchema { get; set; }

        public string NameOfSqlProcedureSelectByValueListFormat { get; set; }

        public string NamespaceNameOfBusinessClass { get; set; }

        public string OutputFileNameContract { get; set; }

        public string OutputFileNameOrchestration { get; set; }
        public string SchemaName                  { get; set; }
        #endregion

        #region string NamespaceName
        string _namespaceName;

        public string NamespaceName
        {
            get { return _namespaceName; }
            set
            {
                if (_namespaceName != value)
                {
                    _namespaceName = value;
                    OnPropertyChanged("NamespaceName");
                }
            }
        }
        #endregion

        #region string DatabaseTableFullPath
        string _databaseTableFullPath;

        public string DatabaseTableFullPath
        {
            get { return _databaseTableFullPath; }
            set
            {
                if (_databaseTableFullPath != value)
                {
                    _databaseTableFullPath = value;
                    OnPropertyChanged("DatabaseTableFullPath");
                }
            }
        }
        #endregion

        #region string DatabaseEnumName
        string _databaseEnumName;

        public string DatabaseEnumName
        {
            get { return _databaseEnumName; }
            set
            {
                if (_databaseEnumName != value)
                {
                    _databaseEnumName = value;
                    OnPropertyChanged("DatabaseEnumName");
                }
            }
        }
        #endregion

        #region string NameOfSqlProcedureInsert
        string _nameOfSqlProcedureInsert;

        public string NameOfSqlProcedureInsert
        {
            get { return _nameOfSqlProcedureInsert; }
            set
            {
                if (_nameOfSqlProcedureInsert != value)
                {
                    _nameOfSqlProcedureInsert = value;
                    OnPropertyChanged("NameOfSqlProcedureInsert");
                }
            }
        }
        #endregion

        #region string NameOfSqlProcedureUpdate
        string _nameOfSqlProcedureUpdate;

        public string NameOfSqlProcedureUpdate
        {
            get { return _nameOfSqlProcedureUpdate; }
            set
            {
                if (_nameOfSqlProcedureUpdate != value)
                {
                    _nameOfSqlProcedureUpdate = value;
                    OnPropertyChanged("NameOfSqlProcedureUpdate");
                }
            }
        }
        #endregion

        #region string NameOfSqlProcedureInsertWithSchema
        string _nameOfSqlProcedureInsertWithSchema;

        public string NameOfSqlProcedureInsertWithSchema
        {
            get { return _nameOfSqlProcedureInsertWithSchema; }
            set
            {
                if (_nameOfSqlProcedureInsertWithSchema != value)
                {
                    _nameOfSqlProcedureInsertWithSchema = value;
                    OnPropertyChanged("NameOfSqlProcedureInsertWithSchema");
                }
            }
        }
        #endregion

        #region string NameOfSqlProcedureUpdateWithSchema
        string _nameOfSqlProcedureUpdateWithSchema;

        public string NameOfSqlProcedureUpdateWithSchema
        {
            get { return _nameOfSqlProcedureUpdateWithSchema; }
            set
            {
                if (_nameOfSqlProcedureUpdateWithSchema != value)
                {
                    _nameOfSqlProcedureUpdateWithSchema = value;
                    OnPropertyChanged("NameOfSqlProcedureUpdateWithSchema");
                }
            }
        }
        #endregion

        #region string NameOfSqlProcedureDeleteWithSchema
        string _nameOfSqlProcedureDeleteWithSchema;

        public string NameOfSqlProcedureDeleteWithSchema
        {
            get { return _nameOfSqlProcedureDeleteWithSchema; }
            set
            {
                if (_nameOfSqlProcedureDeleteWithSchema != value)
                {
                    _nameOfSqlProcedureDeleteWithSchema = value;
                    OnPropertyChanged("NameOfSqlProcedureDeleteWithSchema");
                }
            }
        }
        #endregion

        #region string NameOfSqlProcedureDelete
        string _nameOfSqlProcedureDelete;

        public string NameOfSqlProcedureDelete
        {
            get { return _nameOfSqlProcedureDelete; }
            set
            {
                if (_nameOfSqlProcedureDelete != value)
                {
                    _nameOfSqlProcedureDelete = value;
                    OnPropertyChanged("NameOfSqlProcedureDelete");
                }
            }
        }
        #endregion

        #region string OutputFileNameBusiness
        string _outputFileNameBusiness;

        public string OutputFileNameBusiness
        {
            get { return _outputFileNameBusiness; }
            set
            {
                if (_outputFileNameBusiness != value)
                {
                    _outputFileNameBusiness = value;
                    OnPropertyChanged("OutputFileNameBusiness");
                }
            }
        }
        #endregion

        #region string NameOfSqlProceduresWillBeRunCatalogName
        string _nameOfSqlProceduresWillBeRunCatalogName;

        public string NameOfSqlProceduresWillBeRunCatalogName
        {
            get { return _nameOfSqlProceduresWillBeRunCatalogName; }
            set
            {
                if (_nameOfSqlProceduresWillBeRunCatalogName != value)
                {
                    _nameOfSqlProceduresWillBeRunCatalogName = value;
                    OnPropertyChanged("NameOfSqlProceduresWillBeRunCatalogName");
                }
            }
        }

        public string CompanyNameWillbeShownInSqlComments { get; set; } = "KUWAIT TURKISH PARTICIPATION BANK INC.";
        #endregion
    }
}