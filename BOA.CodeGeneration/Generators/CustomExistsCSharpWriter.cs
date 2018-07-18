using BOA.CodeGeneration.Model;

namespace BOA.CodeGeneration.Generators
{
    class CustomExistsCSharpWriter : SelectByColumnsCs
    {
        #region Constructors
        #region Constructor
        public CustomExistsCSharpWriter(WriterContext context, CustomSelectMethod customSelect)
            : base(context, customSelect)
        {
        }
        #endregion
        #endregion

        #region Properties
        internal override string ForcedComment => new CustomExistSql(Context, CustomSelect).GetDefaultComment();

        internal override string GenericResponseMethodReturnType => "bool";

        protected override string ExecutionMethod => "ExecuteScalar<int>";
        #endregion

        #region Methods
        protected override void ProcessReturnValues()
        {
            WriteLine("var result = sp.Value;");
            WriteLine();

            WriteLine("var exist = 1;");
            WriteLine("returnObject.Value = result == exist;");
        }
        #endregion
    }
}