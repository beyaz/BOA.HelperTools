using System;
using System.ComponentModel;
using System.Windows.Controls;

namespace BOAPlugins
{
    public class UserControlBase : UserControl, INotifyPropertyChanged
    {
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
    }
}