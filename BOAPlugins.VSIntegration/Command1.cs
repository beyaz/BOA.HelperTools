using System;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Windows;
using BOA.CodeGeneration.Util;
using BOAPlugins;
using BOAPlugins.ExportingModel;
using BOAPlugins.GenerateCSharpCode;
using BOAPlugins.ViewClassDependency;
using BOAPlugins.VSIntegration;
using Microsoft.VisualStudio.Shell;
using WhiteStone;
using WhiteStone.IO;
using Handler = BOAPlugins.ExportingModel.Handler;

namespace BOASpSearch
{
    /// <summary>
    ///     Command handler
    /// </summary>
    internal sealed partial class Command1
    {
        #region Constants
        /// <summary>
        ///     Command ID.
        /// </summary>
        public const int CommandId = 0x0100;

        public const int CommandId11 = 11;
        public const int CommandId12 = 12;
        public const int CommandId13 = 13;

        public const int CommandId9 = 0x0900;
        #endregion

        #region Static Fields
        /// <summary>
        ///     Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("02e7ce5d-c034-4f2b-b45a-c89807ed1c77");
        #endregion

        #region Fields
        /// <summary>
        ///     VS Package that provides this command, not null.
        /// </summary>
        readonly Package package;
        #endregion

        #region Constructors
        /// <summary>
        ///     Initializes a new instance of the <see cref="Command1" /> class.
        ///     Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        Command1(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            this.package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                commandService.AddCommand(new MenuCommand(SearchProcedure, new CommandID(CommandSet, CommandId)));
                // commandService.AddCommand(new MenuCommand(GenerateCSharpCodeCallback, new CommandID(CommandSet, CommandId2)));
                // commandService.AddCommand(new MenuCommand(ShowPropertyGenerator, new CommandID(CommandSet, CommandId3)));
                // commandService.AddCommand(new MenuCommand(GenerateInsertSql, new CommandID(CommandSet, CommandId4)));
                // commandService.AddCommand(new MenuCommand(GenerateEntityContract, new CommandID(CommandSet, CommandId5)));
                // commandService.AddCommand(new MenuCommand(ViewMethodCallGraph, new CommandID(CommandSet, CommandId6)));
                // commandService.AddCommand(new MenuCommand(GenerateUpdateSql, new CommandID(CommandSet, CommandId7)));
                // commandService.AddCommand(new MenuCommand(GenerateSelectByKeySql, new CommandID(CommandSet, CommandId8)));
                commandService.AddCommand(new MenuCommand(ViewTypeDependency, new CommandID(CommandSet, CommandId9)));
                commandService.AddCommand(new MenuCommand(DocumentFile, new CommandID(CommandSet, CommandId12)));
                commandService.AddCommand(new MenuCommand(OpenMainForm, new CommandID(CommandSet, CommandId13)));
                commandService.AddCommand(new MenuCommand(CheckInSolution, new CommandID(CommandSet, CommandId11)));

