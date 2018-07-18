using System;
using System.Linq;
using System.Text;

namespace BOAPlugins.PropertyGeneration
{
    /// <summary>
    ///     The multiple property generator
    /// </summary>
    public class MultiplePropertyGenerator
    {
        #region Fields
        /// <summary>
        ///     The value
        /// </summary>
        readonly string _value;
        #endregion

        #region Constructors
        #region Constructor
        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiplePropertyGenerator" /> class.
        /// </summary>
        public MultiplePropertyGenerator(string value)
        {
            _value = value;
        }
        #endregion
        #endregion

        #region Public Methods
        /// <summary>
        ///     Generates this instance.
        /// </summary>
        public string Generate()
        {
            var arr = SplitAndRemoveNullOrEmptyAndTrimValues(_value, Environment.NewLine.ToCharArray());

            var sb = new StringBuilder();
            foreach (var line in arr)
            {
                var dataArr = SplitAndRemoveNullOrEmptyAndTrimValues(line);
                if (dataArr.Length != 2)
                {
                    continue;
                }

                var typeName = dataArr[0];
                var memberName = dataArr[1];

                sb.AppendLine(new PropertyGenerator().Generate(typeName, memberName));
            }

            return sb.ToString();
        }
        #endregion

        #region Methods
        #region Utility
        /// <summary>
        ///     Splits the and remove null or empty and trim values.
        /// </summary>
        static string[] SplitAndRemoveNullOrEmptyAndTrimValues(string s, params char[] separator)
        {
            return (from value in s.Split(separator)
                where !string.IsNullOrWhiteSpace(value)
                select value.Trim()).ToArray();
        }
        #endregion
        #endregion
    }
}