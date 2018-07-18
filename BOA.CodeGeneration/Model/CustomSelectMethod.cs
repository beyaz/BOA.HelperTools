using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Common;

namespace BOA.CodeGeneration.Model
{
    public class CustomSelectMethod : CustomMethod
    {
        #region Constructors
        public CustomSelectMethod()
        {
            Parameters = new List<Where>();
        }
        #endregion

        #region Public Properties
        public MemberAccessibility MemberAccessibility { get; set; }

        public bool MustBeReturnFirstContract { get; set; }

        public bool MustBeReturnReadonlyContract { get; set; }

        /// <summary>
        ///     Indicates ORDER BY Expression.
        /// </summary>
        public string ORDER_BY { get; set; }

        public IReadOnlyList<Where> Parameters { get; set; }

        public string SelectOnlySpecificColumn { get; set; }

        public bool SelectWithStarIsEnabled { get; set; }

        /// <summary>
        ///     Indicates how many records will be select.
        /// </summary>
        public int? TOP_N { get; set; }
        #endregion

        #region Properties
        internal bool IsSelectByValueList
        {
            get
            {
                if (Parameters.Count == 1 &&
                    Parameters.First().IN != null)
                {
                    return true;
                }

                return false;
            }
        }

        internal string SelectByValueListColumnName => Parameters.First().IN;
        #endregion
    }
}