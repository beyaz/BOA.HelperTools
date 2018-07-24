using System.Collections.Generic;

namespace BOAPlugins.FormApplicationGenerator
{
    static class NamingHelper
    {
        #region Public Methods
        public static void InitializeFieldComponentTypes(IReadOnlyCollection<BField> FormDataClassFields)
        {
            foreach (var field in FormDataClassFields)
            {
                if (field.ComponentType != null)
                {
                    continue;
                }

                var fieldTypeName = field.DotNetType;
                if (fieldTypeName == DotNetType.String)
                {
                    field.ComponentType = ComponentType.BInput;
                    continue;
                }

                if (fieldTypeName == DotNetType.Int32 ||
                    fieldTypeName == DotNetType.Decimal)
                {
                    field.ComponentType = ComponentType.BInputNumeric;
                    continue;
                }

                if (fieldTypeName == DotNetType.DateTime)
                {
                    field.ComponentType = ComponentType.BDateTimePicker;
                }

                if (fieldTypeName == DotNetType.Boolean)
                {
                    field.ComponentType = ComponentType.BCheckBox;
                }
            }
        }
        #endregion
    }
}