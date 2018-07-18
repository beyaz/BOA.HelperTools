using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BOAPlugins.SearchProcedure
{
    public sealed class SpaceCaseInsensitiveComparator
    {
        #region Fields
        readonly CultureInfo _culture;

        Func<string, bool> _ignoreLines;
        #endregion

        #region Constructors
        public SpaceCaseInsensitiveComparator(CultureInfo cultureInfo)
        {
            _culture = cultureInfo;
        }

        public SpaceCaseInsensitiveComparator()
        {
            _culture = CultureInfo.CurrentCulture;
        }
        #endregion

        #region Public Methods
        public bool Compare(string left, string right)
        {
            if (left == null)
            {
                if (right == null)
                {
                    return true;
                }
                return false;
            }

            if (right == null)
            {
                return false;
            }

            if (_ignoreLines != null)
            {
                right = string.Join(string.Empty, right.Split(Environment.NewLine.ToCharArray()).Where(line => !_ignoreLines(line)));
                left = string.Join(string.Empty, left.Split(Environment.NewLine.ToCharArray()).Where(line => !_ignoreLines(line)));
            }

            return ExceptChars(left.ToLower(_culture), new[] {' ', '\t', '\n', '\r'}).Equals(ExceptChars(right.ToLower(_culture), new[] {' ', '\t', '\n', '\r'}));
        }

        public SpaceCaseInsensitiveComparator IgnoreLines(Func<string, bool> func)
        {
            _ignoreLines = func;
            return this;
        }
        #endregion

        #region Methods
        static string ExceptChars(string str, char[] toExclude)
        {
            var sb = new StringBuilder();
            foreach (var c in str)
            {
                if (!toExclude.Contains(c))
                {
                    sb.Append(c);
                }
            }
            return sb.ToString();
        }
        #endregion
    }
}