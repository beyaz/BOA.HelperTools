using WhiteStone.Mvc;

namespace BOAPlugins.PropertyGeneration
{
    /// <summary>
    ///     The controller
    /// </summary>
    class Controller : ControllerBase<Model>
    {
        #region Public Methods
        /// <summary>
        ///     Inputs the changed.
        /// </summary>
        public void InputChanged()
        {
            Model.Output = new MultiplePropertyGenerator(Model.Input).Generate();
        }
        #endregion
    }
}