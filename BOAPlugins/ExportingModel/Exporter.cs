using System.Collections.Generic;
using System.IO;
using System.Linq;
using BOA.Common.Helpers;
using Mono.Cecil;
using Newtonsoft.Json.Serialization;
using WhiteStone.Services;

namespace BOAPlugins.ExportingModel
{
    class Exporter
    {
        #region Static Fields
        static readonly CamelCasePropertyNamesContractResolver CamelCasePropertyNamesContractResolver = new CamelCasePropertyNamesContractResolver();
        #endregion

        #region Public Methods
        public static ExportContract Export(string configFilePath)
        {
            var jsonSerializer = new JsonSerializer();
            var config         = jsonSerializer.Deserialize<ExportContract>(File.ReadAllText(configFilePath));
            return Export(config);
        }

        public static string GetResolvedPropertyName(string propertyName)
        {
            return CamelCasePropertyNamesContractResolver.GetResolvedPropertyName(propertyName);
        }
        #endregion

        #region Methods
        internal static string GetTypeNameInContainerNamespace(string typefullName, string containerNamespace)
        {
            while (true)
            {
                var prefix = containerNamespace + ".";
                if (typefullName.StartsWith(prefix))
                {
                    return typefullName.RemoveFromStart(prefix);
                }

                var packages = containerNamespace.Split('.');
                if (packages.Length <= 1)
                {
                    return typefullName;
                }

                var items = new List<string>();
                for (var i = 0; i < packages.Length - 1; i++)
                {
                    items.Add(packages[i]);
                }

                containerNamespace = string.Join(".", items);
            }
        }

        static ExportContract Export(ExportContract data)
        {
            var sb = new PaddedStringBuilder();

            foreach (var info in data.ExportInfoList)
            {
                var assemblyDefinition = AssemblyDefinition.ReadAssembly(@"d:\boa\server\bin\" + info.Assembly);
                if (assemblyDefinition == null)
                {
                    data.ErrorMessage = "AssemblyNotFound:" + info.Assembly;
                    return data;
                }

                var typeDefinitions = new List<TypeDefinition>();
                foreach (var className in info.ExportClassNames)
                {
                    var typeDefinition = FindType(assemblyDefinition, className);

                    if (typeDefinition == null)
                    {
                        data.ErrorMessage = "typeDefinitionNotFound:" + className;
                        return data;
                    }

                    typeDefinitions.Add(typeDefinition);
                }

                foreach (var typeDefinition in typeDefinitions)
                {
                    GenerateType(typeDefinition, sb);
                }
            }

            data.GeneratedTSCode = sb.ToString();

            return data;
        }

        static TypeDefinition FindType(AssemblyDefinition definition, string classFullName)
        {
            foreach (var module in definition.Modules)
            {
                var typeDefinition = module.GetType(classFullName);

                if (typeDefinition != null)
                {
                    return typeDefinition;
                }
            }

            return null;
        }

        static void GenerateType(TypeDefinition typeDefinition, PaddedStringBuilder sb)
        {
            var containerNamespace = typeDefinition.Namespace;
            sb.AppendLine($"namespace {containerNamespace}");
            sb.AppendLine("{");
            sb.PaddingCount++;

            if (typeDefinition.IsEnum)
            {
                sb.AppendLine($"export enum {typeDefinition.Name}");
            }
            else
            {
                var extends = " extends ";
                if (typeDefinition.BaseType.FullName == typeof(object).FullName)
                {
                    extends = "";
                }
                else
                {
                    extends += GetTypeNameInContainerNamespace(typeDefinition.BaseType.FullName, containerNamespace);
                }

                sb.AppendLine($"export interface {typeDefinition.Name}" + extends);
            }

            sb.AppendLine("{");
            sb.PaddingCount++;

            if (typeDefinition.IsEnum)
            {
                var fieldDeclerations = new List<string>();
                foreach (var field in typeDefinition.Fields.Where(f => f.Name != "value__"))
                {
                    fieldDeclerations.Add($"{field.Name} = {field.Constant}");
                }

                for (var i = 0; i < fieldDeclerations.Count; i++)
                {
                    var decleration = fieldDeclerations[i];

                    if (i < fieldDeclerations.Count - 1)
                    {
                        sb.AppendLine(decleration + ",");
                    }
                    else
                    {
                        sb.AppendLine(decleration);
                    }
                }
            }
            else
            {
                foreach (var propertyDefinition in typeDefinition.Properties)
                {
                    var typeName = GetTSTypeName(propertyDefinition.PropertyType, containerNamespace);

                    var name = GetResolvedPropertyName(propertyDefinition.Name);
                    // if (IsNullableType(propertyDefinition.DeclaringType))
                    {
                        name += "?";
                    }

                    sb.AppendLine($"{name} : {typeName};");
                }
            }

            sb.PaddingCount--;
            sb.AppendLine("}");

            sb.PaddingCount--;
            sb.AppendLine("}");
        }

        static string GetTSTypeName(TypeReference typeReference, string containerNamespace)
        {
            if (IsNullableType(typeReference))
            {
                return GetTSTypeName(((GenericInstanceType) typeReference).GenericArguments[0], containerNamespace);
            }

            if (typeReference.FullName == "System.String")
            {
                return "string";
            }

            if (typeReference.FullName == typeof(short).FullName ||
                typeReference.FullName == typeof(int).FullName ||
                typeReference.FullName == typeof(byte).FullName ||
                typeReference.FullName == typeof(sbyte).FullName ||
                typeReference.FullName == typeof(short).FullName ||
                typeReference.FullName == typeof(ushort).FullName ||
                typeReference.FullName == typeof(double).FullName ||
                typeReference.FullName == typeof(float).FullName ||
                typeReference.FullName == typeof(decimal).FullName ||
                typeReference.FullName == typeof(long).FullName)

            {
                return "number";
            }

            if (typeReference.FullName == "System.DateTime")
            {
                return "Date";
            }

            if (typeReference.FullName == "System.Boolean")
            {
                return "boolean";
            }

            if (typeReference.FullName == "System.Object")
            {
                return "any";
            }

            if (typeReference.IsGenericInstance)
            {
                var genericInstanceType = (GenericInstanceType) typeReference;

                var isArrayType =
                    genericInstanceType.GenericArguments.Count == 1 &&
                    (
                        typeReference.Name == "Collection`1" ||
                        typeReference.Name == "List`1" ||
                        typeReference.Name == "IReadOnlyCollection`1" ||
                        typeReference.Name == "IReadOnlyList`1"
                    );

                if (isArrayType)
                {
                    var arrayType = genericInstanceType.GenericArguments[0];
                    return GetTSTypeName(arrayType, containerNamespace) + "[]";
                }
            }

            return GetTypeNameInContainerNamespace(typeReference.FullName, containerNamespace);
        }

        static bool IsNullableType(TypeReference typeReference)
        {
            return typeReference.Name == "Nullable`1" && typeReference.IsGenericInstance;
        }
        #endregion
    }
}