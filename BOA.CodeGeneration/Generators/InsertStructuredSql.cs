using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    public class InsertStructuredSql : WriterBase
    {
        #region Constants
        public const string Structured = "Structured";
        const        string Dot        = ".";

        const string TableType = "TableType";
        #endregion

        #region Constructors
        public InsertStructuredSql(WriterContext context)
            : base(context)
        {
        }
        #endregion

        #region Public Properties
        public string NameOfSqlProcedureInsert => Context.Naming.NameOfSqlProcedureInsert + Structured;
        #endregion

        #region Properties
        IReadOnlyList<ColumnInfo> Columns => Context.Table.Columns;

        string DatabaseTableFullPath => Context.Config.DatabaseTableFullPath;

        string DatabaseTargetSchemaForProcedureNames => Context.Naming.SchemaName;

        string NameOfSqlProceduresWillBeRunCatalogName => Context.Naming.NameOfSqlProceduresWillBeRunCatalogName;

        string ParameterTypeName => Context.Config.TablePathForSqlScript + TableType;
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
            WriteLine();

            // Drop Parameter Type
            WriteLine("IF TYPE_ID(N'{0}') IS NOT NULL", ParameterTypeName);
            Padding++;
            WriteLine("DROP Type {0}", ParameterTypeName);
            Padding--;
            WriteLine("GO");
            WriteLine();

            WriteLine("CREATE TYPE " + ParameterTypeName + " AS TABLE");
            WriteLine("(");
            Padding++;
            WriteInsertParameters();
            Padding--;
            WriteLine(")");

            WriteLine();
            WriteLine("GO");
            WriteLine();

            var comments = new List<string>
            {
                "Insert new record into '{0}'".FormatCode(DatabaseTableFullPath)
            };
            WriteSqlComment(comments);

            WriteLine("CREATE PROCEDURE " + DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedureInsert);
            WriteLine("(");

            Padding++;
            WriteLine("@Value {0} READONLY ", ParameterTypeName);
            Padding--;

            WriteLine(")");
            WriteLine("AS");
            WriteLine("SET NOCOUNT ON");
            WriteLine("BEGIN");
            WriteLine();

            Padding++;
            WriteLine("INSERT INTO {0}", Context.Config.TablePathForSqlScript);
            WriteLine("(");

            Padding++;
            WriteInputParameters();
            Padding--;

            WriteLine(")");
            WriteLine("SELECT");

            Padding++;
            WriteValues();
            Padding--;
            WriteLine();
            WriteLine("FROM @Value");

            WriteLine();
            Padding--;
            WriteLine("END");

            return GeneratedString;
        }
        #endregion

        #region Methods
        List<ColumnInfo> GetInsertValuesColumns()
        {
            return Columns.Where(c => !(Names.GenericUpdateInformationColumns.Contains(c.ColumnName) ||
                                        c.IsIdentity ||
                                        c.DataType == SqlDataType.Timestamp))
                          .Select(y => y)
                          .ToList();
        }

        List<ColumnInfo> GetProcedureParameterColumns(TableInfo table, IReadOnlyList<ColumnInfo> columns)
        {
            columns = columns ?? Columns;

            return columns.Where(c => !(Names.GenericUpdateInformationColumns.Contains(c.ColumnName) ||
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
                var format = "{0} {1}";
                if (c.IsNullable)
                {
                    format += "  NULL";
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
                var format = "{0}";
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