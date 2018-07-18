using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Model
{
    public class WriterContext
    {
        #region Public Properties
        public TableConfig Config { get; set; }

        public string NamespaceNameForTypeClass
        {
            get
            {
                if (Config.NamespaceNameForTypeClass != null)
                {
                    return Config.NamespaceNameForTypeClass;
                }

                return "BOA.Types.{0}".FormatCode(Naming.NamespaceName);
            }
        }
        #endregion

        #region Properties
        internal NamingModel Naming { get; set; }

        internal TableInfo Table { get; set; }
        #endregion
    }
}