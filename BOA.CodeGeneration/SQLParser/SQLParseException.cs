using System;
using System.Runtime.Serialization;

namespace BOA.CodeGeneration.SQLParser
{
    [Serializable]
    public class SQLParseException : Exception
    {
        #region Constructors
        public SQLParseException()
        {
        }

        public SQLParseException(string message) : base(message)
        {
        }

        public SQLParseException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected SQLParseException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
        #endregion
    }
}