using System.Collections.Generic;
using BOAPlugins.Models;

namespace BOAPlugins.SearchProcedure
{
    /// <summary>
    ///     .
    /// </summary>
    public class Result : ResultBase
    {
        #region Public Methods
        /// <summary>
        ///     Adds the SQL file.
        /// </summary>
        public void AddSqlFile(SqlFileInfo info)
        {
            _sqlFileInfos.Add(info);
        }
        #endregion

        #region public IReadOnlyCollection<SqlFileInfo> SqlFileList
        readonly List<SqlFileInfo> _sqlFileInfos = new List<SqlFileInfo>();

        public DatabaseConnectionInfo ProcedureContainerDatabaseConnectionInfo { get; set; }

        public IReadOnlyCollection<SqlFileInfo> SqlFileList => _sqlFileInfos;
        #endregion
    }
}