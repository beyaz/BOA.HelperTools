using System;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.SQLParser;

namespace BOA.CodeGeneration.Util
{
    static class IProcedureParameterExtensions
    {
        #region Public Methods
        public static object GetDefaultValueForCallingProcedure(this IProcedureParameter procedureParameter)
        {
            if (procedureParameter.IsNullable)
            {
                return null;
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.VARCHAR, StringComparison.Ordinal))
            {
                return "''";
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.Int, StringComparison.Ordinal))
            {
                return 0;
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.DateTime, StringComparison.Ordinal) ||
                procedureParameter.SqlDataType.StartsWith(SqlDataType.Date, StringComparison.Ordinal))
            {
                return DateTime.MinValue;
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.SmallDateTime, StringComparison.Ordinal))
            {
                return DateTime.Today.AddDays(-1).Date;
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.SmallInt, StringComparison.Ordinal))
            {
                return (short) 0;
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.Bit, StringComparison.Ordinal))
            {
                return false;
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.BigInt, StringComparison.Ordinal))
            {
                return (long) 0;
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.Binary, StringComparison.Ordinal))
            {
                return new byte[0];
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.Float, StringComparison.Ordinal))
            {
                return (float) 0;
            }

            if (procedureParameter.SqlDataType.StartsWith(SqlDataType.Money, StringComparison.Ordinal) ||
                procedureParameter.SqlDataType.StartsWith(SqlDataType.SmallMoney, StringComparison.Ordinal))
            {
                return (decimal) 0;
            }

            throw new NotImplementedException("Not implemented yet." + procedureParameter.SqlDataType);
        }

        public static string GetDotNetType(this IProcedureParameter procedureParameter)
        {
            return SqlDataType.GetDotNetType(procedureParameter.SqlDataType, procedureParameter.IsNullable);
        }
        #endregion
    }
}