using System;
using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    class SelectByColumnsSql : WriterBase
    {
        #region Constructors
        public SelectByColumnsSql(WriterContext context, CustomSelectMethod customSelect)
            : base(context)
        {
            CustomSelect = customSelect;

            ORDER_BY = customSelect.ORDER_BY;
            TOP_N    = customSelect.TOP_N;
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Indicates ORDER BY Expression.
        /// </summary>
        public string ORDER_BY { get; set; }

        public string SqlProcedureName => CustomSelect.SqlProcedureName;

        /// <summary>
        ///     Indicates how many records will be select.
        /// </summary>
        public int? TOP_N { get; set; }
        #endregion

        #region Properties
        IReadOnlyList<ColumnInfo> Columns => Context.Table.Columns;

        string Comment => CustomSelect.Comment;

        CustomSelectMethod CustomSelect { get; }

        string DatabaseTableFullPath => Context.Config.DatabaseTableFullPath;

        string DatabaseTargetSchemaForProcedureNames => Context.Naming.SchemaName;

        string NameOfSqlProceduresWillBeRunCatalogName => Context.Naming.NameOfSqlProceduresWillBeRunCatalogName;

        IReadOnlyList<Where> Parameters => CustomSelect.Parameters;
        #endregion

        #region Public Methods
        public string Generate()
        {
            WriteLine("USE {0}", NameOfSqlProceduresWillBeRunCatalogName);
            WriteLine("GO");
            WriteLine("");

            WriteLine("IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'P', N'PC'))",
                      DatabaseTargetSchemaForProcedureNames, SqlProcedureName);

            Padding++;

            WriteLine("DROP PROCEDURE {0}", DatabaseTargetSchemaForProcedureNames + "." + SqlProcedureName);

            Padding--;

            WriteLine("GO");
            WriteLine();

            if (Comment == null)
            {
                WriteSqlComment(new List<string>
                {
                    GetDefaultComment()
                });
            }
            else
            {
                WriteSqlComment((from line in Comment.Split(Environment.NewLine.ToCharArray())
                                 where !string.IsNullOrWhiteSpace(line)
                                 select line).ToList());
            }

            WriteLine("CREATE PROCEDURE " + DatabaseTargetSchemaForProcedureNames + "." + SqlProcedureName);

            if (Parameters.Any())
            {
                WriteLine("(");

                Padding++;

                var last = Parameters.Last();
                foreach (var p in Parameters)
                {
                    var column       = p.FindColum(Columns);
                    var propertyName = p.GetPropertyName(Columns);

                    WritePadding();

                    var dataType = column.DataType;
                    if (propertyName == Names.IsNull)
                    {
                        dataType = "TINYINT";
                    }

                    Write("@" + column.ColumnName + propertyName + " " + dataType);

                    if (p.CanBeNull)
                    {
                        Write(" = NULL ");
                    }

                    if (last == p)
                    {
                        break;
                    }

                    Write(",");
                    WriteLine();
                }

                Padding--;

                WriteLine();
                WriteLine(")");
            }

            WriteLine("AS");

            WriteLine("SET NOCOUNT ON");

            WriteLine("BEGIN");

            WriteLine();
            Padding++;
            WriteBody();

            Padding--;
            WriteLine();
            WriteLine("END");

            return GeneratedString;
        }
        #endregion

        #region Methods
        internal virtual string GetDefaultComment()
        {
            return "Selects records from '{0}'".FormatCode(DatabaseTableFullPath);
        }

        protected virtual void WriteBody()
        {
            WriteLine("SELECT");

            if (TOP_N.HasValue)
            {
                Write(" TOP " + TOP_N + " ");
            }

            if (CustomSelect.SelectWithStarIsEnabled)
            {
                Padding++;
                WriteLine(" * ");
                Padding--;
            }
            else
            {
                if (CustomSelect.SelectOnlySpecificColumn != null)
                {
                    GenerateSqlSelectByColumns_WriteColumnsForReturn(Columns.Where(c => c.ColumnName == CustomSelect.SelectOnlySpecificColumn).ToList());
                }
                else
                {
                    GenerateSqlSelectByColumns_WriteColumnsForReturn(Columns);
                }
            }

            WriteLine();

            WriteLine(" FROM {0} WITH (NOLOCK)", Context.Config.TablePathForSqlScript);

            WriteWherePart();

            if (ORDER_BY != null)
            {
                WriteLine("ORDER BY {0}", ORDER_BY);
            }
        }

        protected void WriteWherePart()
        {
            if (Parameters.Any())
            {
                WritePadding();
                Write("WHERE ");

                Padding++;

                var last  = Parameters.Last();
                var first = Parameters.First();
                foreach (var p in Parameters)
                {
                    var column       = p.FindColum(Columns);
                    var propertyName = p.GetPropertyName(Columns);

                    if (p != first)
                    {
                        WritePadding();
                    }

                    var line = GetWhereParameterAppliedLine(propertyName, column.ColumnName);
                    if (p.CanBeNull)
                    {
                        line = "( @{0} IS NULL OR {1} )".FormatCode(column.ColumnName + propertyName, line);
                    }

                    Write(line);

                    if (last == p)
                    {
                        break;
                    }

                    Write(" AND");
                    WriteLine();
                }

                Padding--;
            }
        }

        static string GetWhereParameterAppliedLine(string propertyName, string columnName)
        {
            if (propertyName == null)
            {
                return "{0} = @{0}".FormatCode(columnName);
            }

            if (propertyName == Names.NotEqual)
            {
                return ("{0} <> @{0}" + propertyName).FormatCode(columnName);
            }

            if (propertyName == Names.BiggerThan)
            {
                return ("{0} > @{0}" + propertyName).FormatCode(columnName);
            }

            if (propertyName == Names.BiggerThanOrEquals)
            {
                return ("{0} >= @{0}" + propertyName).FormatCode(columnName);
            }

            if (propertyName == Names.LessThan)
            {
                return ("{0} < @{0}" + propertyName).FormatCode(columnName);
            }

            if (propertyName == Names.LessThanOrEquals)
            {
                return ("{0} <= @{0}" + propertyName).FormatCode(columnName);
            }

            if (propertyName == Names.StartsWith)
            {
                return ("{0} LIKE @{0} + '%'" + propertyName).FormatCode(columnName);
            }

            if (propertyName == Names.EndsWith)
            {
                return ("{0} LIKE '%' + @{0}" + propertyName).FormatCode(columnName);
            }

            if (propertyName == Names.Contains)
            {
                return ("{0} LIKE '%' + @{0} + '%'" + propertyName).FormatCode(columnName);
            }

            if (propertyName == Names.IsNull)
            {
                var parameterName = ("@{0}" + propertyName).FormatCode(columnName);

                return "(( {1} = 1 AND {0} IS NULL ) OR ( {1} = 0 AND {0} IS NOT NULL ))".FormatCode(columnName, parameterName);
            }

            throw new Exception(propertyName);
        }
        #endregion
    }
}