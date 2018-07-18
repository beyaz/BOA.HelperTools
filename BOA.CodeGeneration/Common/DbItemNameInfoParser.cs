using System;

namespace BOA.CodeGeneration.Common
{
    public class DbItemNameInfoParser
    {
        #region Public Methods
        public static DbItemNameInfo Parse(string input)
        {
            var info         = new DbItemNameInfo();
            var selectedText = ProcedureTextUtility.ClearProcedureText(input);

            var list = selectedText.Split('.');

            if (!(list.Length == 2 || list.Length == 3))
            {
                throw new ArgumentException("Yanlış veri girildi. 'Schema.Procedure' veya 'Database.Schema.Procedure' şeklinde seçilmeli.");
            }

            if (list.Length == 2)
            {
                info.SchemaName = list[0].Trim();
                info.Name       = list[1].Trim();
            }
            else if (list.Length == 3)
            {
                info.DatabaseName = list[0].Trim();
                info.SchemaName   = list[1].Trim();
                info.Name         = list[2].Trim();
            }

            return info;
        }
        #endregion
    }
}