                Factory.InitializeApplicationServices();
            }
        }
        #endregion

        #region Public Properties
        /// <summary>
        ///     Gets the instance of the command.
        /// </summary>
        public static Command1 Instance { get; private set; }
        #endregion

        #region Properties
        static Configuration Configuration => SM.Get<Configuration>();
        ICommunication Communication => Factory.GetCommunication(VisualStudio);

        /// <summary>
        ///     Gets the service provider from the owner package.
        /// </summary>
        IServiceProvider ServiceProvider => package;

        TFSAccessForBOA TFSAccessForBOA => new TFSAccessForBOA();

        IVisualStudioLayer VisualStudio => new VisualStudioLayer(ServiceProvider);
        #endregion

        #region Public Methods
        /// <summary>
        ///     Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new Command1(package);
        }
        #endregion

        #region Methods
        void OpenMainForm(object sender, EventArgs e)
        {
            var mainForm = new BOAPlugins.VSIntegration.MainForm
            {
                VisualStudio =  VisualStudio
            };

            mainForm.ShowDialog();
        }

        void CheckInSolution(object sender, EventArgs e)
        {
            new Thread(CheckInSolution).Start();
        }

        void CheckInSolution()
        {
            VisualStudio.UpdateStatusbarText("Check-in started...");
            var data = new CheckinSolutionInput
            {
                SolutionFilePath = VisualStudio.GetSolutionFilePath(),
                Comment = Configuration.CheckInCommentDefaultValue
            };

            try
            {
                TFSAccessForBOA.CheckinSolution(data);
                VisualStudio.UpdateStatusbarText(data.ResultMessage);
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.ToString());
            }
        }

        void GenerateCSharpCodeCallback(object sender, EventArgs e)
        {
            var selectedText = VisualStudio.CursorSelectedText;
            if (selectedText == null)
            {
                return;
            }

            var input = new Input
            {
                ProcedureName = selectedText
            };

            Communication.Send(input);
        }

        void GenerateEntityContract(object sender, EventArgs e)
        {
            var input = new BOAPlugins.GenerateEntityContract.Input
            {
                SelectedText = VisualStudio.CursorSelectedText
            };

            Communication.Send(input);
        }

        void GenerateInsertSql(object sender, EventArgs e)
        {
            var selectedText = VisualStudio.CursorSelectedText;
            if (selectedText == null)
            {
                return;
            }

            var input = new BOAPlugins.GenerateInsertSql.Input
            {
                TableName = selectedText
            };

            Communication.Send(input);
        }

        void GenerateSelectByKeySql(object sender, EventArgs e)
        {
            var selectedText = VisualStudio.CursorSelectedText;
            if (selectedText == null)
            {
                return;
            }

            var input = new BOAPlugins.GenerateSelectByKeySql.Input
            {
                TableName = selectedText
            };

            Communication.GenerateSelectByKeySql(input);
        }

        void GenerateUpdateSql(object sender, EventArgs e)
        {
            var selectedText = VisualStudio.CursorSelectedText;
            if (selectedText == null)
            {
                return;
            }

            var input = new BOAPlugins.GenerateUpdateSql.Input
            {
                TableName = selectedText
            };

            Communication.GenerateUpdateSql(input);
        }

        /// <summary>
        ///     This function is the callback used to execute the command when the menu item is clicked.
        ///     See the constructor to see how the menu item is associated with this function using
        ///     OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        void SearchProcedure(object sender, EventArgs e)
        {
            var selectedText = VisualStudio.CursorSelectedText;
            if (selectedText == null)
            {
                return;
            }

            var input = new BOAPlugins.SearchProcedure.Input
            {
                ProcedureName = selectedText
            };

            Communication.Send(input);
        }

        void ShowPropertyGenerator(object sender, EventArgs e)
        {
            Communication.ShowPropertyGenerator();
        }

        

        void ViewMethodCallGraph(object sender, EventArgs e)
        {
            var input = new Data
            {
                AssemblySearchDirectoryPath = VisualStudio.GetBinFolderPathOfActiveProject(),
                SelectedText = VisualStudio.CursorSelectedText,
                ActiveProjectName = VisualStudio.ActiveProjectName
            };

            Communication.Send(input);
        }

        void ViewTypeDependency(object sender, EventArgs e)
        {
            var filePath = VisualStudio.ActiveProjectCsprojFilePath;
            if (filePath == null)
            {
                return;
            }

            var dgmlFilePath = filePath + ".dgml";

            var FS = new FileService();
            FS.TryDelete(dgmlFilePath);

            var arguments = string.Format(@"graph={1} source={0} {0}", filePath, dgmlFilePath);
            Process.Start(AssemlyUpdater.PluginDirectory + "DeepEnds\\DeepEnds.Console.exe", arguments);

            var count = 0;
            // wait for process finih
            while (true)
            {
                if (!FS.Exists(dgmlFilePath))
                {
                    Thread.Sleep(300);
                    continue;
                }

                var fi = new FileInfo(dgmlFilePath);
                if (fi.Length > 0)
                {
                    VisualStudio.OpenFile(filePath + ".dgml");
                    return;
                }

                Thread.Sleep(300);
                if (count++ > 50)
                {
                    MessageBox.Show("EvaluationTimeout");
                    return;
                }
            }
        }

        
        #endregion
    }
}