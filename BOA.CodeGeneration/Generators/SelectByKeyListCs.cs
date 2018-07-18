using System.Linq;
using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Generators
{
    class SelectByKeyListCs : SelectByValueListCs
    {
        #region Constructors
        public SelectByKeyListCs(WriterContext context)
            : base(context, context.Table.PrimaryKeyColumns.First().ColumnName, context.Config.SelectByKeyListMethodName)
        {
        }
        #endregion
    }
}