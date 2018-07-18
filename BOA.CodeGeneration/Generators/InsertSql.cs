using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    public class InsertSql : WriterBase
    {
        #region Constructors
        public InsertSql(WriterContext context)
            : base(context)
        {
        }
        #endregion

        #region Properties
        bool CanGenerateSetNoCountOn
        {
            get
            {
                if (Context.Config.CanGenerateSetNoCountOnInsert)
                {
                    return true;
                }

                return !Context.Table.HasIdentityColumn;
            }
        }

        IReadOnlyList<ColumnInfo> Columns               => Context.Table.Columns;
        string                    DatabaseTableFullPath => Context.Config.DatabaseTableFullPath;

        string DatabaseTargetSchemaForProcedureNames => Context.Naming.SchemaName;

        string NameOfSqlProcedureInsert => Context.Naming.NameOfSqlProcedureInsert;

        string NameOfSqlProceduresWillBeRunCatalogName => Context.Naming.NameOfSqlProceduresWillBeRunCatalogName;
        #endregion

        #region Public Methods
        public string Generate()
        {
            WriteLine("USE {0}", NameOfSqlProceduresWillBeRunCatalogName);
            WriteLine("GO");
            WriteLine(string.Empty);

            WriteLine("IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'P', N'PC'))",
                      DatabaseTargetSchemaForProcedureNames, NameOfSqlProcedureInsert);

            Padding++;

            WriteLine("DROP PROCEDURE {0}", DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedureInsert);

            Padding--;

            WriteLine("GO");
            WriteLine("");

            var comments = new List<string>
            {
                "Insert new record into '{0}'".FormatCode(DatabaseTableFullPath)
            };
            WriteSqlComment(comments);

            WriteLine("CREATE PROCEDURE " + DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedureInsert);
            WriteLine("(");

            Padding++;
            WriteInsertParameters();
            Padding--;

            WriteLine(")");
            WriteLine("AS");
            if (CanGenerateSetNoCountOn)
            {
                WriteLine("SET NOCOUNT ON");
            }

            WriteLine("BEGIN");
            WriteLine();

            Padding++;
            WriteLine("INSERT INTO {0}", Context.Config.TablePathForSqlScript);
            WriteLine("(");

            Padding++;
            WriteInputParameters();
            Padding--;

            WriteLine(")");
            WriteLine("VALUES");

            WriteLine("(");
            Padding++;
            WriteValues();
            Padding--;
            WriteLine(")");
            WriteLine();

            if (Context.Table.HasIdentityColumn)
            {
                WriteLine("SELECT CAST(SCOPE_IDENTITY() AS INT)");
            }

            WriteLine();
            Padding--;
            WriteLine("END");

            return GeneratedString;
        }
        #endregion

        #region Methods
        internal List<ColumnInfo> GetProcedureParameterColumns(TableInfo table, IReadOnlyList<ColumnInfo> columns)
        {
            columns = columns ?? Columns;

            return columns.Where(c => !(Names.GenericUpdateInformationColumns.Contains(c.ColumnName) ||
                                        c.IsIdentity ||
                                        c.DataType == SqlDataType.Timestamp))
                          .Select(y => y)
                          .ToList();
        }

        List<ColumnInfo> GetInsertValuesColumns()
        {
            return Columns.Where(c => !(Names.GenericUpdateInformationColumns.Contains(c.ColumnName) ||
                                        c.IsIdentity ||
                                        c.DataType == SqlDataType.Timestamp))
                          .Select(y => y)
                          .ToList();
        }

        void WriteInputParameters()
        {
            var columns = GetInsertValuesColumns();

            var len = columns.Count;
            for (var i = 0; i < len; i++)
            {
                var row = columns[i];

                if (i < len - 1)
                {
                    WriteLine("{0},", row.ColumnName.NormalizeColumnNameForReversedKeywords());
                }
                else
                {
                    WriteLine("{0}", row.ColumnName.NormalizeColumnNameForReversedKeywords());
                }
            }
        }

        void WriteInsertParameters()
        {
            var columns = GetProcedureParameterColumns(Context.Table, Columns);

            var lastColumn = columns.Last();
            foreach (var c in columns)
            {
                var format = "@{0} {1}";
                if (c.IsNullable)
                {
                    format += " = NULL";
                }

                if (c != lastColumn)
                {
                    format += ",";
                }

                var dataType = c.DataType;
                if (dataType == SqlDataType.VarBinary)
                {
                    dataType = dataType + "(MAX)";
                }

                WriteLine(format, c.ColumnName, dataType);
            }
        }

        void WriteValues()
        {
            var columns = GetInsertValuesColumns();

            var lastRow = columns.Last();
            foreach (var row in columns)
            {
                var format = "@{0}";
                if (row != lastRow)
                {
                    format += ",";
                }

                WriteLine(format, row.ColumnName);
            }
        }
        #endregion
    }
}