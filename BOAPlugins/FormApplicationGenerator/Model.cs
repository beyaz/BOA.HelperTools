using System;
using System.Collections.Generic;
using System.IO;
using BOA.Common.Helpers;

namespace BOAPlugins.FormApplicationGenerator
{
    [Serializable]
    public class Model
    {
        #region Constructors
        public Model(string solutionFilePath, string formName)
        {
            SolutionFilePath = solutionFilePath ?? throw new ArgumentNullException(nameof(solutionFilePath));
            FormName         = formName ?? throw new ArgumentNullException(nameof(formName));

            var solutionFileName = Path.GetFileName(solutionFilePath);
            var namespaceName    = solutionFileName.RemoveFromStart("BOA.").RemoveFromEnd(".sln");

            NamespaceName               = namespaceName;
            RequestNameForDefinition    = formName + "FormRequest";
            RequestNameForList          = formName + "ListFormRequest";
            NamespaceNameForType        = "BOA.Types." + namespaceName;
            NamespaceNameForOrch        = "BOA.Orchestration." + namespaceName;
            DefinitionFormDataClassName = formName + "FormData";
            TypesProjectFolder          = Path.GetDirectoryName(solutionFilePath) + Path.DirectorySeparatorChar + NamespaceNameForType + Path.DirectorySeparatorChar;
        }
        #endregion

        #region Public Properties
        public string          DefinitionFormDataClassName { get; }
        public List<BField> FormDataClassFields         { get; set; } = new List<BField>();
        public string          FormName                    { get; }
        public List<BField> ListFormSearchFields        { get; set; } = new List<BField>();
        public string          NamespaceName               { get; }
        public string          NamespaceNameForOrch        { get; }
        public string          NamespaceNameForType        { get; }
        public string          RequestNameForDefinition    { get; }
        public string          RequestNameForList          { get; }
        public string          SolutionFilePath            { get; }
        public string          TypesProjectFolder          { get; }

        

        #endregion
    }

    static class NamingHelper
    {
        #region Public Methods
        public static void InitializeFieldComponentTypes(List<BField> FormDataClassFields)
        {
            foreach (var field in FormDataClassFields)
            {
                if (field.ComponentType != null)
                {
                    continue;
                }

                var fieldTypeName = field.DotNetType;
                if (fieldTypeName == DotNetType.String)
                {
                    field.ComponentType = ComponentType.BInput;
                    continue;
                }

                if (fieldTypeName == DotNetType.Int32 ||
                    fieldTypeName == DotNetType.Decimal)
                {
                    field.ComponentType = ComponentType.BInputNumeric;
                    continue;
                }

                if (fieldTypeName == DotNetType.DateTime)
                {
                    field.ComponentType = ComponentType.BDateTimePicker;
                }

                if (fieldTypeName == DotNetType.Boolean)
                {
                    field.ComponentType = ComponentType.BCheckBox;
                }
            }
        }
        #endregion
    }

    [Serializable]
    public class BField
    {
        public BField()
        {
            
        }

        public BField(Enum name, DotNetType dotNetType)
        {
            Name = name.ToString();
            DotNetType = dotNetType;
        }

        #region Public Properties
        public ComponentType? ComponentType { get; set; } = FormApplicationGenerator.ComponentType.BInput;
        public string         GroupBoxTitle { get; set; }
        public string         Name          { get; set; }
        public string         ParamType     { get; set; }
        public DotNetType DotNetType      { get; set; } = DotNetType.String;
        #endregion
    }

    [Serializable]
    public enum ComponentType
    {
        BAccountComponent,
        BDateTimePicker,
        BInput,
        BInputNumeric,
        BParameterComponent,
        BBranchComponent,
        BCheckBox
    }

    [Serializable]
    public enum DotNetType
    {
        Int32,
        String,
        Decimal,
        DateTime,
        Boolean
    }
}