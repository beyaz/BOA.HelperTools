using System.Collections.Generic;
using System.Linq;
using WhiteStone.Common;

namespace BOA.CodeGeneration.Model
{
    public sealed class TableInfo : ContractBase
    {
        #region Constructors
        #region Constructor
        public TableInfo()
        {
            _columns = new List<ColumnInfo>();
        }
        #endregion
        #endregion

        #region Public Properties
        public IEnumerable<ColumnInfo> PrimaryKeyColumns => from c in _columns
                                                            where c.IsPrimaryKey
                                                            select c;
        #endregion

        #region Properties
        internal bool HasIdentityColumn => IdentityColumn != null;

        internal ColumnInfo IdentityColumn
        {
            get { return _columns.FirstOrDefault(c => c.IsIdentity); }
        }
        #endregion

        #region Methods
        internal void AddColumn(ColumnInfo column)
        {
            _columns.Add(column);
        }

        internal void ExcludeColumns(string excludeColumns)
        {
            if (string.IsNullOrWhiteSpace(excludeColumns))
            {
                return;
            }

            var columnNames = excludeColumns.Split(',');

            _columns.RemoveAll(x => columnNames.Contains(x.ColumnName));
        }
        #endregion

        #region List<ColumnInfo> Columns
        readonly List<ColumnInfo> _columns;

        public IReadOnlyList<ColumnInfo> Columns => _columns;
        #endregion
    }
}