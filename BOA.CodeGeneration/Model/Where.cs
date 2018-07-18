using WhiteStone.Common;

namespace BOA.CodeGeneration.Model
{
    public class Where : ContractBase
    {
        #region Public Properties
        public string BiggerThan { get; set; }

        public string BiggerThanOrEquals { get; set; }

        public bool CanBeNull { get; set; }

        public string Contains { get; set; }

        public string EndsWith { get; set; }

        public string Equal { get; set; }

        public string IN { get; set; }

        public string IsNull { get; set; }

        public string LessThan { get; set; }

        public string LessThanOrEquals { get; set; }

        public string NotEqual { get; set; }

        public string StartsWith { get; set; }
        #endregion
    }
}