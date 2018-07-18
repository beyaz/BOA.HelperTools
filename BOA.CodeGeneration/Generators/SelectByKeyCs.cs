using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    class SelectByKeyCs : WriterBase
    {
        #region Constructors
        #region Constructor
        public SelectByKeyCs(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Public Methods
        public string Generate()
        {
            Padding = PaddingForMethodDeclaration;
            WriteLine("/// <summary>");

            WriteComment();

            WriteLine("/// </summary>");

            WriteLine("public GenericResponse<{2}> {0}({1})", NameOfCSharpMethod, GetMethodParametersPart(), GetGenericResponseValueType());

            WriteLine("{");

            Padding++;

            WriteLine("var returnObject = CreateResponse<{0}>();", GetGenericResponseValueType());

            WriteLine();

            WriteLine("var command = DBLayer.GetDBCommand(Databases.{0}, \"{1}\");",
                      DatabaseEnumName, DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedure);

            WriteLine("");

            WriteLine("#region Parameters");

            WriteProcedureParameters();

            WriteLine("#endregion");

            WriteLine();
            WriteLine("var sp = DBLayer.ExecuteReader(command);");
            WriteLine("if (!sp.Success)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.Add(sp);");
            Padding--;
            WriteLine("}");
            WriteLine();

            ProcessReturnValues();

            WriteLine("return returnObject;");
            Padding--;
            WriteLine("}");

            return GeneratedString;
        }
        #endregion

        #region Methods
        protected virtual string GetGenericResponseValueType()
        {
            if (Context.Config.SelectByKeyMustBeReturnReadonlyContract)
            {
                return "IReadOnly" + ContractName;
            }

            return ContractName;
        }

        protected virtual string GetMethodParametersPart()
        {
            return string.Join(" , ", from c in PrimaryKeyColumns
                                      select c.DotNetType + " " + c.ColumnName.AsMethodParameter());
        }

        protected virtual void ProcessReturnValues()
        {
            WriteLine("var reader = sp.Value;");
            WriteLine();

            WriteLine("#region Fill from SqlDataReader to DataContract");
          
            WriteLine("{0} dataContract = null;", ContractName);
            WriteLine();
            WriteLine("while (reader.Read())");
            WriteLine("{");
            Padding++;
            WriteLine("dataContract = new {0}();", ContractName);

            WriteLine();

            WriteLine("ReadContract(dataContract,reader);");

            WriteLine();
            WriteLine("break;");
            Padding--;
            WriteLine("}");
            WriteLine("reader.Close();");
            WriteLine("#endregion");

            WriteLine();
            WriteLine("returnObject.Value = dataContract;");
            WriteLine();
        }

        protected virtual void WriteComment()
        {
            var primaryKeys = string.Join(" - ", from c in PrimaryKeyColumns
                                                 select c.ColumnName);

            WriteLine("///" + PaddingForComment + "Selects only one record from '{0}' by using primary key '{1}'", DatabaseTableFullPath, primaryKeys);
        }

        protected virtual void WriteProcedureParameters()
        {
            foreach (var c in PrimaryKeyColumns)
            {
                WriteLine("DBLayer.AddInParameter(command, \"{0}\", SqlDbType.{1}, {2});",
                          c.ColumnName, c.SqlDatabaseTypeName, c.ColumnName.AsMethodParameter());
            }
        }
        #endregion

        #region Parameters
        protected virtual string NameOfSqlProcedure => Context.Naming.NameOfSqlProcedureSelectByKey;

        string DatabaseTargetSchemaForProcedureNames => Context.Naming.SchemaName;

        string DatabaseEnumName => Context.Naming.DatabaseEnumName;

        protected IEnumerable<ColumnInfo> PrimaryKeyColumns => Context.Table.PrimaryKeyColumns;

        protected string DatabaseTableFullPath => Context.Naming.DatabaseTableFullPath;

        protected virtual string NameOfCSharpMethod => Context.Naming.NameOfDotNetMethodSelectByKey;

        protected string ContractName => Context.Naming.ContractName;
        #endregion
    }
}