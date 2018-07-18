using System.Collections.Generic;
using System.IO;
using Microsoft.SqlServer.TransactSql.ScriptDom;

namespace BOA.CodeGeneration.SQLParser
{
    public static class Parser
    {
        #region Public Methods
        public static IProcedureDefinition ParseProcedure(string sqlScript)
        {
            var SqlParser = new TSql120Parser(false);

            IList<ParseError> parseErrors;
            var result = SqlParser.Parse(new StringReader(sqlScript),
                                         out parseErrors);

            if (parseErrors.Count > 0)
            {
                throw new SQLParseException("SqlParseError");
            }

            return new ProcedureDefinition(result as TSqlScript);
        }
        #endregion
    }
}