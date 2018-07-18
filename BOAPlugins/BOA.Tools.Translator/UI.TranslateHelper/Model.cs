using System;
using BOA.Data.MvcWpf;

namespace BOA.Tools.Translator.UI.TranslateHelper
{
    [Serializable]
    public class Model : ModelBase
    {
        #region Public Properties
        public string JsonUI { get; set; }
        #endregion

        #region string Input
        string _input;

        public string Input
        {
            get { return _input; }
            set
            {
                if (_input != value)
                {
                    _input = value;
                    OnPropertyChanged("Input");
                }
            }
        }
        #endregion

        #region string PropertyDefinition
        string _propertyDefinition;
        public string PropertyDefinition
        {
            get { return _propertyDefinition; }
            set
            {
                if (_propertyDefinition != value)
                {
                    _propertyDefinition = value;
                    OnPropertyChanged("PropertyDefinition");
                }
            }
        }
        #endregion

        #region string PropertyAssignmentForEN
        string _propertyAssignmentForEN;
        public string PropertyAssignmentForEN
        {
            get { return _propertyAssignmentForEN; }
            set
            {
                if (_propertyAssignmentForEN != value)
                {
                    _propertyAssignmentForEN = value;
                    OnPropertyChanged("PropertyAssignmentForEN");
                }
            }
        }
        #endregion


    }
}