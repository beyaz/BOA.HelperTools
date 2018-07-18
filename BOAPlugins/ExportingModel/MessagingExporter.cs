using System;
using System.IO;
using System.Linq;
using BOA.Common.Helpers;

namespace BOAPlugins.ExportingModel
{
    public class MessagingExporter
    {
        #region Public Methods
        public static MessagingExporterResult ExportAsCSharpCode(string solutionFilePath)
        {
            var directory = Path.GetDirectoryName(solutionFilePath);

            if (directory == null)
            {
                return new MessagingExporterResult
                {
                    ErrorMessage = "directory is null.@solutionFilePath:" + solutionFilePath
                };
            }

            var messageFileName = "Message.cs";
            var messageFilePath = Directory.GetFiles(directory, "*.cs", SearchOption.AllDirectories).LastOrDefault(f => f.EndsWith(messageFileName));

            if (messageFilePath == null)
            {
                return new MessagingExporterResult
                {
                    ErrorMessage = $"messageFilePath not found.You must specify named file like:{messageFileName}"
                };
            }

            var firstLine = File.ReadAllLines(messageFilePath).FirstOrDefault(line => string.IsNullOrWhiteSpace(line) == false);

            var config = MessagingExporterInputLineParser.Parse(firstLine);

            return new MessagingExporterResult
            {
                GeneratedCode  = ExportAsCSharpCode(config.GroupName, config.NamespaceName),
                TargetFilePath = messageFilePath
            };
        }

        public static MessagingExporterResult ExportAsTypeScriptCode(string solutionFilePath)
        {
            var directory = Path.GetDirectoryName(solutionFilePath);

            if (directory == null)
            {
                return new MessagingExporterResult
                {
                    ErrorMessage = "directory is null.@solutionFilePath:" + solutionFilePath
                };
            }

            var messageFileName = "Message.tsx";
            var messageFilePath = Directory.GetFiles(directory, "*.tsx", SearchOption.AllDirectories).LastOrDefault(f => f.EndsWith(messageFileName));

            if (messageFilePath == null)
            {
                return new MessagingExporterResult
                {
                    ErrorMessage = $"messageFilePath not found.You must specify named file like:{messageFileName}"
                };
            }

            var firstLine = File.ReadAllLines(messageFilePath).FirstOrDefault(line => string.IsNullOrWhiteSpace(line) == false);

            var config = MessagingExporterInputLineParser.Parse(firstLine);

            return new MessagingExporterResult
            {
                GeneratedCode  = ExportGroupAsTypeScriptCode(config.GroupName),
                TargetFilePath = messageFilePath
            };
        }

        #endregion


        public static string ExportGroupAsTypeScriptCode(string groupName)
        {
            var builder = new PaddedStringBuilder();

            var propertyNames = DataSource.GetPropertyNames(groupName);

            builder.AppendLine($"// GroupName: {groupName}");

            builder.AppendLine("import { getMessage } from \"b-framework\"");
            builder.AppendLine("");
            builder.AppendLine($"export class Message");
            builder.AppendLine("{");
            builder.PaddingCount++;

            foreach (var item in propertyNames)
            {

                if (item.TR_Description.HasValue() || item.EN_Description.HasValue())
                {
                    builder.AppendLine("/**");

                    if (item.TR_Description.HasValue())
                    {
                        builder.AppendLine("*     TR: " + item.TR_Description.Replace(Environment.NewLine, ""));
                    }

                    if (item.EN_Description.HasValue())
                    {
                        builder.AppendLine("*     EN: " + item.EN_Description.Replace(Environment.NewLine, ""));
                    }

                    builder.AppendLine("*/");
                }


                var propertyName = item.PropertyName;

                builder.AppendLine($"static get {propertyName}() : string");
                builder.AppendLine("{");
                builder.PaddingCount++;

                builder.AppendLine($"return getMessage(\"{groupName}\", \"{propertyName}\");");

                builder.PaddingCount--;
                builder.AppendLine("}");
            }

            builder.PaddingCount--;
            builder.AppendLine("}");

            return builder.ToString();
        }
        #region Methods
        static string ExportAsCSharpCode(string groupName, string namespaceFullName)
        {
            if (string.IsNullOrWhiteSpace(namespaceFullName))
            {
                throw new ArgumentException(nameof(namespaceFullName));
            }

            var builder = new PaddedStringBuilder();

            var propertyNames = DataSource.GetPropertyNames(groupName);

            builder.AppendLine($"// GroupName: {groupName} , NamespaceName: {namespaceFullName}");
            builder.AppendLine("");
            builder.AppendLine("using BOA.Messaging;");
            builder.AppendLine("using MH = BOA.Messaging.MessagingHelper;");
            builder.AppendLine("");
            builder.AppendLine($"namespace {namespaceFullName}");
            builder.AppendLine("{");
            builder.PaddingCount++;

            builder.AppendLine($"public static class Message");
            builder.AppendLine("{");
            builder.PaddingCount++;

            foreach (var item in propertyNames)
            {
                var propertyName = item.PropertyName;

                if (item.TR_Description.HasValue() || item.EN_Description.HasValue())
                {
                    builder.AppendLine("/// <summary>");

                    
                    if (item.TR_Description.HasValue())
                    {
                        var suffix = "";
                        if (item.EN_Description.HasValue())
                        {
                            suffix = "<para></para>";
                        }
                        builder.AppendLine("///     TR: " + item.TR_Description.Replace(Environment.NewLine, "")+suffix) ;
                    }

                    if (item.EN_Description.HasValue())
                    {
                        builder.AppendLine("///     EN: " + item.EN_Description.Replace(Environment.NewLine, ""));
                    }

                    builder.AppendLine("/// </summary>");
                }

                builder.AppendLine($"public static string {propertyName} " + "{ get { return MH.GetMessage(\"" + groupName + "\", \"" + propertyName + "\"); " + "} }");
            }

            builder.PaddingCount--;
            builder.AppendLine("}");

            builder.PaddingCount--;
            builder.AppendLine("}");

            return builder.ToString();
        }
        #endregion
    }
}