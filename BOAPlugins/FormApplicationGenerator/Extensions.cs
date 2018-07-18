using System;
using BOA.Common.Helpers;

namespace BOAPlugins.FormApplicationGenerator
{
    static class Extensions
    {
        #region Public Methods
        public static string GetSnapName(this FieldInfo dataField)
        {
            return $"{dataField.ComponentName.ToString().RemoveFromStart("B").MakeLowerCaseFirstChar()}{dataField.Name}";
        }

        public static string MakeLowerCaseFirstChar(this string value)
        {
            return value?[0].ToString().ToLower() + value?.Substring(1);
        }

        public static bool HasSnapName(this FieldInfo dataField)
        {
            return dataField.ComponentName == ComponentName.BAccountComponent ||
                   dataField.ComponentName == ComponentName.BParameterComponent;
        }

        public static string ToCSharp(this DotNetTypeName name)
        {
            if (name == DotNetTypeName.Boolean)
            {
                return "bool?";
            }

            if (name == DotNetTypeName.DateTime)
            {
                return "DateTime?";
            }
            if (name == DotNetTypeName.Decimal)
            {
                return "decimal?";
            }
            if (name == DotNetTypeName.Int32)
            {
                return "int?";
            }
            if (name == DotNetTypeName.String)
            {
                return "string";
            }

            throw new ArgumentException(name.ToString());
        }
        

        #endregion
    }
}