using System;
using System.Data;
using BOA.CodeGeneration.Util;
using BOA.Common.Helpers;
using BOA.DatabaseAccess;
using WhiteStone.Helpers;

namespace BOA.CodeGeneration.Services
{
    public sealed class DataAccess : IDisposable
    {
        #region Constructors
        public DataAccess(string serverName, string catalog)
        {
            var connection = "Server={0};Database={1};Trusted_Connection=True;".FormatCode(serverName, catalog);

            Database = new SqlDatabase(connection);
        }

        public DataAccess(IDatabase database)
        {
            Database = database;
        }
        #endregion

        #region Public Properties
        public IDatabase Database { get; }
        #endregion

        #region Public Methods
        public void Dispose()
        {
            Database.Dispose();
        }

        public string GetColumnComment(string schemaName, string tableName, string columnName)
        {
            var sql = Database;

            sql.CommandText =
                @"SELECT TOP 1 value
    FROM sys.extended_properties
   WHERE major_id = OBJECT_ID('{0}.{1}') AND
         minor_id = COLUMNPROPERTY(major_id, '{2}', 'ColumnId')".FormatCode(schemaName, tableName, columnName);

            return (string) sql.ExecuteScalar();
        }

        public DataTable GetPrimaryColumns(string tableSchema, string tableName)
        {
            var sql = Database;

            sql.CommandText =
                @"SELECT Col.Column_Name from
    INFORMATION_SCHEMA.TABLE_CONSTRAINTS Tab,
    INFORMATION_SCHEMA.CONSTRAINT_COLUMN_USAGE Col ,
    (select NAME from dbo.sysobjects where xtype='u') AS A
WHERE
    Col.Constraint_Name = Tab.Constraint_Name
    AND Col.Table_Name = Tab.Table_Name
    AND Constraint_Type = 'PRIMARY KEY '
    AND Col.Table_Name = A.NAME
	AND Col.TABLE_SCHEMA = @TABLE_SCHEMA
	AND Col.TABLE_NAME = @TABLE_NAME";

            sql["TABLE_SCHEMA"] = tableSchema;
            sql["TABLE_NAME"]   = tableName;

            return sql.ExecuteReader().ToDataTable();
        }

        public string GetProcedureDefinition(string schemaName, string procedureName)
        {
            var sql =
                @"
SELECT sm.definition
	  FROM sys.sql_modules sm WITH(NOLOCK) 
INNER JOIN sys.objects      o WITH(NOLOCK) ON sm.object_id = o.object_id  
     WHERE o.name = @procedureName
       AND SCHEMA_NAME(O.schema_id) = @schemaName
";

            Database.CommandText      = sql;
            Database["procedureName"] = procedureName;
            Database["schemaName"]    = schemaName;

            return Cast.To<string>(Database.ExecuteScalar());
        }

        public DataTable GetTableInformation(string tableCatalog, string tableSchema, string tableName)
        {
            var sql = Database;
            sql.CommandText =
                @"SELECT c.*,  (SELECT  TOP 1 i.name
				  FROM   sys.tables			AS t INNER JOIN
						 sys.indexes	    AS i ON t.object_id = i.object_id INNER JOIN
						 sys.index_columns  AS IC ON i.object_id = IC.object_id AND i.index_id = IC.index_id INNER JOIN
						 sys.all_columns    AS AC ON t.object_id = AC.object_id AND IC.column_id = AC.column_id

				  WHERE   IC.key_ordinal = 1 AND t.name = @TABLE_NAME AND AC.name = c.COLUMN_NAME ) AS IndexName,

			 (SELECT  TOP 1 AC.is_identity
				  FROM   sys.tables			 AS t INNER JOIN
						 sys.all_columns     AS AC ON t.object_id = AC.object_id

				  WHERE  t.name = @TABLE_NAME AND AC.name = c.COLUMN_NAME ) AS IsIdentity

  FROM INFORMATION_SCHEMA.COLUMNS c
 WHERE  TABLE_NAME    = @TABLE_NAME 
   AND (TABLE_CATALOG = @TABLE_CATALOG OR @TABLE_CATALOG IS NULL) 
   AND  TABLE_SCHEMA  = @TABLE_SCHEMA
";
            sql["TABLE_NAME"]    = tableName;
            sql["TABLE_CATALOG"] = tableCatalog;
            sql["TABLE_SCHEMA"]  = tableSchema;

            return sql.ExecuteReader().ToDataTable();
        }

        public bool TableExists(string schemaName, string tableName)
        {
            var sql = Database;
            sql.CommandText = "SELECT COUNT(*) " +
                              "  FROM INFORMATION_SCHEMA.TABLES " +
                              " WHERE TABLE_NAME   = @tableName AND " +
                              "       TABLE_SCHEMA = @schemaName ";
            sql["tableName"]  = tableName;
            sql["schemaName"] = schemaName;
            return Cast.To<int>(sql.ExecuteScalar()) > 0;
        }
        #endregion
    }
}