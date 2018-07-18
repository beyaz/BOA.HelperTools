using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using BOA.CodeGeneration.Util;
using BOA.DatabaseAccess;
using Microsoft.SqlServer.TransactSql.ScriptDom;
using WhiteStone.Helpers;

namespace BOA.CodeGeneration.SQLParser
{
    public interface IProcedureDefinition
    {
        #region Public Properties
        string                     Comment    { get; }
        IList<IProcedureParameter> Parameters { get; }
        #endregion

        #region Public Methods
        IReadOnlyList<DataReaderColumn> TryToGetReturnColumnNames(IDatabase db);
        #endregion
    }

    public class DataReaderColumn
    {
        #region Constructors
        public DataReaderColumn(string columnName, Type dataType, bool allowDBNull, string dataTypeName)
        {
            ColumnName   = columnName;
            DataType     = dataType;
            AllowDBNull  = allowDBNull;
            DataTypeName = dataTypeName;
        }
        #endregion

        #region Public Properties
        public bool   AllowDBNull  { get; }
        public string ColumnName   { get; }
        public Type   DataType     { get; }
        public string DataTypeName { get; }
        #endregion
    }

    class ProcedureDefinition : IProcedureDefinition
    {
        #region Fields
        readonly TSqlScript _script;

        IList<IProcedureParameter> _parameters;

        IReadOnlyList<DataReaderColumn> _returnColumns;
        #endregion

        #region Constructors
        public ProcedureDefinition(TSqlScript script)
        {
            if (script == null)
            {
                throw new ArgumentNullException(nameof(script));
            }

            _script = script;
        }
        #endregion

        #region Public Properties
        public string Comment
        {
            get
            {
                var commentToken = Statement.ScriptTokenStream.FirstOrDefault(x => x.TokenType == TSqlTokenType.MultilineComment);
                if (commentToken != null)
                {
                    return commentToken.Text;
                }

                return null;
            }
        }

        public IList<IProcedureParameter> Parameters
        {
            get
            {
                if (_parameters == null)
                {
                    _parameters = new List<IProcedureParameter>();

                    foreach (var procedureParameter in Statement.Parameters)
                    {
                        _parameters.Add(new ProcedureParameter(procedureParameter));
                    }
                }

                return _parameters;
            }
        }
        #endregion

        #region Properties
        CreateProcedureStatement Statement => (CreateProcedureStatement) _script.Batches.First().Statements.First();
        #endregion

        #region Public Methods
        public IReadOnlyList<DataReaderColumn> TryToGetReturnColumnNames(IDatabase db)
        {
            if (_returnColumns != null)
            {
                return _returnColumns;
            }

            var list = new List<DataReaderColumn>();

            _returnColumns = list;

            db.CommandText = string.Join(".", Statement.ProcedureReference.Name.Identifiers.Select(x => x.Value));

            Parameters.ForEach(p => db[p.Name] = p.GetDefaultValueForCallingProcedure());

            db.CommandIsStoredProcedure = true;

            var reader = db.ExecuteReader();

            var dt = reader.GetSchemaTable();
            if (dt == null)
            {
                return list;
            }

            foreach (DataRow row in dt.Rows)
            {
                var columnName   = row["ColumnName"] as string;
                var allowDbNull  = (bool) row["AllowDBNull"];
                var dataType     = row["DataType"] as Type;
                var dataTypeName = row["DataTypeName"] as string;

                list.Add(new DataReaderColumn(columnName, dataType, allowDbNull, dataTypeName));
            }

            return list;
        }
        #endregion
    }
}