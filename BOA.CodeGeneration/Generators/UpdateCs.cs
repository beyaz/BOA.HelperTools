using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Generators
{
    class UpdateCs : WriterBase
    {
        #region Constructors
        #region Constructor
        public UpdateCs(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Properties
        protected virtual string Comment                          => string.Format(CultureInfo, "Updates only one record of '{0}'", Context.Naming.DatabaseTableFullPath);
        bool                     DoCompressionForVarBinaryColumns => Context.Config.DoCompressionForVarBinaryColumns;
        #endregion

        #region Public Methods
        public string Generate()
        {
            Padding = PaddingForMethodDeclaration;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "{0}", Comment);
            WriteLine("/// </summary>");

            var methodAccess = "public ";
            if (MemberAccessibility == MemberAccessibility.Private)
            {
                methodAccess = "";
            }

            WriteLine("{2}GenericResponse<int> {0}({1} contract)", NameOfCsMethod, Context.Naming.ContractName, methodAccess);

            WriteLine("{");

            Padding++;

            WriteLine("var returnObject = CreateResponse<int>();");

            WriteLine();

            WriteLine("if (contract == null)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.AddError({0});", Context.Config.MethodParameterCannotBeNullMessage);
            Padding--;
            WriteLine("}");

            WriteLine();

            WriteLine("var command = DBLayer.GetDBCommand(Databases.{0}, \"{1}\");",
                      Context.Naming.DatabaseEnumName, Context.Naming.SchemaName + "." + NameOfSqlProcedure);

            WriteLine("");

            WriteLine("#region Parameters");

            WriteUpdateInformationColumnsForContract(Context.Table.Columns.Any(c => c.ColumnName == "UpdateUserName"),
                                                     Context.Table.Columns.Any(c => c.ColumnName == "UpdateHostName"),
                                                     Context.Table.Columns.Any(c => c.ColumnName == "UpdateHostIP"),
                                                     Context.Table.Columns.Any(c => c.ColumnName == "UpdateSystemDate"));

            foreach (var c in GetUpdateColumns())
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
        protected virtual IEnumerable<ColumnInfo> GetUpdateColumns()
        {
            return Context.Table.Columns.Where(c => !(
                                                   c.ColumnName == Names.UserName ||
                                                   c.ColumnName == Names.SystemDate ||
                                                   c.ColumnName == Names.HostName ||
                                                   c.ColumnName == Names.HostIP ||
                                                   c.IsIdentity && !c.IsPrimaryKey ||
                                                   c.DataType == SqlDataType.Timestamp))
                          .Select(y => y);
        }

        string GetDbLayerMethod(ColumnInfo columnInfo)
        {
            if (Context.Config.IsSecureColumn(columnInfo.ColumnName))
            {
                return "AddInSecureParameter";
            }

            return "AddInParameter";
        }
        #endregion

        #region Parameters
        protected virtual string NameOfSqlProcedure => Context.Naming.NameOfSqlProcedureUpdate;

        protected virtual string NameOfCsMethod => Context.Naming.NameOfDotNetMethodUpdate;

        protected virtual MemberAccessibility MemberAccessibility => MemberAccessibility.Public;
        #endregion
    }
}