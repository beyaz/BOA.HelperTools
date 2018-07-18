using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Generators
{
    class InsertCs : WriterBase
    {
        #region Constructors
        #region Constructor
        public InsertCs(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Properties
        bool DoCompressionForVarBinaryColumns => Context.Config.DoCompressionForVarBinaryColumns;
        #endregion

        #region Public Methods
        public string Generate()
        {
            Padding = PaddingForMethodDeclaration;
            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Insert new record into '{0}'", DatabaseTableFullPath);
            WriteLine("/// </summary>");

            var methodAccess = "public ";
            if (Context.Config.InsertMethodMustBePrivate)
            {
                methodAccess = "";
            }

            WriteLine("{2}GenericResponse<int> {0}({1} contract)", NameOfCsMethodInsert, ContractName, methodAccess);

            WriteLine("{");

            Padding++;

            WriteLine("var returnObject = CreateResponse<int>();", NameOfCsMethodInsert);
            WriteLine();
            WriteLine("if (contract == null)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.AddError({0});", MethodParameterCannotBeNullMessage);
            Padding--;
            WriteLine("}");

            WriteLine();

            WriteLine("var command = DBLayer.GetDBCommand(Databases.{0}, \"{1}\");",
                      DatabaseEnumName, DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedureInsert);

            WriteLine("");

            WriteLine("#region Parameters");

            var contractUpdated = false;

            if (Columns.Any(c => c.ColumnName == Names.UserName))
            {
                contractUpdated = true;
                WriteLine("contract.UserName = {0};", Names.UserNameValue);
            }

            if (Columns.Any(c => c.ColumnName == Names.HostName))
            {
                contractUpdated = true;
                WriteLine("contract.HostName = {0};", Names.HostNameValue);
            }

            if (Columns.Any(c => c.ColumnName == Names.HostIP))
            {
                contractUpdated = true;
                WriteLine("contract.HostIP = {0};", Names.HostIPValue);
            }

            if (Columns.Any(c => c.ColumnName == Names.SystemDate))
            {
                contractUpdated = true;
                WriteLine("contract.SystemDate = {0};", Names.SystemDateValue);
            }

            if (contractUpdated)
            {
                WriteLine();
            }

            var columns = GetProcedureParameterColumns();

            foreach (var c in columns)
            {
                var dbLayerMethod = GetDbLayerMethod(c);

                var inputValue = "contract." + c.ColumnName;

                if (c.DataType == SqlDataType.VarBinary &&
                    DoCompressionForVarBinaryColumns)
                {
                    inputValue = "CompressionHelper.CompressBuffer(" + inputValue + ")";
                }

                WriteLine("DBLayer." + dbLayerMethod + "(command, " + "\"{0}\", SqlDbType.{1}, " + inputValue + ");", c.ColumnName, c.SqlDatabaseTypeName);
            }

            WriteLine("#endregion");
            WriteLine();

            if (Context.Table.HasIdentityColumn)
            {
                WriteLine("var sp = DBLayer.ExecuteScalar<int>(command);");
                WriteLine("if (!sp.Success)");
                WriteLine("{");
                Padding++;
                WriteLine("return returnObject.Add(sp);");
                Padding--;
                WriteLine("}");
            }
            else
            {
                WriteLine("var sp = DBLayer.ExecuteNonQuery(command);");
                WriteLine("if (!sp.Success)");
                WriteLine("{");
                Padding++;
                WriteLine("return returnObject.Add(sp);");
                Padding--;
                WriteLine("}");
            }

            WriteLine("returnObject.Value = sp.Value;");
            WriteLine();

            if (Context.Table.HasIdentityColumn)
            {
                WriteLine("contract.{0} = returnObject.Value;", Context.Table.IdentityColumn.ColumnName);
                WriteLine();
            }

            WriteLine("return returnObject;");
            Padding--;
            WriteLine("}");

            return GeneratedString;
        }
        #endregion

        #region Methods
        string GetDbLayerMethod(ColumnInfo columnInfo)
        {
            if (Context.Config.IsSecureColumn(columnInfo.ColumnName))
            {
                return "AddInSecureParameter";
            }

            return "AddInParameter";
        }

        List<ColumnInfo> GetProcedureParameterColumns()
        {
            return new InsertSql(Context).GetProcedureParameterColumns(Context.Table, Columns);
        }
        #endregion

        #region Parameters
        string MethodParameterCannotBeNullMessage => Context.Config.MethodParameterCannotBeNullMessage;

        string NameOfSqlProcedureInsert => Context.Naming.NameOfSqlProcedureInsert;

        string DatabaseTargetSchemaForProcedureNames => Context.Naming.SchemaName;

        string DatabaseEnumName => Context.Naming.DatabaseEnumName;

        string DatabaseTableFullPath => Context.Naming.DatabaseTableFullPath;

        string NameOfCsMethodInsert => Context.Naming.NameOfDotNetMethodInsert;

        string ContractName => Context.Naming.ContractName;

        IReadOnlyList<ColumnInfo> Columns => Context.Table.Columns;
        #endregion
    }
}