using System;
using System.Collections.Generic;
using System.Linq;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Generators
{
    class UpdateCsCustom : UpdateCs
    {
        #region Fields
        readonly IEnumerable<ColumnInfo> _procedureParameters;
        #endregion

        #region Constructors
        public UpdateCsCustom(WriterContext context, CustomUpdateMethod customUpdateMethod)
            : base(context)
        {
            CustomUpdate = customUpdateMethod;

            var updateColumns = customUpdateMethod.UpdateColumnNames.Split(',');
            var whereColumns  = customUpdateMethod.WhereColumnNames.Split(',');

            foreach (var columnName in updateColumns)
            {
                if (Context.Table.Columns.All(x => x.ColumnName != columnName))
                {
                    throw new ArgumentException("InvalidColumnName:" + columnName);
                }
            }

            foreach (var columnName in whereColumns)
            {
                if (Context.Table.Columns.All(x => x.ColumnName != columnName))
                {
                    throw new ArgumentException("InvalidColumnName:" + columnName);
                }
            }

            _procedureParameters = from c in Context.Table.Columns
                                   where updateColumns.Contains(c.ColumnName) ||
                                         whereColumns.Contains(c.ColumnName) ||
                                         Names.GenericUpdateInformationColumns.Contains(c.ColumnName)
                                   select c;
        }
        #endregion

        #region Public Properties
        public CustomUpdateMethod CustomUpdate { get; set; }
        #endregion

        #region Properties
        protected override string Comment => CustomUpdate.Comment;

        protected override MemberAccessibility MemberAccessibility => CustomUpdate.MethodAccessiblity;

        protected override string NameOfCsMethod => CustomUpdate.DotNetMethodName;

        protected override string NameOfSqlProcedure => CustomUpdate.SqlProcedureName;
        #endregion

        #region Methods
        protected override IEnumerable<ColumnInfo> GetUpdateColumns()
        {
            return _procedureParameters;
        }
        #endregion
    }
}