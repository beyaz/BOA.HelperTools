using BOA.DatabaseAccess;

namespace BOAPlugins.SearchProcedure
{
    static class Extensions
    {
        #region Methods
        internal static bool IsTable(this IDatabase db, string tableName, string schemaName)
        {
            db.CommandText = "SELECT TOP 1 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = @tableName AND TABLE_SCHEMA =@tableSchema";
            db["tableName"] = tableName;
            db["tableSchema"] = schemaName;
            var existsObj = db.ExecuteScalar();

            return existsObj + "" == "1";
        }
        #endregion
    }
}