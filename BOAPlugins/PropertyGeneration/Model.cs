using System;
using BOA.Data.MvcWpf;

namespace BOAPlugins.PropertyGeneration
{
    /// <summary>
    ///     The model
    /// </summary>
    [Serializable]
    public class Model : ModelBase
    {
        #region Public Properties
        /// <summary>
        ///     Gets or sets the json UI.
        /// </summary>
        public string JsonUI { get; set; }
        #endregion

        #region string Input
        /// <summary>
        ///     The input
        /// </summary>
        string _input;

        /// <summary>
        ///     Gets or sets the input.
        /// </summary>
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

        #region string Output
        /// <summary>
        ///     The output
        /// </summary>
        string _output;

        /// <summary>
        ///     Gets or sets the output.
        /// </summary>
        public string Output
        {
            get { return _output; }
            set
            {
                if (_output != value)
                {
                    _output = value;
                    OnPropertyChanged("Output");
                }
            }
        }
        #endregion
    }
}