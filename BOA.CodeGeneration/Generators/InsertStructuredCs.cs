using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.Common.Helpers;

namespace BOA.CodeGeneration.Generators
{
    class InsertStructuredCs : WriterBase
    {
        #region Constructors
        public InsertStructuredCs(WriterContext context)
            : base(context)
        {
        }
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

            WriteLine("{2}GenericResponse<int> {0}({1} items)", NameOfCsMethodInsert, ContractName, methodAccess);

            WriteLine("{");

            Padding++;

            WriteLine("var returnObject = CreateResponse<int>();", NameOfCsMethodInsert);
            WriteLine();
            WriteLine("if (items == null)");
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

            WriteLine("foreach (var contract in items)");
            WriteLine("{");
            Padding++;

            if (Columns.Any(c => c.ColumnName == Names.UserName))
            {
                WriteLine("contract.UserName = {0};", Names.UserNameValue);
            }

            if (Columns.Any(c => c.ColumnName == Names.HostName))
            {
                WriteLine("contract.HostName = {0};", Names.HostNameValue);
            }

            if (Columns.Any(c => c.ColumnName == Names.HostIP))
            {
                WriteLine("contract.HostIP = {0};", Names.HostIPValue);
            }

            if (Columns.Any(c => c.ColumnName == Names.SystemDate))
            {
                WriteLine("contract.SystemDate = {0};", Names.SystemDateValue);
            }

            Padding--;
            WriteLine("}");

            WriteLine();

            var columns = GetProcedureParameterColumns();

            WriteLine("var dt = new DataTable();");
            foreach (var c in columns)
            {
                var dataColumnName = $"nameof({Context.Naming.ContractName}.{c.ColumnName})";

                if (c.IsNullable)
                {
                    
                    WriteLine("dt.Columns.Add(new DataColumn(" + dataColumnName + ", typeof(" + c.DotNetType.RemoveFromEnd("?") + ")) { AllowDBNull = true});");
                }
                else
                {
                    WriteLine("dt.Columns.Add({0}, typeof({1}));", dataColumnName, c.DotNetType);
                }
            }

            WriteLine();
            WriteLine("foreach (var contract in items)");
            WriteLine("{");
            Padding++;

            WriteLine("var dataRow = dt.NewRow();");
            WriteLine();

            foreach (var c in columns)
            {
                var propertyName = c.ColumnName;
                var dataColumnName = $"nameof({Context.Naming.ContractName}.{c.ColumnName})";

                if (c.IsNullable)
                {
                    WriteLine("if (contract.{0} != null)", propertyName);
                    WriteLine("{");
                    Padding++;
                    WriteLine($"dataRow[{dataColumnName}] = contract.{propertyName};");
                    Padding--;
                    WriteLine("}");
                }
                else
                {
                    WriteLine($"dataRow[{dataColumnName}] = contract.{propertyName};");
                }
            }

            WriteLine("dt.Rows.Add(dataRow);");

            Padding--;
            WriteLine("}");

            WriteLine("DBLayer.AddInParameter(command, \"Value\",  SqlDbType.Structured, dt);");

            WriteLine("#endregion");
            WriteLine();

            WriteLine("var sp = DBLayer.ExecuteNonQuery(command);");
            WriteLine("if (!sp.Success)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.Add(sp);");
            Padding--;
            WriteLine("}");

            WriteLine();
            WriteLine("returnObject.Value = sp.Value;");
            WriteLine();

            WriteLine("return returnObject;");
            Padding--;
            WriteLine("}");

            return GeneratedString;
        }
        #endregion

        #region Methods
        List<ColumnInfo> GetProcedureParameterColumns()
        {
            return new InsertSql(Context).GetProcedureParameterColumns(Context.Table, Columns);
        }
        #endregion

        #region Parameters
        string MethodParameterCannotBeNullMessage => Context.Config.MethodParameterCannotBeNullMessage;

        string NameOfSqlProcedureInsert => InsertStructuredSql.NameOfSqlProcedureInsert;

        string DatabaseTargetSchemaForProcedureNames => Context.Naming.SchemaName;

        string DatabaseEnumName => Context.Naming.DatabaseEnumName;

        string DatabaseTableFullPath => Context.Naming.DatabaseTableFullPath;

        string NameOfCsMethodInsert => Context.Naming.NameOfDotNetMethodInsert;

        string ContractName => "IEnumerable<" + Context.Naming.ContractName + ">";

        IReadOnlyList<ColumnInfo> Columns => Context.Table.Columns;

        InsertStructuredSql InsertStructuredSql => new InsertStructuredSql(Context);
        #endregion
    }
}