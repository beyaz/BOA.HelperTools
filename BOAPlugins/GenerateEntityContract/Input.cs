using BOAPlugins.Models;

namespace BOAPlugins.GenerateEntityContract
{
    public class Input
    {
        #region Public Properties
        public string SelectedText { get; set; }
        #endregion
    }

    public class Result : ResultBase
    {
        #region Public Properties
        public string ContractClassBody { get; set; }
        #endregion
    }
}