using BOA.CodeGeneration.Model;
using BOA.CodeGeneration.Util;

namespace BOA.CodeGeneration.Generators
{
    class CustomExistSql : SelectByColumnsSql
    {
        #region Constructors
        #region Constructor
        public CustomExistSql(WriterContext context, CustomSelectMethod customSelect)
            : base(context, customSelect)
        {
        }
        #endregion
        #endregion

        #region Methods
        internal override string GetDefaultComment()
        {
            return "Check records exists in table '{0}' from given parameters".FormatCode(Context.Config.DatabaseTableFullPath);
        }

        protected override void WriteBody()
        {
            WriteLine("IF EXISTS");
            WriteLine("(");

            Padding++;
            WriteLine("SELECT TOP 1 1");
            WriteLine("  FROM {0} WITH (NOLOCK)", Context.Config.TablePathForSqlScript);
            WriteWherePart();
            Padding--;

            if (!EndsWithNewLine())
            {
                WriteLine();
            }

            WriteLine(")");

            Padding++;
            WriteLine("SELECT 1 AS Result");
            Padding--;
            WriteLine("ELSE");
            Padding++;
            WriteLine("SELECT 0 AS Result");
            Padding--;
        }
        #endregion
    }
}