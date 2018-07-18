using System;
using System.Linq;
using BOA.Common.Helpers;

namespace BOA.CodeGeneration.Services
{
    public interface IBOAProcedureCommentParser
    {
        #region Public Methods
        string GetCommentForDotNet(string comment);
        #endregion
    }

    public class BOAProcedureCommentParser : IBOAProcedureCommentParser
    {
        #region Public Methods
        public string GetCommentForDotNet(string comment)
        {
            if (comment == null)
            {
                return null;
            }

            comment = comment.Split(Environment.NewLine.ToCharArray())
                             .FirstOrDefault(line => line != null &&
                                                     (line.Trim().StartsWith("Purpose", StringComparison.Ordinal) ||
                                                      line.Replace("\t", string.Empty).Trim().StartsWith("*Purpose", StringComparison.Ordinal)));
            if (comment == null)
            {
                return null;
            }

            comment = comment.Replace("\t", string.Empty).Trim().RemoveFromStart("*").RemoveFromStart("Purpose").Trim().RemoveFromStart(":").Trim();

            return comment;
        }
        #endregion
    }
}