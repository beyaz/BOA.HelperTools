using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Model;
using BOA.Common.Helpers;

namespace BOA.CodeGeneration.Generators
{
    public class ContractBodyGenerator : WriterBaseBase
    {
        #region Public Properties
        public IReadOnlyList<ColumnInfo> Columns                         { get; set; }
        public string                    PrefixOfFieldOfContractProperty { get; set; }
        public string                    RegionText                      { get; set; }
        #endregion

        #region Properties
        static string RegionTextDefaultValue => "Database Columns";
        #endregion

        #region Public Methods
        public void GenerateDatabaseColumns()
        {
            WriteLine();
            WriteLine("#region " + (RegionText ?? RegionTextDefaultValue));
            WriteLine();
            foreach (var c in Columns)
            {
                WriteProperty(c.DotNetType, c.ColumnName, c.Comment);

                WriteLine();
            }

            WriteLine();
            WriteLine("#endregion");
            WriteLine();
        }

        public void WriteProperty(string typeName, string propertyName, string comment)
        {
            WriteProperty(GetPropertyFieldName(propertyName), "\"" + propertyName + "\"", typeName, propertyName, comment);
        }
        #endregion

        #region Methods
        string GetPropertyFieldName(string propertyName)
        {
            return GetPropertyFieldName(PrefixOfFieldOfContractProperty ?? "", propertyName);
        }

        public static string GetPropertyFieldName(string PrefixOfFieldOfContractProperty, string propertyName)
        {
            var prefix = PrefixOfFieldOfContractProperty ?? "";

            var firstChar = propertyName[0];

            if (firstChar == 'I')
            {
                firstChar = 'i';
            }
            else
            {
                firstChar = firstChar.ToString().ToLowerTR().First();
            }

            return prefix + firstChar + propertyName.Substring(1);
        }
        #endregion
    }

    class ContractGenerator : WriterBase
    {
        #region Constructors
        #region Constructor
        public ContractGenerator(WriterContext context)
            : base(context)
        {
        }
        #endregion
        #endregion

        #region Methods
        string Generate()
        {
            Padding = 0;

            WriteLine("using System;");
            WriteLine();

            WriteLine("namespace {0}", Context.NamespaceNameForTypeClass);
            WriteLine("{");

            Padding++;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Entity contract for database table {0}", Context.Naming.DatabaseTableFullPath);
            WriteLine("/// </summary>");

            var sealedPropertyText = "sealed ";
            if (!Context.Config.ContractIsSealed)
            {
                sealedPropertyText = "";
            }

            WriteLine("[Serializable]");
            WriteLine("{1}partial class {0}", Context.Naming.ContractName, sealedPropertyText);
            WriteLine("{");
            Padding++;

            var bodyGenerator = new ContractBodyGenerator
            {
                Columns = Context.Table.Columns,
                Padding = Padding,
                PrefixOfFieldOfContractProperty = Context.Config.PrefixOfFieldOfContractProperty
            };
            bodyGenerator.GenerateDatabaseColumns();
            Write(bodyGenerator.GeneratedString);

            Padding--;
            WriteLine("}"); // end of class
            Padding--;
            WriteLine("}");

            return GeneratedString;
        }

        string GenerateUserFile()
        {
            Padding = 0;
            if (Context.NamespaceNameForTypeClass != "BOA.Common.Types")
            {
                WriteLine("using BOA.Common.Types;");
            }

            WriteLine();

            WriteLine("namespace {0}", Context.NamespaceNameForTypeClass);
            WriteLine("{");
            Padding++;

            WriteLine("/// <summary>");
            WriteLine("///" + PaddingForComment + "Entity contract for database table {0}", Context.Naming.DatabaseTableFullPath);
            WriteLine("/// </summary>");
            WriteLine("public partial class {0} : ContractBase", Context.Naming.ContractName);
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