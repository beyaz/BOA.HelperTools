using System.Linq;
using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Generators
{
    class SelectByKeyListSql : SelectByValueArraySql
    {
        #region Constructors
        public SelectByKeyListSql(WriterContext context)
            : base(context, context.Table.PrimaryKeyColumns.First().ColumnName)
        {
        }
        #endregion
    }
}