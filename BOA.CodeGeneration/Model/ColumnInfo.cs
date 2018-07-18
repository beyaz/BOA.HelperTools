using BOA.CodeGeneration.Common;
using WhiteStone.Common;

namespace BOA.CodeGeneration.Model
{
    public sealed class ColumnInfo : ContractBase
    {
        #region Constructors
        public ColumnInfo(string columnName, bool isNullable, bool isIdentity, string dataType)
        {
            ColumnName = columnName;
            IsNullable = isNullable;
            IsIdentity = isIdentity;
            DataType   = dataType;

            SqlDatabaseTypeName = SqlDataType.GetSqlDbType(DataType);
            DotNetType          = SqlDataType.GetDotNetType(DataType, IsNullable);
            SqlReaderMethod     = SqlDataType.GetSqlReaderMethod(DataType, IsNullable);
        }
        #endregion

        #region Public Properties
        public string ColumnName   { get; }
        public string Comment      { get; set; }
        public string DataType     { get; }
        public string DotNetType   { get; }
        public bool   IsIdentity   { get; }
        public bool   IsNullable   { get; }
        public bool   IsPrimaryKey { get; set; }

        public string SqlDatabaseTypeName { get; }
        public string SqlReaderMethod     { get; }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return DotNetType + " " + ColumnName;
        }
        #endregion
    }
}