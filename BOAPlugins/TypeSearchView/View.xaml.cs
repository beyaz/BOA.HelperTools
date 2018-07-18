using System;
using System.ComponentModel;
using System.Windows;
using FeserWard.Controls;

namespace BOAPlugins.TypeSearchView
{
    partial class View : INotifyPropertyChanged
    {
        #region Constructors
        public View()
        {
            InitializeComponent();
        }
        #endregion

        #region Public Properties
        public string SelectedTypeFullName { get; set; }
        #endregion

        #region Public Methods
        public void button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region INotifyPropertyChanged Members
        /// <summary>
        ///     Notifies clients that a property value has changed.
        /// </summary>
        [field: NonSerialized]
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        ///     Notifies clients that a property value has changed.
        /// </summary>
        /// <param name="prop"></param>
        public virtual void OnPropertyChanged(string prop)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
            }
        }
        #endregion

        #region IIntelliboxResultsProvider QueryProvider
        IIntelliboxResultsProvider _queryProvider;

        public IIntelliboxResultsProvider QueryProvider
        {
            get { return _queryProvider; }
            set
            {
                if (_queryProvider != value)
                {
                    _queryProvider = value;
                    OnPropertyChanged("QueryProvider");
                }
            }
        }
        #endregion
    }
}