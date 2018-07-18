using System;
using System.Linq;
using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    class OrchestrationFileGenerator : WriterBase
    {
        #region Fields
        string _designerFile;
        #endregion

        #region Constructors
        #region Constructor
        public OrchestrationFileGenerator(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Public Properties
        public string DesignerFile
        {
            get
            {
                if (_designerFile == null)
                {
                    ClearOutput();
                    _designerFile = Generate();
                }

                return _designerFile;
            }
        }
        #endregion

        #region Properties
        bool IsCollectionGenericNamespaceRequired
        {
            get
            {
                var config = Context.Config;

                var useGenericCollections = false;
                if (config.CustomSelects.Any())
                {
                    useGenericCollections = config.CustomSelects.Any(CanGenerateOrchestrationMethod);
                }

                return useGenericCollections;
            }
        }
        #endregion

        #region Methods
        bool CanGenerateOrchestrationMethod(CustomSelectMethod c)
        {
            return c.Parameters.All(p => p.Equal != null);
        }

        string Generate()
        {
            var config = Context.Config;
            var table  = Context.Table;
            var naming = Context.Naming;

            Padding = 0;

            WriteLine("using BOA.Base;");
            WriteLine("using BOA.Common.Extensions;");
            WriteLine("using BOA.Common.Types;");
            WriteLine("using {0};", Context.NamespaceNameForTypeClass);
            if (IsCollectionGenericNamespaceRequired)
            {
                WriteLine("using System.Collections.Generic;");
            }

            WriteLine();
            WriteLine("namespace {0}", GetNamespaceNameForOrchestrationClass());

            WriteLine("{");

            Padding++;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Orchestration layer for {0}", naming.DatabaseTableFullPath);
            WriteLine("/// </summary>");

            WriteLine("public partial class " + naming.ClassNameOfOrchestration);
            WriteLine("{");

            Padding++;

            WriteLine("#region auto-generated");

            var classKeyPrefix = "BOA.Orchestration." + naming.NamespaceName + "." + naming.ClassNameOfOrchestration + ".";

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Creates a new GenericResponse instance");
            WriteLine("/// </summary>");
            WriteLine("GenericResponse<T> CreateResponse<T>(ObjectHelper objectHelper, [System.Runtime.CompilerServices.CallerMemberName] string memberName = null)");
            WriteLine("{");
            Padding++;
            WriteLine("return objectHelper.InitializeGenericResponse<T>(\"{0}\" + memberName);", classKeyPrefix);
            Padding--;
            WriteLine("}");

            WriteLine();

            if (table.PrimaryKeyColumns.Any())
            {
                if (config.CanGenerateSelectByKey)
                {
                    WriteSelectByKey();
                    WriteLine();
                }

                if (config.CanGenerateUpdate)
                {
                    WriteUpdate();
                    WriteLine();
                }

                if (config.CanGenerateDelete)
                {
                    WriteDelete();
                    WriteLine();
                }
            }

            if (config.CanGenerateInsert)
            {
                WriteInsert();
                WriteLine();
            }

            foreach (var c in config.CustomSelects)
            {
                if (CanGenerateOrchestrationMethod(c))
                {
                    WriteCustomSelect(c);
                    WriteLine();
                }
            }

            foreach (var c in config.CustomExists)
            {
                if (CanGenerateOrchestrationMethod(c))
                {
                    WriteCustomSelect(c, true);
                    WriteLine();
                }
            }

            foreach (var c in config.CustomUpdates)
            {
                WriteCustomUpdate(c);
                WriteLine();
            }

            WriteLine("#endregion");

            Padding--;

            WriteLine("}");

            Padding--;
            WriteLine("}");

            return GeneratedString;
        }

        string GenerateUserFile()
        {
            var naming = Context.Naming;

            Padding = 0;

            WriteLine("namespace {0}", GetNamespaceNameForOrchestrationClass());
            WriteLine("{");
            Padding++;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Orchestration layer for {0}", naming.DatabaseTableFullPath);
            WriteLine("/// </summary>");
            WriteLine("public partial class " + naming.ClassNameOfOrchestration);
            WriteLine("{");

            WriteLine();

            WriteLine("}");

            Padding--;

            WriteLine("}");

            return GeneratedString;
        }

        string GetNamespaceNameForOrchestrationClass()
        {
            if (Context.Config.NamespaceNameForOrchestrationClass != null)
            {
                return Context.Config.NamespaceNameForOrchestrationClass;
            }

            return "BOA.Orchestration.{0}".FormatCode(Context.Naming.NamespaceName);
        }

        void WriteCustomSelect(CustomSelectMethod c, bool isCustomExist = false)
        {
            var sql = new SelectByColumnsCs(Context, c);
            if (isCustomExist)
            {
                sql = new CustomExistsCSharpWriter(Context, c);
            }

            var config = Context.Config;
            var naming = Context.Naming;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "<para> {0} </para>", sql.ForcedComment);
            if (c.Comment != null)
            {
                foreach (var commentLine in from line in c.Comment.Split(Environment.NewLine.ToCharArray())
                                            where !string.IsNullOrWhiteSpace(line)
                                            select line)
                {
                    WriteLine("///" + PaddingForComment + "<para> " + commentLine + " </para>");
                }
            }

            foreach (var p in c.Parameters)
            {
                WriteLine("///" + PaddingForComment + "<para> WHERE " + p.Equal + " = request.DataContract." + p.Equal + " </para>");
            }

            WriteLine("/// </summary>");
            WriteLine("public GenericResponse<{0}> {1}({2}Request request, ObjectHelper objectHelper)",
                      sql.GenericResponseMethodReturnType, c.DotNetMethodName, naming.ClassNameOfOrchestration);

            WriteLine("{");
            Padding++;
            WriteLine("var returnObject = CreateResponse<{0}>(objectHelper);", sql.GenericResponseMethodReturnType);
            WriteLine();

            WriteLine("if (request.DataContract == null)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.AddError({0});", config.MethodParameterCannotBeNullMessage);
            Padding--;
            WriteLine("}");

            WriteLine();

            WriteLine("var bo{0} = new BOA.Business.{1}.{0}(objectHelper.Context);", naming.ClassNameOfBusiness,
                      naming.NamespaceName);

            var parameterPart = string.Join(" , ", from p in c.Parameters
                                                   select "request.DataContract." + p.Equal);
            WriteLine();
            WriteLine("var response = bo{0}.{1}({2});", naming.ClassNameOfBusiness, c.DotNetMethodName, parameterPart);

            WriteLine("if (!response.Success)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.Add(response);");
            Padding--;
            WriteLine("}");
            WriteLine();
            WriteLine("returnObject.Value = response.Value;");
            WriteLine();
            WriteLine("return returnObject;");
            Padding--;
            WriteLine("}");
        }

        void WriteCustomUpdate(CustomUpdateMethod customUpdateMethod)
        {
            var naming = Context.Naming;

            if (customUpdateMethod.Comment != null)
            {
                WriteLine("/// <summary>");
                WriteLine("///" + PaddingForComment + "Updates only one record of {0}", naming.DatabaseTableFullPath);
                WriteLine("/// </summary>");
            }

            WriteLine("public GenericResponse<int> {1}({2}Request request, ObjectHelper objectHelper)",
                      naming.ContractName, customUpdateMethod.DotNetMethodName, naming.ClassNameOfOrchestration);

            WriteLine("{");
            Padding++;
            WriteLine("var returnObject = CreateResponse<int>(objectHelper);");
            WriteLine();
            WriteLine("var bo{0} = new BOA.Business.{1}.{0}(objectHelper.Context);", naming.ClassNameOfBusiness,
                      naming.NamespaceName);

            var parameterPart = "request.DataContract";
            WriteLine();
            WriteLine("var response = bo{0}.{1}({2});", naming.ClassNameOfBusiness, customUpdateMethod.DotNetMethodName,
                      parameterPart);

            WriteLine("if (!response.Success)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.Add(response);");
            Padding--;
            WriteLine("}");
            WriteLine();
            WriteLine("returnObject.Value = response.Value;");
            WriteLine();
            WriteLine("return returnObject;");
            Padding--;
            WriteLine("}");
        }

        void WriteDelete()
        {
            var config = Context.Config;
            var table  = Context.Table;
            var naming = Context.Naming;

            var primaryKeys = string.Join(" - ", from c in table.PrimaryKeyColumns
                                                 select c.ColumnName);

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Deletes only one record from '{0}' by using primary key '{1}'", naming.DatabaseTableFullPath,
                      primaryKeys);
            WriteLine("/// </summary>");
            WriteLine("public GenericResponse<{0}> {1}({2}Request request, ObjectHelper objectHelper)",
                      "int", naming.NameOfDotNetMethodDelete, naming.ClassNameOfOrchestration);

            WriteLine("{");
            Padding++;
            WriteLine("var returnObject = CreateResponse<{0}>(objectHelper);", "int");
            WriteLine();

            WriteLine("if (request.DataContract == null)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.AddError({0});", config.MethodParameterCannotBeNullMessage);
            Padding--;
            WriteLine("}");
            WriteLine();

            WriteLine("var bo{0} = new BOA.Business.{1}.{0}(objectHelper.Context);", naming.ClassNameOfBusiness,
                      naming.NamespaceName);

            var parameterPart = string.Join(" , ", from c in table.PrimaryKeyColumns
                                                   select "request.DataContract." + c.ColumnName);
            WriteLine();
            WriteLine("var response = bo{0}.{1}({2});", naming.ClassNameOfBusiness, naming.NameOfDotNetMethodDelete,
                      parameterPart);

            WriteLine("if (!response.Success)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.Add(response);");
            Padding--;
            WriteLine("}");
            WriteLine();
            WriteLine("returnObject.Value = response.Value;");
            WriteLine();
            WriteLine("return returnObject;");
            Padding--;
            WriteLine("}");
        }

        void WriteInsert()
        {
            var naming = Context.Naming;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Insert new record into '{0}'", naming.DatabaseTableFullPath);
            WriteLine("/// </summary>");
            WriteLine("public GenericResponse<int> {1}({2}Request request, ObjectHelper objectHelper)",
                      naming.ContractName, naming.NameOfDotNetMethodInsert, naming.ClassNameOfOrchestration);

            WriteLine("{");
            Padding++;
            WriteLine("var returnObject = CreateResponse<int>(objectHelper);");

            WriteLine();
            WriteLine("var bo{0} = new BOA.Business.{1}.{0}(objectHelper.Context);", naming.ClassNameOfBusiness,
                      naming.NamespaceName);

            var parameterPart = "request.DataContract";
            WriteLine();
            WriteLine("var response = bo{0}.{1}({2});", naming.ClassNameOfBusiness, naming.NameOfDotNetMethodInsert,
                      parameterPart);

            WriteLine("if (!response.Success)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.Add(response);");
            Padding--;
            WriteLine("}");
            WriteLine();
            WriteLine("returnObject.Value = response.Value;");
            WriteLine();
            WriteLine("return returnObject;");
            Padding--;
            WriteLine("}");
        }

        void WriteSelectByKey()
        {
            var config = Context.Config;
            var table  = Context.Table;
            var naming = Context.Naming;

            var primaryKeys = string.Join(" - ", from c in table.PrimaryKeyColumns
                                                 select c.ColumnName);

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Selects only one record from '{0}' by using primary key '{1}'", naming.DatabaseTableFullPath,
                      primaryKeys);
            WriteLine("/// </summary>");
            WriteLine("public GenericResponse<{0}> {1}({2}Request request, ObjectHelper objectHelper)",
                      naming.ContractName, naming.NameOfDotNetMethodSelectByKey, naming.ClassNameOfOrchestration);

            WriteLine("{");
            Padding++;
            WriteLine("var returnObject = CreateResponse<{0}>(objectHelper);", naming.ContractName);
            WriteLine();

            WriteLine("if (request.DataContract == null)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.AddError({0});", config.MethodParameterCannotBeNullMessage);
            Padding--;
            WriteLine("}");
            WriteLine();

            WriteLine("var bo{0} = new BOA.Business.{1}.{0}(objectHelper.Context);", naming.ClassNameOfBusiness,
                      naming.NamespaceName);

            var parameterPart = string.Join(" , ", from c in table.PrimaryKeyColumns
                                                   select "request.DataContract." + c.ColumnName);
            WriteLine();
            WriteLine("var response = bo{0}.{1}({2});", naming.ClassNameOfBusiness, naming.NameOfDotNetMethodSelectByKey,
                      parameterPart);

            WriteLine("if (!response.Success)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.Add(response);");
            Padding--;
            WriteLine("}");
            WriteLine();
            WriteLine("returnObject.Value = response.Value;");
            WriteLine();
            WriteLine("return returnObject;");
            Padding--;
            WriteLine("}");
        }

        void WriteUpdate()
        {
            var naming = Context.Naming;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Updates only one record of {0}", naming.DatabaseTableFullPath);
            WriteLine("/// </summary>");
            WriteLine("public GenericResponse<int> {1}({2}Request request, ObjectHelper objectHelper)",
                      naming.ContractName, naming.NameOfDotNetMethodUpdate, naming.ClassNameOfOrchestration);

            WriteLine("{");
            Padding++;
            WriteLine("var returnObject = CreateResponse<int>(objectHelper);");
            WriteLine();
            WriteLine("var bo{0} = new BOA.Business.{1}.{0}(objectHelper.Context);", naming.ClassNameOfBusiness,
                      naming.NamespaceName);

            var parameterPart = "request.DataContract";
            WriteLine();
            WriteLine("var response = bo{0}.{1}({2});", naming.ClassNameOfBusiness, naming.NameOfDotNetMethodUpdate,
                      parameterPart);

            WriteLine("if (!response.Success)");
            WriteLine("{");
            Padding++;
            WriteLine("return returnObject.Add(response);");
            Padding--;
            WriteLine("}");
            WriteLine();
            WriteLine("returnObject.Value = response.Value;");
            WriteLine();
            WriteLine("return returnObject;");
            Padding--;
            WriteLine("}");
        }
        #endregion

        #region string UserFile
        string _userFile;

        public string UserFile
        {
            get
            {
                if (_userFile == null)
                {
                    ClearOutput();
                    _userFile = GenerateUserFile();
                }

                return _userFile;
            }
        }
        #endregion
    }
}