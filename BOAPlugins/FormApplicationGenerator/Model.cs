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
        public List<FieldInfo> FormDataClassFields         { get; set; } = new List<FieldInfo>();
        public string          FormName                    { get; }
        public List<FieldInfo> ListFormSearchFields        { get; set; } = new List<FieldInfo>();
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
        public static void InitializeFieldComponentTypes(List<FieldInfo> FormDataClassFields)
        {
            foreach (var field in FormDataClassFields)
            {
                if (field.ComponentName != null)
                {
                    continue;
                }

                var fieldTypeName = field.TypeName;
                if (fieldTypeName == DotNetTypeName.String)
                {
                    field.ComponentName = ComponentName.BInput;
                    continue;
                }

                if (fieldTypeName == DotNetTypeName.Int32 ||
                    fieldTypeName == DotNetTypeName.Decimal)
                {
                    field.ComponentName = ComponentName.BInputNumeric;
                    continue;
                }

                if (fieldTypeName == DotNetTypeName.DateTime)
                {
                    field.ComponentName = ComponentName.BDateTimePicker;
                }

                if (fieldTypeName == DotNetTypeName.Boolean)
                {
                    field.ComponentName = ComponentName.BCheckBox;
                }
            }
        }
        #endregion
    }

    [Serializable]
    public class FieldInfo
    {
        #region Public Properties
        public ComponentName? ComponentName { get; set; } = FormApplicationGenerator.ComponentName.BInput;
        public string         GroupBoxTitle { get; set; }
        public string         Name          { get; set; }
        public string         ParamType     { get; set; }
        public DotNetTypeName TypeName      { get; set; } = DotNetTypeName.String;
        #endregion
    }

    [Serializable]
    public enum ComponentName
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
    public enum DotNetTypeName
    {
        Int32,
        String,
        Decimal,
        DateTime,
        Boolean
    }
}