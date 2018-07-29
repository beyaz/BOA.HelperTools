using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Generators;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;
using BOA.Common.Helpers;
using BOA.DatabaseAccess;
using BOAPlugins.Models;
using BOAPlugins.TypeSearchView;

namespace BOAPlugins.GenerateCSharpCode
{
    /// <summary>
    /// The handler
    /// </summary>
    public class Handler
    {
        #region Fields
        readonly Input _input;
        #endregion

        #region Constructors
        public Handler(Input input)
        {
            _input = input;
            Result = new Result();
        }
        #endregion

        #region Public Properties
        public Result Result { get; }
        #endregion

        #region Public Methods
        public static string GetMethodNameFromProcedure(string procedureName)
        {
            procedureName = ClearProcedure(procedureName);
            procedureName = procedureName.Split('.').Last();

            if (procedureName.StartsWith("sel_"))
            {
                procedureName = "Select" + procedureName.RemoveFromStart("sel_");
            }

            if (procedureName.StartsWith("upd_"))
            {
                procedureName = "Update" + procedureName.RemoveFromStart("upd_");
            }

            if (procedureName.StartsWith("del_"))
            {
                procedureName = "Delete" + procedureName.RemoveFromStart("del_");
            }

            if (procedureName.StartsWith("ins_"))
            {
                procedureName = "Insert" + procedureName.RemoveFromStart("ins_");
            }

            return procedureName;
        }

        public virtual string GetReturnTypeNameFromUser()
        {
            return UserIteraction.FindType(BinFolderPaths.ServerBin);
        }

        public void Handle()
        {
            var procedureName = _input.ProcedureName;

            if (procedureName.IsNullOrEmpty())
            {
                Result.ErrorMessage = "Herhangi bir procedure ismi seçilmelidir";
                return;
            }
            var connectionInfo = FindProcedureContainerDatabaseConnectionInfo();
            if (connectionInfo == null)
            {
                return;
            }

            using (var database = new SqlDatabase(connectionInfo.ConnectionStringDev))
            {
                procedureName = ClearProcedure(procedureName);

                var dotNetMethodName = GetMethodNameFromProcedure(procedureName);

                var data = new CustomExecution
                {
                    DotNetMethodName = dotNetMethodName,
                    SqlProcedureName = procedureName.Split('.').Last(),
                    ExecutionType = ExecutionType.ExecuteReader,
                    DatabaseEnumName = connectionInfo.DatabaseName,
                    ProcedureFullName = procedureName,
                    Database = database
                };

                if (dotNetMethodName.StartsWith("Update") ||
                    dotNetMethodName.StartsWith("Delete"))
                {
                    data.ExecutionType = ExecutionType.ExecuteNonQuery;
                }
                if (dotNetMethodName.StartsWith("Insert"))
                {
                    data.ExecutionType = ExecutionType.ExecuteScalar;
                }

                var generator = new CustomExecutionCs(null, data);

                if (dotNetMethodName.StartsWith("Select"))
                {
                    if (generator.ProcedureInfoReturnColumns.Count == 1)
                    {
                        data.ExecutionType = ExecutionType.ExecuteReaderForOneColumn;
                    }
                }

                if (data.ReturnValueType == null)
                {
                    if (generator.ParameterIsContract ||
                        data.ExecutionType == ExecutionType.ExecuteReader)
                    {
                        data.ReturnValueType = FindType();
                    }
                }

                Result.GeneratedCsCode = generator.Generate();
            }
        }
        #endregion

        #region Methods
        static string ClearProcedure(string procedureName)
        {
            return ProcedureTextUtility.ClearProcedureText(procedureName);
        }

        DatabaseConnectionInfo FindProcedureContainerDatabaseConnectionInfo()
        {
            var procedureName = _input.ProcedureName;
            var handler = new SearchProcedure.Handler(new SearchProcedure.Input {ProcedureName = procedureName});
            handler.Handle();

            if (handler.Result.ErrorMessage != null)
            {
                Result.ErrorMessage = handler.Result.ErrorMessage;
            }
            return handler.Result.ProcedureContainerDatabaseConnectionInfo;
        }

        ITypeDefinition FindType()
        {
            var classFullName = GetReturnTypeNameFromUser();
            if (LastUsedTypes.Value == null)
            {
                return classFullName.LoadBOAType();
            }

            return LastUsedTypes.Value.First(t => t.FullName == classFullName);
        }
        #endregion
    }
}