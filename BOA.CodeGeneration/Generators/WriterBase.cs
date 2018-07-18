using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BOA.CodeGeneration.Common;
using BOA.CodeGeneration.Model;
using BOA.Common.Helpers;
using WhiteStone.Common;
using WhiteStone.Helpers;

namespace BOA.CodeGeneration.Generators
{
    public class WriterBaseBase : ContractBase
    {
        #region Constants
        protected const int PaddingLength = 4;
        #endregion

        #region Fields
        protected readonly CultureInfo CultureInfo = new CultureInfo("en-US");
        StringBuilder                  _output;
        #endregion

        #region Constructors
        public WriterBaseBase()
        {
            ClearOutput();
        }
        #endregion

        #region Public Properties
        public string GeneratedString => _output.ToString();

        public int Padding { get; set; }
        #endregion

        #region Properties
        protected static string PaddingForComment => "     ";

        protected static int PaddingForMethodDeclaration => 2;
        #endregion

        #region Public Methods
        public void WriteProperty(string propName, string propertyChangedString, string dotNetType, string propertyName, string comment)
        {
            propName = CheckName(propName);

            WriteLine("#region " + dotNetType + " " + propertyName);
            WriteLine(dotNetType + " " + propName + ";");
            if (comment != null)
            {
                WriteLine(@"/// <summary>");
                var commentList    = comment.Split(Environment.NewLine.ToCharArray());
                var isFirstComment = true;
                foreach (var item in commentList)
                {
                    if (isFirstComment)
                    {
                        isFirstComment = false;
                        WriteLine("///" + PaddingForComment + "" + item);
                    }
                    else
                    {
                        WriteLine("///" + PaddingForComment + "<para> " + item + " </para>");
                    }
                }

                WriteLine(@"/// </summary>");
            }

            WriteLine("public " + dotNetType + " " + propertyName);
            WriteLine("{");
            Padding++;
            WriteLine("get{ return " + propName + ";}");
            WriteLine("set");
            WriteLine("{");
            Padding++;

            if (dotNetType.IsStartsWith("IReadOnlyCollection<") ||
                dotNetType.IsStartsWith("IReadOnlyList<"))
            {
                WriteLine("if(!Equals(" + propName + ", value))");
            }
            else
            {
                WriteLine("if( " + propName + " != value )");
            }

            WriteLine("{");
            Padding++;
            WriteLine(propName + " = value;");
            WriteLine("OnPropertyChanged(" + propertyChangedString + ");");
            Padding--;
            WriteLine("}");
            Padding--;
            WriteLine("}");
            Padding--;
            WriteLine("}");
            WriteLine("#endregion");
        }
        #endregion

        #region Methods
        protected void ClearOutput()
        {
            _output = new StringBuilder();
        }

        protected bool EndsWithNewLine()
        {
            return _output.EndsWith(Environment.NewLine);
        }

        protected void Write(string value)
        {
            Append(value);
        }

        protected void Write(string format, object arg0)
        {
            _output.AppendFormat(format, arg0);
        }

        protected void Write(string format, params object[] args)
        {
            _output.AppendFormat(CultureInfo, format, args);
        }

        protected void WriteLine(string value)
        {
            WritePadding();
            AppendLine(value);
        }

        protected void WriteLine()
        {
            AppendLine();
        }

        protected void WriteLine(string format, params object[] args)
        {
            WritePadding();
            _output.AppendFormat(CultureInfo, format, args);
            AppendLine();
        }

        protected void WritePadding()
        {
            if (Padding > 0)
            {
                _output.Append("".PadRight(Padding * PaddingLength, ' '));
            }
        }

        static string CheckName(string propName)
        {
            if (propName == "operator")
            {
                return "_" + propName;
            }

            return propName;
        }

        void Append(string value)
        {
            _output.Append(value);
        }

        void AppendLine(string value)
        {
            _output.AppendLine(value);
        }

        void AppendLine()
        {
            _output.AppendLine();
        }
        #endregion
    }

    public class WriterBase : WriterBaseBase
    {
        #region Constructors
        #region Constructor
        public WriterBase(WriterContext context)
        {
            Context = context;
        }
        #endregion
        #endregion

        #region Properties
        protected WriterContext Context { get; }

        string ProgrammerFullName => "Abdullah BEYAZTAŞ";
        #endregion

        #region Methods
        protected void GenerateSqlSelectByColumns_WriteColumnsForReturn(IReadOnlyList<ColumnInfo> columns)
        {
            if (columns == null)
            {
                throw new ArgumentNullException(nameof(columns));
            }

            var len = columns.Count;
            Padding++;
            Padding++;
            for (var i = 0; i < len; i++)
            {
                var row = columns[i];

                var isLast = i == len - 1;

                WriteLine(isLast ? "{0}" : "{0},", row.ColumnName.NormalizeColumnNameForReversedKeywords());
            }

            Padding--;
            Padding--;
        }

        protected void WriteSqlComment(IList<string> comments)
        {
            if (comments == null)
            {
                throw new ArgumentNullException(nameof(comments));
            }

            WriteLine("/* {0}", Context.Naming.CompanyNameWillbeShownInSqlComments);
            WriteLine();
            WriteLine("    All rights are reserved. Reproduction or transmission in whole or in part, in");
            WriteLine("    any form or by any means, electronic, mechanical or otherwise, is prohibited");
            WriteLine("    without the prior written consent of the copyright owner.");
            WriteLine();
            WriteLine("    Generator Information");
            WriteLine("        Generated By  : {0}", ProgrammerFullName);
            for (var i = 0; i < comments.Count; i++)
            {
                if (i == 0)
                {
                    WriteLine("        Purpose       : " + comments[i]);
                    continue;
                }

                WriteLine("                      : " + comments[i]);
            }

            WriteLine("*/");
        }

        protected void WriteUpdateInformationColumnsForContract(bool UpdateUserName, bool UpdateHostName, bool UpdateHostIP, bool UpdateSystemDate)
        {
            var contractUpdated = false;
            if (UpdateUserName)
            {
                contractUpdated = true;
                WriteLine("contract.{0} = {1};", Names.UpdateUserName, Names.UpdateUserNameValue);
            }

            if (UpdateHostName)
            {
                contractUpdated = true;
                WriteLine("contract.{0} = {1};", Names.UpdateHostName, Names.UpdateHostNameValue);
            }

            if (UpdateHostIP)
            {
                contractUpdated = true;
                WriteLine("contract.{0} = {1};", Names.UpdateHostIP, Names.UpdateHostIPValue);
            }

            if (UpdateSystemDate)
            {
                contractUpdated = true;
                WriteLine("contract.{0} = {1};", Names.UpdateSystemDate, Names.UpdateSystemDateValue);
            }

            if (contractUpdated)
            {
                WriteLine();
            }
        }
        #endregion
    }
}