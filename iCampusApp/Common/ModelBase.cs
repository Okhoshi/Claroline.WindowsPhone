using GalaSoft.MvvmLight;
using System.ComponentModel;

namespace ClarolineApp.Common
{
    public class ModelBase : ViewModelBase, INotifyPropertyChanging
    {
        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to Raise that a property is about to change

        protected void RaisePropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion
    }
}
