using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    class DeleteCs : WriterBase
    {
        #region Constructors
        #region Constructor
        public DeleteCs(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Properties
        string ParametersPart => string.Join(" , ", from c in PrimaryKeyColumns
                                                    select c.DotNetType + " " + c.ColumnName.AsMethodParameter());
        #endregion

        #region Public Methods
        public string Generate()
        {
            var primaryKeys = string.Join(" - ", from c in PrimaryKeyColumns
                                                 select c.ColumnName);

            Padding = PaddingForMethodDeclaration;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Deletes only one record from '{0}' by using field '{1}'", DatabaseTableFullPath, primaryKeys);
            WriteLine("/// </summary>");

            WriteLine("public GenericResponse<int> {0}({1})", NameOfCsMethodDelete, ParametersPart);

            WriteLine("{");

            Padding++;

            WriteLine("var returnObject = CreateResponse<int>();");

            WriteLine();

            WriteLine("var command = DBLayer.GetDBCommand(Databases.{0}, \"{1}\");",
                      DatabaseEnumName, DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedureDelete);

            WriteLine("");

            WriteLine("#region Parameters");

            foreach (var c in PrimaryKeyColumns)
            {
                WriteLine("DBLayer.AddInParameter(command, \"{0}\", SqlDbType.{1}, {2});",
                          c.ColumnName, c.SqlDatabaseTypeName, c.ColumnName.AsMethodParameter());
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

        #region Parameters
        string NameOfSqlProcedureDelete => Context.Naming.NameOfSqlProcedureDelete;

        string DatabaseTargetSchemaForProcedureNames => Context.Naming.SchemaName;

        string DatabaseEnumName => Context.Naming.DatabaseEnumName;

        IEnumerable<ColumnInfo> PrimaryKeyColumns => Context.Table.PrimaryKeyColumns;

        string DatabaseTableFullPath => Context.Naming.DatabaseTableFullPath;

        string NameOfCsMethodDelete => Context.Naming.NameOfDotNetMethodDelete;
        #endregion
    }
}