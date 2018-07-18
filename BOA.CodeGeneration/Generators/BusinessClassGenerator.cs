using System;
using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Generators
{
    class BusinessClassGenerator : WriterBase
    {
        #region Constructors
        #region Constructor
        public BusinessClassGenerator(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Properties
        TableConfig Config => Context.Config;

        NamingModel Naming => Context.Naming;

        bool ReadContractMustBeWrite
        {
            get
            {
                if (Config.NoNeedToGenerateMethodReadContract)
                {
                    return false;
                }

                return Config.CanGenerateSelectByKey ||
                       Config.CanGenerateSelectByKeyList ||
                       Config.CustomSelects.Any();
            }
        }

        TableInfo Table => Context.Table;
        #endregion

        #region Methods
        string Generate()
        {
            Padding = 0;

            var useGenericCollections = Config.CustomSelects.Any() || Config.CanGenerateSelectByKeyList || Config.CanGenerateInsertStructured;

            var useLinq = Config.CustomSelects.Any(cs => cs.MustBeReturnFirstContract) || Config.CanGenerateSelectByKeyList||
                          Config.CustomSelects.Any(cs => cs.Parameters?.Any(p=>p.IN != null) == true)
                ;

            var usingList = new List<string>
            {
                "BOA.Base",
                "BOA.Base.Data",
                "BOA.Common.Extensions",
                "BOA.Common.Types"
            };

            if (Config.DoCompressionForVarBinaryColumns)
            {
                usingList.Add("BOA.Common.Helpers");
            }

            if (Context.NamespaceNameForTypeClass != "BOA.Common.Types")
            {
                usingList.Add(Context.NamespaceNameForTypeClass);
            }

            usingList.Add("System");

            if (useGenericCollections)
            {
                usingList.Add("System.Collections.Generic");
            }

            if (useLinq)
            {
                usingList.Add("System.Linq");
            }

            usingList.Add("System.Data");
            usingList.Add("System.Data.SqlClient");

            Config.OnUsingNamespacesWillbeGenerateInBusinessClassDesignerFile?.Invoke(usingList);

            foreach (var namespaceFullName in usingList)
            {
                WriteLine($"using {namespaceFullName};");
            }

            WriteLine();
            WriteLine("namespace {0}", Naming.NamespaceNameOfBusinessClass);

            WriteLine("{");

            Padding++;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Data access layer for {0}", Naming.DatabaseTableFullPath);
            WriteLine("/// </summary>");

            WriteLine("partial class " + Naming.ClassNameOfBusiness + " : ObjectHelper");
            WriteLine("{");

            Padding++;

            WriteLine("#region Constructor");
            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Creates a new data access layer for {0}", Naming.DatabaseTableFullPath);
            WriteLine("/// </summary>");
            WriteLine("public " + Naming.ClassNameOfBusiness + "(ExecutionDataContext context) : base(context) { }");
            WriteLine("#endregion");
            WriteLine();

            WriteLine("#region auto-generated");

            var businessClassKeyPrefix = Naming.NamespaceNameOfBusinessClass + "." + Naming.ClassNameOfBusiness + ".";

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Creates a new GenericResponse instance");
            WriteLine("/// </summary>");
            WriteLine("GenericResponse<T> CreateResponse<T>([System.Runtime.CompilerServices.CallerMemberName] string memberName = null)");
            WriteLine("{");
            Padding++;
            WriteLine("return InitializeGenericResponse<T>(\"{0}\" + memberName);", businessClassKeyPrefix);
            Padding--;
            WriteLine("}");

            if (Table.PrimaryKeyColumns.Any())
            {
                if (Config.CanGenerateSelectByKey)
                {
                    WriteLine();
                    Write(new SelectByKeyCs(Context).Generate());
                }

                if (Config.CanGenerateSelectByKeyList)
                {
                    WriteLine();
                    Write(new SelectByKeyListCs(Context).Generate());
                }

                if (Config.CanGenerateUpdate)
                {
                    WriteLine();
                    Write(new UpdateCs(Context).Generate());
                }

                if (Config.CanGenerateDelete)
                {
                    WriteLine();
                    Write(new DeleteCs(Context).Generate());
                }
            }

            if (Config.CanGenerateInsert)
            {
                WriteLine();
                Write(new InsertCs(Context).Generate());
            }

            if (Config.CanGenerateInsertStructured)
            {
                WriteLine();
                Write(new InsertStructuredCs(Context).Generate());
            }

            foreach (var c in Config.CustomSelects)
            {
                WriteLine();

                if (c.IsSelectByValueList)
                {
                    Write(new SelectByValueListCs(Context, c.SelectByValueListColumnName, c.DotNetMethodName).Generate());

                    continue;
                }

                Write(new SelectByColumnsCs(Context, c).Generate());
            }

            foreach (var c in Config.CustomExists)
            {
                WriteLine();
                Write(new CustomExistsCSharpWriter(Context, c).Generate());
            }

            foreach (var c in Config.CustomUpdates)
            {
                WriteLine();
                Write(new UpdateCsCustom(Context, c).Generate());
            }

            if (Config.CustomExecutions != null)
            {
                foreach (var c in Config.CustomExecutions)
                {
                    WriteLine();
                    var customExecuteScalar = c as CustomExecution;
                    if (customExecuteScalar != null)
                    {
                        Write(new CustomExecutionCs(Context, customExecuteScalar).Generate());
                    }
                    else
                    {
                        throw new NotImplementedException(c.GetType().FullName);
                    }
                }
            }

            if (ReadContractMustBeWrite)
            {
                WriteLine();
                Write(new ReadContractGenerator(Context).Generate());
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
            Padding = 0;

            WriteLine("namespace {0}", Naming.NamespaceNameOfBusinessClass);
            WriteLine("{");
            Padding++;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Data access layer for {0}", Naming.DatabaseTableFullPath);
            WriteLine("/// </summary>");
            WriteLine("public sealed partial class " + Naming.ClassNameOfBusiness);
            WriteLine("{");

            WriteLine();

            WriteLine("}");

            Padding--;

            WriteLine("}");

            return GeneratedString;
        }
        #endregion

        #region string DesignerFile
        string _designerFile;

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