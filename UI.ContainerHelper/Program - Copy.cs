using System;
using System.Linq;
using System.Windows;
using BOA.Tools.Translator.UI.TranslateHelper;

namespace UI.ContainerHelper
{
    class Program
    {
        #region Public Methods
        [STAThread]
        public static void Main(string[] args)
        {
            new Application().Run(new BOAPlugins.FormApplicationGenerator.View());
            
        }
        #endregion
    }
}