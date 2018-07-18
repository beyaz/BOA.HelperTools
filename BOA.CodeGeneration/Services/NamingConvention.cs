using System;
using System.Data;
using System.Linq;
using BOA.CodeGeneration.Model;
using BOA.Common.Helpers;

namespace BOA.CodeGeneration.Services
{
    public class NamingConvention
    {
        #region Public Properties
        public WriterContext Context { private get; set; }
        #endregion

        #region Properties
        string TableNameInDatabase
        {
            get
            {
                if (Context.Config.HasOriginalTable)
                {
                    return Context.Config.TableName + "_ORJ";
                }

                return Context.Config.TableName;
            }
        }
        #endregion

        #region Public Methods
        public void InitializeNames()
        {
            var tableConfig = Context.Config;
            var tableName   = tableConfig.TableName;

            var naming = new NamingModel
            {
                DatabaseTableFullPath                       = tableConfig.DatabaseTableFullPath,
                NamespaceName                               = tableConfig.NamespaceName,
                DatabaseEnumName                            = tableConfig.DatabaseEnumName,
                NameOfSqlProceduresWillBeRunCatalogName     = tableConfig.DatabaseEnumName.ToUpperEN(),
                SchemaName                                  = tableConfig.SchemaName,
                ContractName                                = tableConfig.ContractName ?? tableName + "Contract",
                OutputFileNameContract                      = tableConfig.ContractName ?? tableName + "Contract",
                ClassNameOfBusiness                         = tableConfig.ClassNameOfBusiness ?? tableName,
                OutputFileNameBusiness                      = tableConfig.ClassNameOfBusiness ?? tableName,
                NameOfSqlProcedureInsert                    = "ins_" + tableName,
                NameOfSqlProcedureUpdate                    = "upd_" + tableName,
                NameOfSqlProcedureDelete                    = "del_" + tableName,
                NameOfSqlProcedureSelectByKey               = "sel_" + tableName + "ByKey",
                NameOfSqlProcedureSelectByKeyList           = "sel_" + tableName + "ByKeyList",
                NameOfSqlProcedureSelectByValueListFormat   = "sel_" + tableName + "By{0}List",
                NameOfSqlProcedureInsertWithSchema          = tableConfig.SchemaName + "." + "ins_" + tableName,
                NameOfSqlProcedureUpdateWithSchema          = tableConfig.SchemaName + "." + "upd_" + tableName,
                NameOfSqlProcedureDeleteWithSchema          = tableConfig.SchemaName + "." + "del_" + tableName,
                NameOfSqlProcedureSelectByKeyWithSchema     = tableConfig.SchemaName + "." + "sel_" + tableName + "ByKey",
                NameOfSqlProcedureSelectByKeyListWithSchema = tableConfig.SchemaName + "." + "sel_" + tableName + "ByKeyList",
                NameOfDotNetMethodSelectByKey               = "SelectByKey",
                NameOfDotNetMethodSelectByKeyList           = "SelectByKeyList",
                NameOfDotNetMethodSelectByValueListFormat   = "SelectBy{0}List",
                NameOfDotNetMethodInsert                    = "Insert",
                NameOfDotNetMethodUpdate                    = "Update",
                NameOfDotNetMethodDelete                    = "Delete",
                NameOfDotNetMethodSelectByColumns           = "SelectByColumns",
                OutputFileNameOrchestration                 = tableName,
                ClassNameOfOrchestration                    = tableName,
                NamespaceNameOfBusinessClass                = "BOA.Business." + tableConfig.NamespaceName
            };

            tableConfig.AfterNamingModelCreated?.Invoke(naming);

            if (tableName != null)
            {
                Context.Table = InitializeTableInformation();
            }
            else
            {
                Context.Table = new TableInfo();
            }

            Context.Naming = naming;
        }
        #endregion

        #region Methods
        TableInfo InitializeTableInformation()
        {
            var tableConfig = Context.Config;

            using (var dal = new DataAccess(tableConfig.GetDatabase()))
            {
                if (!dal.TableExists(tableConfig.SchemaName, TableNameInDatabase))
                {
                    throw new ArgumentException("TableNotFoundInDatabase:" + TableNameInDatabase);
                }

                var table = dal.GetTableInformation(tableConfig.DatabaseName, tableConfig.SchemaName, TableNameInDatabase);

                if (table.Rows.Count <= 0)
                {
                    throw new ArgumentException("TableNotFoundInDatabase:" + TableNameInDatabase);
                }

                var primaryKeyColumns = dal.GetPrimaryColumns(tableConfig.SchemaName, TableNameInDatabase);

                var tableInfo = TableInfoGeneratorFromMsSql.CreateTable(table);

                tableInfo.ExcludeColumns(tableConfig.ExcludeColumns);

                foreach (DataRow r in primaryKeyColumns.Rows)
                {
                    var indexName = r[0].ToString().Trim();

                    (from c in tableInfo.Columns where c.ColumnName == indexName select c).First().IsPrimaryKey = true;
                }

                #region Update comments
                foreach (var c in tableInfo.Columns)
                {
                    c.Comment = dal.GetColumnComment(tableConfig.SchemaName, TableNameInDatabase, c.ColumnName);
                }

                return tableInfo;
                #endregion
            }
        }
        #endregion

        //    {
        //    if (value == null)
        //{

        //static string HungarianNotation(string value)
        //        throw new ArgumentNullException(nameof(value));
        //    }

        //    value = value.ToLowerEN();
        //    value = value.First().ToString().ToUpperEN() + value.Substring(1);
        //    return value;
        //}
    }
}