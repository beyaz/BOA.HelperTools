using System;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Generators;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Services;
using BOA.DatabaseAccess;
using BOAPlugins.Models;

namespace BOAPlugins.GenerateEntityContract
{
    public class Handler
    {
        #region Public Methods
        public Result Handle(Input input)
        {
            var result = new Result();
            if (string.IsNullOrWhiteSpace(input.SelectedText))
            {
                result.ErrorMessage = "Editörden bir text seçiniz.(Tablo adı)";
                return result;
            }

            var selectedText = ProcedureTextUtility.ClearProcedureText(input.SelectedText);

            var dbItemNameParser = DbItemNameInfoParser.Parse(selectedText);

            var connectionInfo = FindDbItemContainerDatabaseConnectionInfo(input, result);
            if (connectionInfo == null)
            {
                return result;
            }

            var table = GetTableInfo(new SqlDatabase(connectionInfo.ConnectionStringDev), dbItemNameParser);

            var generator = new ContractBodyGenerator
            {
                Columns = table.Columns,
                Padding = 4,
                RegionText = dbItemNameParser.ToString()
            };

            generator.GenerateDatabaseColumns();
            result.ContractClassBody = generator.GeneratedString;

            return result;
        }
        #endregion

        #region Methods
        static DatabaseConnectionInfo FindDbItemContainerDatabaseConnectionInfo(Input input, ResultBase result)
        {
            var procedureName = input.SelectedText;
            var handler = new SearchProcedure.Handler(new SearchProcedure.Input {ProcedureName = procedureName});
            handler.Handle();

            if (handler.Result.ErrorMessage != null)
            {
                result.ErrorMessage = handler.Result.ErrorMessage;
            }
            return handler.Result.ProcedureContainerDatabaseConnectionInfo;
        }

        static TableInfo GetTableInfo(IDatabase db, DbItemNameInfo input)
        {
            using (var dal = new DataAccess(db))
            {
                if (!dal.TableExists(input.SchemaName, input.Name))
                {
                    throw new ArgumentException("TableNotFoundInDatabase:" + input.Name);
                }

                var table = dal.GetTableInformation(input.DatabaseName, input.SchemaName, input.Name);

                if (table.Rows.Count <= 0)
                {
                    throw new ArgumentException("TableNotFoundInDatabase:" + input.Name);
                }

                var tableInfo = TableInfoGeneratorFromMsSql.CreateTable(table);

                #region Update comments
                foreach (var c in tableInfo.Columns)
                {
                    c.Comment = dal.GetColumnComment(input.SchemaName, input.Name, c.ColumnName);
                }

                return tableInfo;
                #endregion
            }
        }
        #endregion
    }
}