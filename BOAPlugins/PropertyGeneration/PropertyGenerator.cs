using BOA.CodeGeneration.Generators;

namespace BOAPlugins.PropertyGeneration
{
    /// <summary>
    ///     .
    /// </summary>
    public class PropertyGenerator
    {
        #region Public Methods
        /// <summary>
        ///     Generates this instance.
        /// </summary>
        public string Generate(string typeName, string propertyName)
        {
            var writer = new ContractBodyGenerator();

            writer.WriteProperty(typeName, propertyName, null);

            return writer.GeneratedString;
        }
        #endregion
    }
}