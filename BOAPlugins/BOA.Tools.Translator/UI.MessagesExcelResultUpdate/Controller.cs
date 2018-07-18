using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Text;
using BOA.DatabaseAccess;
using BOA.Office.Excel;
using WhiteStone.Helpers;
using WhiteStone.IO;

namespace BOA.Tools.Translator.UI.MessagesExcelResultUpdate
{
    class Controller
    {
        #region Public Properties
        public Model Model { get; set; }
        #endregion

        #region Public Methods
        public static List<Item> ConvertToList(DataTable dt)
        {
            var items = new List<Item>();

            var rowIndex = 0;
            rowIndex++; // pass headers
            foreach (DataRow row in dt.Rows)
            {
                rowIndex++;
                var trValues = (row["MessageText"] + "").Trim();
                if (string.IsNullOrWhiteSpace(trValues))
                {
                    continue;
                }

                var en = GoogleTranslator.TranslateTurkishToEnglish(trValues);

                if (string.IsNullOrWhiteSpace(en))
                {
                    continue;
                }

                var propName = GoogleTranslator.CreatePropertyNameFromSentence(en);
                items.Add(new Item {NameTR = trValues, ExcelRowIndex = rowIndex, NameEN = propName});
            }
            return items;
        }

        public void UpdateEnglishColumns()
        {
            if (Model.GroupId == null)
            {
                throw new ArgumentException("GroupId giriniz");
            }

            var groupId = Model.GroupId;

            var sb = new StringBuilder(
                @"
DECLARE @TranName VARCHAR(20);

	SELECT @TranName = 'MyTransaction';

	BEGIN TRANSACTION @TranName;");
            sb.AppendLine();

            using (var sql = GetSqlManager())
            {
                sql.CommandText = "select Code,PropertyName from BOA.COR.Messaging WHERE GroupId = " + groupId;
                var dt = sql.ExecuteReader().ToDataTable();
                foreach (DataRow row in dt.Rows)
                {
                    var code = row["Code"] + "";
                    var propertyName = row["PropertyName"] + "";

                    var value = EnglishFromPropertyName(propertyName);

                    value = value.Replace("'", "''");
                    sb.AppendLine("INSERT INTO BOA.COR.MessagingDetail(Code,LanguageId,Description)VALUES(" + code + ",/*ingilizce*/2,'" + value + "')");
                }
            }

            sb.AppendLine("COMMIT TRANSACTION @TranName;");

            var tempPath = Path.GetTempPath();

            var path = tempPath + "MessagesInsert.txt";
            new FileService().Write(path, sb.ToString());

            Process.Start(path);
        }

        public void WriteToExcel()
        {
            foreach (var a in Model.list)
            {
                a.NameEN = a.NameEN.Replace(".", "").Replace("-", "").Replace("%", "").Replace(")", "").Replace(":", "").Replace(";", "").Replace("(", "").Replace("İ", "I")
                            .Replace("‘", "")
                            .Replace("/", "")
                            .Replace(",", "");
                if (a.NameEN == "CustomerNo")
                {
                    a.NameEN = "CustomerNumber";
                }

                if (a.NameEN.StartsWith("3"))
                {
                    a.NameEN = "Three" + a.NameEN.Substring(1);
                }
            }

            using (var excel = new Wrapper {FilePath = Model.FileName})
            {
                foreach (var item in Model.list)
                {
                    excel.UpdateCell("Sheet1", item.ExcelRowIndex, 9, item.NameEN);
                }

                excel.Save();
            }
        }
        #endregion

        #region Methods
        static string EnglishFromPropertyName(string propertyName)
        {
            var values = new List<string>();
            var sb = new StringBuilder();

            char? next = null;
            var len = propertyName.Length;
            for (var i = 0; i < len; i++)
            {
                var c = propertyName[i];
                next = null;
                if (i < len - 1)
                {
                    next = propertyName[i + 1];
                }

                if (char.IsUpper(c))
                {
                    if (sb.Length > 0 && next.HasValue && char.IsLower(next.Value))
                    {
                        values.Add(sb.ToString());
                        sb.Clear();
                        sb.Append(c);
                        continue;
                    }
                }
                sb.Append(c);
            }

            if (sb.Length > 0)
            {
                values.Add(sb.ToString());
            }

            //for (int i = 1; i < values.Count; i++)
            //{
            //    var value = values[i];
            //    values[i] = value.First().ToString().ToLowerEN() + value.Substring(1);
            //}

            var returnValue = string.Join(" ", values);

            return returnValue;
        }

        static SqlDatabase GetSqlManager()
        {
            return new SqlDatabase("Server=srvdev\\Atlas;Database=" + "BOA" + ";Trusted_Connection=True; ");
        }
        #endregion
    }
}