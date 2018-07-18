using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using BOAPlugins.ViewClassDependency;

namespace BOAPlugins.VSIntegration
{
    /// <summary>
    ///     Interaction logic for MainForm.xaml
    /// </summary>
    public partial class MainForm
    {
        #region Constructors
        public MainForm()
        {
            InitializeComponent();
            Loaded += OnLoadCompleted;

            KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    Close();
                }
            };
        }
        #endregion

        #region Public Properties
        public IVisualStudioLayer VisualStudio { get; set; }
        #endregion

        #region Properties
        Command Command
        {
            get
            {
                return new Command
                {
                    VisualStudio = VisualStudio
                };
            }
        }

        Communication Communication
        {
            get { return new Communication(VisualStudio); }
        }
        #endregion

        #region Methods
        void OnLoadCompleted(object sender, RoutedEventArgs e)
        {
            buttonUpdateMessageCs.Focus();

            if (string.IsNullOrWhiteSpace(VisualStudio.CursorSelectedText))
            {
                buttonViewMethodCallGraph.Visibility = Visibility.Collapsed;
            }
            else
            {
                buttonViewMethodCallGraph.Content = buttonViewMethodCallGraph.Content + ": " + VisualStudio.CursorSelectedText;
            }
        }

        void OpenPluginDirectory(object sender, RoutedEventArgs e)
        {
            Close();

            Process.Start(DirectoryHelper.PluginDirectory);
        }

        void GenerateForm(object sender, RoutedEventArgs e)
        {
            Close();

            var view = new FormApplicationGenerator.View
            {
                VisualStudio = VisualStudio
            };
            view.ShowDialog();
        }
        

        void Reload_Config_File(object sender, RoutedEventArgs e)
        {
            Configuration.Initialize();
            Close();

            VisualStudio.UpdateStatusbarText("Config file reloaded.");
        }

        // TODO: remove with sub tree
        void ShowPropertyGenerator(object sender, RoutedEventArgs e)
        {
            Communication.ShowPropertyGenerator();
            Close();
        }

        void UpdateMessageCs(object sender, RoutedEventArgs e)
        {
            Command.UpdateMessageCs();
            Close();
        }
        void UpdateMessageTsx(object sender, RoutedEventArgs e)
        {
            Command.UpdateMessageTsx();
            Close();
        }
        

        void UpdateTypeScriptModels(object sender, RoutedEventArgs e)
        {
            Command.UpdateTypeScriptModels();
            Close();
        }

        void ViewMethodCallGraph(object sender, RoutedEventArgs e)
        {
            Close();

            var input = new Data
            {
                AssemblySearchDirectoryPath = VisualStudio.GetBinFolderPathOfActiveProject(),
                SelectedText                = VisualStudio.CursorSelectedText,
                ActiveProjectName           = VisualStudio.ActiveProjectName
            };

            Communication.Send(input);
        }
        #endregion
    }
}