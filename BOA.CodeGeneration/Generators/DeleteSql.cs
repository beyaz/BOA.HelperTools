using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Generators
{
    class DeleteSql : WriterBase
    {
        #region Constructors
        #region Constructor
        public DeleteSql(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Properties
        bool? CanGenerateSetNoCountOn => Context.Config.CanGenerateSetNoCountOnWhenDeleteByKey;

        string DatabaseTableFullPath => Context.Config.DatabaseTableFullPath;

        string DatabaseTargetSchemaForProcedureNames => Context.Naming.SchemaName;

        string NameOfSqlProcedureDelete => Context.Naming.NameOfSqlProcedureDelete;

        string NameOfSqlProceduresWillBeRunCatalogName => Context.Naming.NameOfSqlProceduresWillBeRunCatalogName;

        IEnumerable<ColumnInfo> PrimaryKeyColumns => Context.Table.PrimaryKeyColumns;
        #endregion

        #region Public Methods
        public string Generate()
        {
            WriteLine("USE {0}", NameOfSqlProceduresWillBeRunCatalogName);
            WriteLine("GO");
            WriteLine("");

            WriteLine("IF EXISTS (SELECT TOP 1 1 FROM sys.objects WHERE object_id = OBJECT_ID(N'[{0}].[{1}]') AND type in (N'P', N'PC'))",
                      DatabaseTargetSchemaForProcedureNames, NameOfSqlProcedureDelete);

            Padding++;

            WriteLine("DROP PROCEDURE {0}", DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedureDelete);

            Padding--;

            WriteLine("GO");
            WriteLine("");
            WriteLine("/*");
            Padding++;
            WriteLine("Deletes only one record from '{0}'", DatabaseTableFullPath);
            Padding--;
            WriteLine("*/");
            WriteLine("CREATE PROCEDURE " + DatabaseTargetSchemaForProcedureNames + "." + NameOfSqlProcedureDelete);
            WriteLine("(");

            Padding++;

            var maxNameLength     = PrimaryKeyColumns.Max(c => c.ColumnName.Length) + PaddingLength;
            var maxDataTypeLength = PrimaryKeyColumns.Max(c => c.DataType.Length);
            var lastColumn        = PrimaryKeyColumns.Last();
            foreach (var c in PrimaryKeyColumns)
            {
                if (c == lastColumn)
                {
                    WriteLine("@" + c.ColumnName.PadRight(maxNameLength) + c.DataType.PadRight(maxDataTypeLength));
                }
                else
                {
                    WriteLine("@" + c.ColumnName.PadRight(maxNameLength) + c.DataType.PadRight(maxDataTypeLength) + ",");
                }
            }

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
            var whereParameters = string.Join(" AND ",
                                              from c in PrimaryKeyColumns select c.ColumnName + " = @" + c.ColumnName);
            WriteLine("DELETE FROM {0} WHERE {1}", Context.Config.TablePathForSqlScript, whereParameters);
            Padding--;

            WriteLine();
            WriteLine("END");

            return GeneratedString;
        }
        #endregion
    }
}