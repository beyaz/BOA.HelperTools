using System;
using System.Globalization;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    class SelectByValueArraySql : SelectByKeySql
    {
        #region Static Fields
        new static readonly CultureInfo CultureInfo = new CultureInfo("en-US");
        #endregion

        #region Fields
        readonly string _columnName;
        #endregion

        #region Constructors
        public SelectByValueArraySql(WriterContext context, string columnName)
            : base(context)
        {
            if (columnName == null)
            {
                throw new ArgumentNullException(nameof(columnName));
            }

            _columnName = columnName;
        }
        #endregion

        #region Public Properties
        public override string NameOfSqlProcedure => Context.Naming.NameOfSqlProcedureSelectByValueListFormat.FormatCode(_columnName);
        #endregion

        #region Properties
        bool       ColumnIsString  => ParameterColumn.DotNetType == Names.DotNetStringName;
        ColumnInfo ParameterColumn => Context.Table.Columns.First(c => c.ColumnName == GetColumnName());

        string ParameterType
        {
            get
            {
                if (ColumnIsString)
                {
                    return "dbo.TpVarchar50Table";
                }

                return "dbo.TpIntTable";
            }
        }
        #endregion

        #region Methods
        internal static string GetComment(string databaseTableFullPath, string columnName)
        {
            return string.Format(CultureInfo, "Selects records from '{0}' by using given '{1}' column values", databaseTableFullPath, columnName);
        }

        protected override string GetComment()
        {
            return GetComment(DatabaseTableFullPath, _columnName);
        }

        protected override void WriteFromAndWherePart()
        {
            WriteLine("FROM {0} AS a WITH (NOLOCK) INNER JOIN {1} AS b ON a.{2} = b.Value",
                      Context.Config.TablePathForSqlScript,
                      GetSqlParameterName(),
                      GetColumnName());
        }

        protected override void WriteSqlParameters()
        {
            WriteLine($"{GetSqlParameterName()} {ParameterType} READONLY");
        }

        string GetColumnName()
        {
            return _columnName;
        }

        string GetSqlParameterName()
        {
            return "@" + GetColumnName() + "List";
        }
        #endregion
    }
}