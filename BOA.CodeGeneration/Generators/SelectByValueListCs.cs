using System;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    class SelectByValueListCs : SelectByKeyCs
    {
        #region Fields
        readonly string _columnName;
        readonly string _nameOfCSharpMethod;
        #endregion

        #region Constructors
        public SelectByValueListCs(WriterContext context, string columnName, string nameOfCSharpMethod)
            : base(context)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            _columnName         = columnName;
            _nameOfCSharpMethod = nameOfCSharpMethod;
        }
        #endregion

        #region Properties
        protected override string NameOfCSharpMethod => _nameOfCSharpMethod ?? Context.Naming.NameOfDotNetMethodSelectByValueListFormat.FormatCode(_columnName);

        protected override string NameOfSqlProcedure => Context.Naming.NameOfSqlProcedureSelectByValueListFormat.FormatCode(_columnName);

        bool ColumnIsString => ParameterColumn.DotNetType == Names.DotNetStringName;

        ColumnInfo ParameterColumn => Context.Table.Columns.First(c => c.ColumnName == GetColumnName());
        #endregion

        #region Methods
        protected override string GetGenericResponseValueType()
        {
            return "List<" + ContractName + ">";
        }

        protected override string GetMethodParametersPart()
        {
            if (ColumnIsString)
            {
                return "IReadOnlyCollection<string> " + GetMethodParameterName();
            }

            return "IReadOnlyCollection<int> " + GetMethodParameterName();
        }

        protected override void ProcessReturnValues()
        {
            WriteLine("var reader = sp.Value;");
            WriteLine();

            WriteLine("#region Fill from SqlDataReader to List<" + ContractName + ">");
            WriteLine("var listOfDataContract = new List<{0}>();", ContractName);
            WriteLine();
            
            WriteLine("while (reader.Read())");
            WriteLine("{");
            Padding++;
            WriteLine("var dataContract = new {0}();", ContractName);

            WriteLine();

            WriteLine("ReadContract(dataContract,reader);");

            WriteLine();
            WriteLine("listOfDataContract.Add(dataContract);");
            Padding--;
            WriteLine("}");
            WriteLine("reader.Close();");
            WriteLine("#endregion");

            WriteLine();
            WriteLine("returnObject.Value = listOfDataContract;");
            WriteLine();
        }

        protected override void WriteComment()
        {
            WriteLine("///" + PaddingForComment + "" + SelectByValueArraySql.GetComment(DatabaseTableFullPath, GetColumnName()));
        }

        protected override void WriteProcedureParameters()
        {
            var methodName = "GetInt32TableFromList";
            if (ColumnIsString)
            {
                methodName = "GetStringTableFromList";
            }

            WriteLine("DBLayer.AddInParameter(command, \"{0}\", SqlDbType.{1}, {2});",
                      GetSqlParameterName(), "Structured", "DBLayer." + methodName + "(" + GetMethodParameterName() + ".ToList())");
        }

        string GetColumnName()
        {
            return _columnName;
        }

        string GetMethodParameterName()
        {
            return (GetColumnName() + "List").AsMethodParameter();
        }

        string GetSqlParameterName()
        {
            return "@" + GetColumnName() + "List";
        }
        #endregion
    }
}