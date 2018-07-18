using BOAPlugins.GenerateSelectByKeySql;
using BOAPlugins.ViewClassDependency;

namespace BOAPlugins.VSIntegration
{
    public interface ICommunication
    {
        #region Public Methods
        void GenerateSelectByKeySql(Input input);
        void GenerateUpdateSql(GenerateUpdateSql.Input input);

        /// <summary>
        ///     Sends the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        void Send(SearchProcedure.Input input);

        /// <summary>
        ///     Sends the specified input.
        /// </summary>
        /// <param name="input">The input.</param>
        void Send(GenerateCSharpCode.Input input);

        void Send(GenerateInsertSql.Input input);

        void Send(GenerateEntityContract.Input input);
        void Send(Data input);

        /// <summary>
        ///     Shows the property generator.
        /// </summary>
        void ShowPropertyGenerator();

        void ShowTranslateHelperForLabels(); // TODO remove  unused method
        #endregion
    }
}