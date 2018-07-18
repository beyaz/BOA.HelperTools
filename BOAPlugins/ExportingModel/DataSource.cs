using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using BOA.DatabaseAccess;
using WhiteStone.Helpers;

namespace BOAPlugins.ExportingModel
{
    class PropertyInfo
    {
        #region Public Properties
        public string EN_Description { get; set; }

        public string PropertyName { get; set; }

        public string TR_Description { get; set; }
        #endregion
    }
    class DataSource
    {
        #region Public Methods
        public static IList<PropertyInfo> GetPropertyNames(string groupName)
        {
            var sqlDatabaseLayer = new SqlDatabaseLayer
            {
                GetConnectionByConnectionName = GetConnection
            };

            var sql =
                @"
	SELECT	distinct(PropertyName),
			md.Description	AS TR_Description,
			md2.Description AS EN_Description 
	  FROM COR.Messaging         m WITH(NOLOCK) 
INNER JOIN COR.MessagingDetail  md WITH(NOLOCK) ON m.Code = md.Code  AND md.LanguageId  = 1
 LEFT JOIN COR.MessagingDetail md2 WITH(NOLOCK) ON m.Code = md2.Code AND md2.LanguageId = 2
 WHERE MessagingGroupId = ( SELECT MessagingGroupId 
							  FROM Cor.MessagingGroup WITH(NOLOCK) 
							 WHERE Name = @groupName )

";
            var command = sqlDatabaseLayer.GetDBCommand("", sql);
            command.CommandType = CommandType.Text;
            sqlDatabaseLayer.AddInParameter(command, "groupName", SqlDbType.VarChar, groupName);

            return sqlDatabaseLayer.ExecuteReader(command).ToList<PropertyInfo>();
        }
        #endregion

        #region Methods
        static DbConnection GetConnection(string name)
        {
            const string ConnectionStringForDEV = @"server=srvdev\ATLAS;database =BOA;integrated security=true";

            return new SqlConnection(ConnectionStringForDEV);
        }
        #endregion
    }
}