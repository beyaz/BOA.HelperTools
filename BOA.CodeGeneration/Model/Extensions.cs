using System;
using BOA.CodeGeneration.Util;
using BOA.DatabaseAccess;

namespace BOA.CodeGeneration.Model
{
    public static class Extensions
    {
        #region Public Methods
        public static IDatabase GetDatabase(this TableConfig config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (config.ConnectionStringForTakeTableInformation == null)
            {
                if (config.ServerNameForTakeTableInformation == null)
                {
                    throw new ArgumentException("DatabaseForTakeTableInformation must have value.");
                }

                config.ConnectionStringForTakeTableInformation = "Server={0};Database={1};Trusted_Connection=True;".FormatCode(config.ServerNameForTakeTableInformation, config.DatabaseName);
            }

            return new SqlDatabase(config.ConnectionStringForTakeTableInformation);
        }
        #endregion
    }
}