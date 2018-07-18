using System.Windows;
using System.Windows.Input;
using WhiteStone.Helpers;

namespace BOAPlugins.PropertyGeneration
{
    public partial class View
    {
        #region Constructors
        public View()
        {
            InitializeComponent();

            ClearForm();
        }
        #endregion

        #region Public Properties
        public Model Model { get; set; }
        #endregion

        #region Properties
        Controller Controller => new Controller {Model = Model};
        #endregion

        #region Public Methods
        public static Window Create()
        {
            var view = new View();
            var window = new Window
            {
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                Width = 900,
                Height = 400,
                Content = view,
                Title = "Property Generation (Close: ESC)"
            };

            window.KeyDown += (sender, e) =>
            {
                if (e.Key == Key.Escape)
                {
                    window.Close();
                }
            };

            return window;
        }

        public void ClearForm()
        {
            DataContext = Model = new Model();
            Model.OnPropertyChanged("Input", () => { Controller.InputChanged(); });
        }
        #endregion
    }
}