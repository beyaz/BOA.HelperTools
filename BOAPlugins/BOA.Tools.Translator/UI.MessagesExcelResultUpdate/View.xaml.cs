using System.Windows;
using BOA.Office.Excel;
using Microsoft.Win32;

namespace BOA.Tools.Translator.UI.MessagesExcelResultUpdate
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class View
    {
        #region Constructors
        public View()
        {
            InitializeComponent();
            Model = new Model();
        }
        #endregion

        #region Public Properties
        public Model Model { get; }
        #endregion

        #region Properties
        Controller Controller => new Controller {Model = Model};
        #endregion

        #region Methods
        void Button_Click(object sender, RoutedEventArgs e)
        {
            var fileDialog = new OpenFileDialog {Filter = "Excel File (*.xlsx)|*.xlsx|Excel File (*.xls)|*.xls"};

            if (fileDialog.ShowDialog() == true)
            {
                Model.FileName = fileDialog.FileName;
            }

            if (string.IsNullOrWhiteSpace(Model.FileName))
            {
                return;
            }

            var dt = Reader.ReadFromFile(Model.FileName, "Sheet1");

            Model.list = Controller.ConvertToList(dt);
        }

        void UpdateEnglishColumns(object sender, RoutedEventArgs e)
        {
            Controller.UpdateEnglishColumns();
        }

        void WriteToExcel(object sender, RoutedEventArgs e)
        {
            Controller.WriteToExcel();
        }
        #endregion
    }
}