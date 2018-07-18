namespace BOAPlugins.SearchProcedure
{
    public class Input
    {
        #region Public Properties
        public string ProcedureName { get; set; }
        #endregion

        #region Public Methods
        public override string ToString()
        {
            return ProcedureName;
        }
        #endregion
    }
}