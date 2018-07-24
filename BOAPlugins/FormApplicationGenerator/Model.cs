using System;
using System.Collections.Generic;
using System.IO;
using BOA.Common.Helpers;

namespace BOAPlugins.FormApplicationGenerator
{
    [Serializable]
    public class Model
    {
        #region Fields
        protected readonly DotNetType Boolean  = DotNetType.Boolean;
        protected readonly DotNetType DateTime = DotNetType.DateTime;
        protected readonly DotNetType Decimal  = DotNetType.Decimal;
        protected readonly DotNetType Int32    = DotNetType.Int32;
        protected readonly DotNetType String   = DotNetType.String;
        #endregion

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
        public IReadOnlyCollection<BCard> Cards                       { get; set; } = new List<BCard>();
        public IReadOnlyCollection<BTab> Tabs { get; set; } = new List<BTab>();
        public string                     DefinitionFormDataClassName { get; }

        public IReadOnlyCollection<BField> FormDataClassFields => Cards.GetAllFields();

        public string                      FormName                 { get; }
        public bool                        IsTabForm                { get; set; }
        public IReadOnlyCollection<BField> ListFormSearchFields     { get; set; } = new List<BField>();
        public string                      NamespaceName            { get; }
        public string                      NamespaceNameForOrch     { get; }
        public string                      NamespaceNameForType     { get; }
        public string                      RequestNameForDefinition { get; }
        public string                      RequestNameForList       { get; }
        public string                      SolutionFilePath         { get; }
        public string                      TypesProjectFolder       { get; }
        #endregion
    }

    [Serializable]
    public class BCard
    {
        #region Constructors
        public BCard(string title, IReadOnlyCollection<BField> fields)
        {
            Fields = fields;
            Title  = title;
        }
        #endregion

        #region Public Properties
        public IReadOnlyCollection<BField> Fields { get; }
        public string                      Title  { get; }
        #endregion
    }

    [Serializable]
    public class BTab
    {
        #region Constructors
        public BTab(string title, IReadOnlyCollection<BCard> cards)
        {
            Cards = cards;
            Title  = title;
        }

        public BTab(string title, IReadOnlyCollection<BField> fields)
        {
            Cards = new[] {new BCard(null, fields)};
            Title = title;
        }
        #endregion

        #region Public Properties
        public IReadOnlyCollection<BCard> Cards { get; }
        public string                      Title  { get; }
        #endregion
    }

    [Serializable]
    public class BField
    {
        #region Constructors
        public BField(DotNetType dotNetType, Enum name)
        {
            Name       = name.ToString();
            DotNetType = dotNetType;
        }
        #endregion

        #region Public Properties
        public ComponentType? ComponentType { get; set; } = FormApplicationGenerator.ComponentType.BInput;
        public DotNetType     DotNetType    { get; }
        public string         Name          { get; }
        public string         ParamType     { get; set; }
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