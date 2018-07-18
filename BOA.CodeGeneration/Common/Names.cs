namespace BOA.CodeGeneration.Common
{
    public static class Names
    {
        #region Static Fields
        internal static readonly string[] GenericUpdateInformationColumns = {UpdateUserName, UpdateHostName, UpdateHostIP, UpdateSystemDate};

        internal static readonly string[] GenericColumns =
        {
            UserName, HostName, HostIP, SystemDate,
            UpdateUserName, UpdateHostName, UpdateHostIP, UpdateSystemDate,
            ChannelId
        };
        #endregion

        #region Public Properties
        public static string BiggerThan => "BiggerThan";

        public static string BiggerThanOrEquals => "BiggerThanOrEqual";
        public static string ChannelId          => "ChannelId";
        public static string ChannelIdValue     => "Context.ApplicationContext.Authentication.Channel";

        public static string Contains => "Contains";

        public static string DotNetBool => "bool";

        public static string DotNetByte => "byte";

        public static string DotNetByteArray => "byte[]";

        public static string DotNetDateTime => "DateTime";

        public static string DotNetDecimal => "decimal";

        public static string DotNetDouble => "double";

        public static string DotNetGuid => "Guid";

        public static string DotNetInt16 => "short";

        public static string DotNetInt32 => "int";

        public static string DotNetInt64 => "long";

        public static string DotNetNullableDecimal => "decimal?";

        public static string DotNetObject => "object";

        public static string DotNetStringName => "string";

        public static string DotNetTimeSpan => "TimeSpan";

        public static string EndsWith      => "EndsWith";
        public static string HostIP        => "HostIP";
        public static string HostIPValue   => "Context.ApplicationContext.Authentication.IPAddress";
        public static string HostName      => "HostName";
        public static string HostNameValue => "Context.ApplicationContext.Authentication.MachineName";

        public static string IsNull => "IsNull";

        public static string LessThan => "LessThan";

        public static string LessThanOrEquals => "LessThanOrEqual";

        public static string NotEqual => "NotEqual";

        public static string StartsWith            => "StartsWith";
        public static string SystemDate            => "SystemDate";
        public static string SystemDateValue       => "DateTime.Now";
        public static string UpdateHostIP          => "UpdateHostIP";
        public static string UpdateHostIPValue     => "Context.ApplicationContext.Authentication.IPAddress";
        public static string UpdateHostName        => "UpdateHostName";
        public static string UpdateHostNameValue   => "Context.ApplicationContext.Authentication.MachineName";
        public static string UpdateSystemDate      => "UpdateSystemDate";
        public static string UpdateSystemDateValue => "DateTime.Now";

        public static string UpdateUserName => "UpdateUserName";

        public static string UpdateUserNameValue => "Context.ApplicationContext.Authentication.UserName";
        public static string UserName            => "UserName";

        public static string UserNameValue => "Context.ApplicationContext.Authentication.UserName";
        #endregion

        #region Methods
        internal static string NormalizeColumnNameForReversedKeywords(this string columnName)
        {
            if (columnName == "Key")
            {
                return "[" + columnName + "]";
            }

            return columnName;
        }
        #endregion
    }
}