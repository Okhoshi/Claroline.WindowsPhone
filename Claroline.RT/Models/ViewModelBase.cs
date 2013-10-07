using System.ComponentModel;
using System.Collections.Generic;
using Windows.ApplicationModel.Resources;

namespace ClarolineApp.RT.Models
{
    public class ViewModelBase : INotifyPropertyChanged
    {
        #region Events
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        #region Constuctors
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelBase"/> class.
        /// </summary>
        public ViewModelBase()
        {
            //
            // define and intialize
            //
            Properties = new Dictionary<string, object>();
        }
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the properties.
        /// </summary>
        /// <value>
        /// The properties.
        /// </value>
        protected Dictionary<string, object> Properties
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the resources.
        /// </summary>
        /// <value>
        /// The resources.
        /// </value>
        public ResourceLoader Resources
        {
            get { return new ResourceLoader(); }
        }

        #endregion

        #region Methods
        /// <summary>
        /// Gets the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <returns></returns>
        public T GetProperty<T>(string name)
        {
            lock (this)
            {
                return Properties.ContainsKey(name) ? (T)Properties[name] : default(T);
            }
        }

        /// <summary>
        /// Sets the property.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        public void SetProperty<T>(string name, T value)
        {
            lock (this)
            {
                if (Properties.ContainsKey(name))
                {
                    if (!Properties[name].Equals(value))
                    {
                        Properties[name] = value;
                        RaisePropertyChanged(name);
                    }
                }
                else
                {
                    Properties.Add(name, value);
                    RaisePropertyChanged(name);
                }
            }
        }

        /// <summary>
        /// Raises the property changed.
        /// </summary>
        /// <param name="name">The name.</param>
        public void RaisePropertyChanged(string name)
        {
            //
            // just call override
            //
            OnPropertyChanged(name);
        }

        /// <summary>
        /// Called when [property changed].
        /// </summary>
        /// <param name="propertyName">Name of the property.</param>
        protected virtual void OnPropertyChanged(string name)
        {
            if (null != PropertyChanged)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion
    }
}
