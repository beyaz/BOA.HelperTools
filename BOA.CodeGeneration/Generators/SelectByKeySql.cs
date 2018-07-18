using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    public class SelectByKeySql : WriterBase
    {
        #region Constructors
        #region Constructor
        public SelectByKeySql(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Public Properties
        public virtual string NameOfSqlProcedure => Context.Naming.NameOfSqlProcedureSelectByKey;
        #endregion

        #region Properties
        protected string DatabaseTableFullPath => Context.Config.DatabaseTableFullPath;

        protected IEnumerable<ColumnInfo> PrimaryKeyColumns => Context.Table.PrimaryKeyColumns;

        bool? CanGenerateSetNoCountOn => Context.Config.CanGenerateSetNoCountOnWhenSelectByKey == true || GetType() == typeof(SelectByKeyListSql);

        IReadOnlyList<ColumnInfo> Columns => Context.Table.Columns;

        string DatabaseTargetSchemaForProcedureNames => Context.Naming.SchemaName;

        string NameOfSqlProceduresWillBeRunCatalogName => Context.Naming.NameOfSqlProceduresWillBeRunCatalogName;
        #endregion

        #region Public Methods
        public string Generate()
        {
            WriteLine("USE {0}", NameOfSqlProceduresWillBeRunCatalogName);
            WriteLine("GO");
            WriteLine("");

            WriteLine("IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'P', N'PC'))",
                      DatabaseTargetSchemaForProcedureNames, NameOfSqlProcedure);

            Padding++;

            WriteLine("DROP PROCEDURE {0}", DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedure);

            Padding--;

            WriteLine("GO");
            WriteLine();

            WriteSqlComment(new List<string>
            {
                GetComment()
            });

            WriteLine("CREATE PROCEDURE " + DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedure);
            WriteLine("(");

            Padding++;

            WriteSqlParameters();

            Padding--;

            WriteLine(")");
            WriteLine("AS");

            if (CanGenerateSetNoCountOn == true)
            {
                WriteLine("SET NOCOUNT ON");
            }

            WriteLine("BEGIN");
            WriteLine();

            Padding++;
            WriteLine("SELECT");
            WriteLine();
            GenerateSqlSelectByColumns_WriteColumnsForReturn(Columns);

            WriteLine();

            WriteFromAndWherePart();

            Padding--;

            WriteLine();
            WriteLine("END");

            return GeneratedString;
        }
        #endregion

        #region Methods
        protected virtual string GetComment()
        {
            return "Select one record from '{0}'".FormatCode(DatabaseTableFullPath);
        }

        protected virtual void WriteFromAndWherePart()
        {
            var whereParameters = string.Join(" AND ", from c in PrimaryKeyColumns select c.ColumnName + " = @" + c.ColumnName);

            WriteLine("FROM {0} WITH (NOLOCK) WHERE {1}", Context.Config.TablePathForSqlScript, whereParameters);
        }

        protected virtual void WriteSqlParameters()
        {
            var maxNameLength = PrimaryKeyColumns.Max(c => c.ColumnName.Length) + PaddingLength;

            var maxDataTypeLength = PrimaryKeyColumns.Max(c => c.DataType.Length);

            var lastColumn = PrimaryKeyColumns.Last();

            foreach (var c in PrimaryKeyColumns)
            {
                var dataType = c.DataType;

                var value = "@" + c.ColumnName.PadRight(maxNameLength) + dataType.PadRight(maxDataTypeLength);

                if (c != lastColumn)
                {
                    value += ",";
                }

                WriteLine(value);
            }
        }
        #endregion
    }
}