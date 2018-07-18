using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BOA.CodeGeneration.Services
{
    public static class SpaceCaseInsensitiveComparator
    {
        #region Static Fields
        static readonly char[] ExceptCharachters = {' ', '\t', '\n', '\r'};
        #endregion

        #region Public Methods
        public static bool Compare(string left, string right)
        {
            return Compare(left, right, CultureInfo.CurrentCulture);
        }

        public static bool Compare(string left, string right, CultureInfo cultureInfo)
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
                throw new ArgumentNullException(nameof(right));
            }

            return ExceptChars(left.ToLower(cultureInfo), ExceptCharachters).Equals(ExceptChars(right.ToLower(cultureInfo), ExceptCharachters));
        }

        public static string ToLowerAndClearSpaces(this string value)
        {
            if (value == null)
            {
                return null;
            }

            return ExceptChars(value.ToLower(), ExceptCharachters);
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