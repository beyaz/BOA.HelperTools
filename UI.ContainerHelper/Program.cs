using System;
using System.Linq;
using System.Windows;
using BOA.Tools.Translator.UI.TranslateHelper;

namespace UI.ContainerHelper
{
    /// <summary>
    ///     The program.
    /// </summary>
    class Program
    {
        #region Public Methods
        [STAThread]
        public static void Main(string[] args)
        {
            // new Application().Run(new BOAPlugins.FormApplicationGenerator.View());

            if (args == null)
            {
                throw new ArgumentException(nameof(args));
            }

            if (args.First() == typeof(View).FullName)
            {
                new Application().Run(View.Create());
                return;
            }

            if (args.First() == typeof(BOAPlugins.PropertyGeneration.View).FullName)
            {
                new Application().Run(BOAPlugins.PropertyGeneration.View.Create());
                return;
            }

            throw new InvalidOperationException(args.First());
        }
        #endregion
    }
}