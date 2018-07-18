using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Generators
{
    public class UpdateSql : WriterBase
    {
        #region Constructors
        #region Constructor
        public UpdateSql(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Properties
        protected virtual string Comment => string.Format(CultureInfo, "Updates only one record of '{0}'", DatabaseTableFullPath);

        protected virtual string NameOfSqlProcedureUpdate => Context.Naming.NameOfSqlProcedureUpdate;

        protected virtual IEnumerable<ColumnInfo> ProcedureParameters
        {
            get
            {
                return Columns.Where(c => !(
                                         c.ColumnName == Names.UserName ||
                                         c.ColumnName == Names.HostName ||
                                         c.ColumnName == Names.SystemDate ||
                                         c.ColumnName == Names.HostIP ||
                                         c.IsIdentity && !c.IsPrimaryKey ||
                                         c.DataType == SqlDataType.Timestamp))
                              .Select(y => y)
                              .ToList();
            }
        }

        protected virtual IEnumerable<ColumnInfo> WhereColumns => Context.Table.PrimaryKeyColumns;

        bool CanGenerateSetNoCountOn
        {
            get
            {
                if (Context.Config.CanGenerateSetNoCountOnUpdate)
                {
                    return true;
                }

                return !Context.Table.HasIdentityColumn;
            }
        }

        IReadOnlyList<ColumnInfo> Columns => Context.Table.Columns;

        string DatabaseTableFullPath => Context.Config.DatabaseTableFullPath;

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
                      DatabaseTargetSchemaForProcedureNames, NameOfSqlProcedureUpdate);

            Padding++;

            WriteLine("DROP PROCEDURE {0}", DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedureUpdate);

            Padding--;

            WriteLine("GO");
            WriteLine();

            WriteSqlComment(new List<string>
            {
                Comment
            });

            WriteLine("CREATE PROCEDURE " + DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedureUpdate);
            WriteLine("(");

            Padding++;

            var lastColumn = ProcedureParameters.Last();
            foreach (var c in ProcedureParameters)
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

            Padding--;

            WriteLine(")");
            WriteLine("AS");
            if (CanGenerateSetNoCountOn)
            {
                WriteLine("SET NOCOUNT ON");
            }

            WriteLine("BEGIN");
            WriteLine("");

            Padding++;
            WriteLine("UPDATE T SET");

            Padding++;
            lastColumn = GetUpdateColumns().Last();
            foreach (var c in GetUpdateColumns())
            {
                var format = "T.{0} = @{0}";

                if (c != lastColumn)
                {
                    format += ",";
                }

                WriteLine(format, c.ColumnName.NormalizeColumnNameForReversedKeywords());
            }

            Padding--;

            WriteLine();
            Padding++;
            var whereParameters = string.Join(" AND ", from c in WhereColumns select "T." + c.ColumnName + " = @" + c.ColumnName);
            WriteLine("FROM {0} AS T WHERE {1}", Context.Config.TablePathForSqlScript, whereParameters);
            Padding--;

            WriteLine("");
            Padding--;
            WriteLine("END");

            return GeneratedString;
        }
        #endregion

        #region Methods
        protected virtual IEnumerable<ColumnInfo> GetUpdateColumns()
        {
            return Columns.Where(c => !(
                                     c.ColumnName == Names.UserName ||
                                     c.ColumnName == Names.HostName ||
                                     c.ColumnName == Names.SystemDate ||
                                     c.ColumnName == Names.HostIP ||
                                     c.IsIdentity ||
                                     c.DataType == SqlDataType.Timestamp))
                          .Select(y => y);
        }
        #endregion
    }
}