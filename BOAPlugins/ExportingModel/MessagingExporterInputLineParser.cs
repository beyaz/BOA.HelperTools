using System;
using BOA.Common.Helpers;

namespace BOAPlugins.ExportingModel
{
    class MessagingExporterInputLineParser
    {
        #region Public Methods
        public static MessagingExporterInput Parse(string line)
        {
            var config = new MessagingExporterInput();

            if (line == null)
            {
                throw new ArgumentNullException(nameof(line));
            }

            var arr = line.Trim().RemoveFromStart("//").Trim().Split(',');

            foreach (var param in arr)
            {
                if (param.Trim().StartsWith(nameof(MessagingExporterInput.GroupName)))
                {
                    config.GroupName = param.Trim().RemoveFromStart(nameof(MessagingExporterInput.GroupName)).Trim().RemoveFromStart(":").Trim();
                }

                if (param.Trim().StartsWith(nameof(MessagingExporterInput.NamespaceName)))
                {
                    config.NamespaceName = param.Trim().RemoveFromStart(nameof(MessagingExporterInput.NamespaceName)).Trim().RemoveFromStart(":").Trim();
                }
            }

            if (config.GroupName == null)
            {
                throw new ArgumentException(nameof(MessagingExporterInput.GroupName));
            }

            return config;
        }
        #endregion
    }
}