using System;
using System.Data;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.Common.Helpers;

namespace BOA.CodeGeneration.Services
{
    public static class TableInfoGeneratorFromMsSql
    {
        #region Public Methods
        public static TableInfo CreateTable(DataTable table)
        {
            var info = new TableInfo();

            var len = table.Rows.Count;
            for (var i = 0; i < len; i++)
            {
                var row = table.Rows[i];

                info.AddColumn(CreateColumn(row));
            }

            return info;
        }
        #endregion

        #region Methods
        static ColumnInfo CreateColumn(DataRow row)
        {
            var columnName = row["COLUMN_NAME"].ToString();

            #region  var dataType = row["DATA_TYPE"
            var dataType = row["DATA_TYPE"].ToString().ToUpperEN();
            if (dataType == "VARCHAR" || dataType == "NVARCHAR" || dataType == "CHAR" || dataType == "NCHAR")
            {
                var characterLength = row["CHARACTER_MAXIMUM_LENGTH"].ToString();
                if (characterLength == "-1")
                {
                    characterLength = "MAX";
                }

                dataType = dataType + "(" + characterLength + ")";
            }
            else if (dataType == SqlDataType.SmallDateTime ||
                     dataType == SqlDataType.TinyInt ||
                     dataType == SqlDataType.DateTime ||
                     dataType == SqlDataType.Date ||
                     dataType == SqlDataType.SmallInt ||
                     dataType == SqlDataType.Int ||
                     dataType == SqlDataType.Bit ||
                     dataType == SqlDataType.BigInt ||
                     dataType == SqlDataType.Money ||
                     dataType == SqlDataType.UniqueIdentifier ||
                     dataType == SqlDataType.Float ||
                     dataType == SqlDataType.SmallMoney ||
                     dataType == SqlDataType.Image ||
                     dataType == SqlDataType.VarBinary ||
                     dataType == SqlDataType.Binary ||
                     dataType == SqlDataType.Real ||
                     dataType == SqlDataType.NText ||
                     dataType == SqlDataType.Text ||
                     dataType == SqlDataType.Time ||
                     dataType == SqlDataType.Xml ||
                     dataType == SqlDataType.Timestamp)
            {
            }
            else if (dataType == "NUMERIC")
            {
                var numericPrecision = row["NUMERIC_PRECISION"] + "";
                var numericScale     = row["NUMERIC_SCALE"] + "";

                if (numericPrecision.IsNullOrWhiteSpace())
                {
                    throw new InvalidOperationException(nameof(numericPrecision))
                        .AddData(nameof(columnName), columnName);
                }

                if (numericScale.IsNullOrWhiteSpace())
                {
                    throw new InvalidOperationException(nameof(numericScale))
                        .AddData(nameof(columnName), columnName);
                }

                dataType = dataType + "(" + numericPrecision + "," + numericScale + ")";
            }
            else if (dataType == "DECIMAL")
            {
                var numericPrecision = row["NUMERIC_PRECISION"] + "";
                var numericScale     = row["NUMERIC_SCALE"] + "";

                if (numericPrecision.IsNullOrWhiteSpace())
                {
                    throw new InvalidOperationException(nameof(numericPrecision))
                        .AddData(nameof(columnName), columnName);
                }

                if (numericScale.IsNullOrWhiteSpace())
                {
                    throw new InvalidOperationException(nameof(numericScale))
                        .AddData(nameof(columnName), columnName);
                }

                dataType = "NUMERIC" + "(" + numericPrecision + "," + numericScale + ")";
            }
            else
            {
                throw new ArgumentException(dataType);
            }
            #endregion

            return new ColumnInfo(columnName,
                                  row["IS_NULLABLE"].ToString() == "YES",
                                  row["IsIdentity"] + "" == "True",
                                  dataType);
        }
        #endregion
    }
}