using System.Collections.Generic;
using System.IO;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Util;
using BOAPlugins.Models;
using WhiteStone.Services;

namespace BOAPlugins.SearchProcedure
{
    /// <summary>
    ///     The database connection strings
    /// </summary>
    public static class DatabaseConnectionStrings
    {
        #region Static Fields
        /// <summary>
        ///     The connections
        /// </summary>
        static List<DatabaseConnectionInfo> _connections;
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets the connections.
        /// </summary>
        public static IReadOnlyList<DatabaseConnectionInfo> Connections
        {
            get
            {
                if (_connections == null)
                {
                    var path = DirectoryHelper.PluginDirectory + "SearchProcedure\\DatabaseConnectionStrings.json";
                    _connections = new JsonSerializer().Deserialize<List<DatabaseConnectionInfo>>(File.ReadAllText(path));

                    if (Local.IsBOAGermany)
                    {
                        BringForwardGermanyConnection();
                    }
                }

                return _connections;
            }
        }
        #endregion

        #region Methods
        /// <summary>
        ///     Brings the forward germany connection.
        /// </summary>
        static void BringForwardGermanyConnection()
        {
            var germany = _connections.First(x => x.ConnectionStringDev.Contains(ServerNames.DevAtlasDE));
            _connections.Remove(germany);
            _connections.Insert(0, germany);
        }
        #endregion
    }
}