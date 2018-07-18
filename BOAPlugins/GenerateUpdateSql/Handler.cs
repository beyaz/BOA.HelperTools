using System;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Generators;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Services;
using BOAPlugins.Models;
using BOA.Common.Helpers;

namespace BOAPlugins.GenerateUpdateSql
{
    public class Input
    {
        #region Public Properties
        public string TableName { get; set; }
        #endregion
    }

    public class Result : ResultBase
    {
        #region Public Properties
        public string GeneratedSQLCode { get; set; }
        #endregion
    }

    /// <summary>
    ///     Defines the handler.
    /// </summary>
    public class Handler
    {
        #region Public Methods
        /// <summary>
        ///     Handles this instance.
        /// </summary>
        public Result Handle(Input input)
        {
            var Result = new Result();
            var tableName = input.TableName;

            if (tableName.IsNullOrEmpty())
            {
                Result.ErrorMessage = "Herhangi bir tablo ismi seçilmelidir";
                return Result;
            }

            var connectionInfo = FindProcedureContainerDatabaseConnectionInfo(input.TableName);
            if (connectionInfo == null)
            {
                return Result;
            }

            var info = DbItemNameInfoParser.Parse(tableName);

            var writerContext = new WriterContext
            {
                Config = new TableConfig
                {
                    TableName = info.Name,
                    SchemaName = info.SchemaName,
                    DatabaseName = info.DatabaseName ?? DatabaseNames.BOA,
                    DatabaseEnumName = connectionInfo.DatabaseName,
                    ServerNameForTakeTableInformation = ServerNames.GetServerNameOfConnectionString(connectionInfo.ConnectionStringDev)
                }
            };

            new NamingConvention {Context = writerContext}.InitializeNames();

            var generator = new UpdateSql(writerContext);

            Result.GeneratedSQLCode = generator.Generate();

            return Result;
        }
        #endregion

        #region Methods
        DatabaseConnectionInfo FindProcedureContainerDatabaseConnectionInfo(string text)
        {
            var procedureName = text;
            var handler = new SearchProcedure.Handler(new SearchProcedure.Input {ProcedureName = procedureName});
            handler.Handle();

            if (handler.Result.ErrorMessage != null)
            {
                throw new Exception(handler.Result.ErrorMessage);
            }
            return handler.Result.ProcedureContainerDatabaseConnectionInfo;
        }
        #endregion
    }
}