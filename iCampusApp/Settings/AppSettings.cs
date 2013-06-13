using System;
using System.IO.IsolatedStorage;
using System.Diagnostics;
using System.Collections.Generic;
using ClarolineApp.Settings;
using System.ComponentModel;
using System.Windows;

namespace ClarolineApp.Settings
{
    public class AppSettings : INotifyPropertyChanged
    {
        private static AppSettings _instance;

        public static AppSettings Instance
        {
            get {
                if (_instance == null)
                {
                    _instance = new AppSettings();
                }
                return AppSettings._instance; 
            }
        }


        // Our isolated storage Settings

        IsolatedStorageSettings settings;

        // The isolated storage key names of our Settings

        internal const string DomainSettingKeyName = "DomainSetting";
        internal const string WebServiceSettingKeyName = "WebServiceSetting";
        internal const string AuthPageSettingKeyName = "AuthPageSetting";
        internal const string ListBoxSettingKeyName = "ListBoxSetting";
        internal const string AdvancedSwitchSettingKeyName = "AdvancedSwitchSetting";
        internal const string CellularDataEnabledSettingKeyName = "CellularDataEnabledSetting";
        internal const string RadioButton3SettingKeyName = "RadioButton3Setting";
        internal const string UserNameSettingKeyName = "UserNameSetting";
        internal const string PasswordSettingKeyName = "PasswordSetting";
        internal const string InstituteSettingKeyName = "InstituteSetting";
        internal const string PlatformSettingKeyName = "PlatformSetting";
        internal const string UserSettingKeyName = "UserSetting";

        // The default value of our Settings

#if DEBUG
        const string UserNameSettingDefault = "qdevos";
        const string PasswordSettingDefault = "elegie24";
        const string DomainSettingDefault = "http://mesconsult.be/clarodev"; // Debug Platform
#else
        const string UserNameSettingDefault = "";
        const string PasswordSettingDefault = "";
        const string DomainSettingDefault = "http://"; // Debug Platform
#endif
        const string WebServiceSettingDefault = "/module/MOBILE/";
        const string AuthPageSettingDefault = "/claroline/auth/login.php";
        const int ListBoxSettingDefault = 0;
        const bool AdvancedSwitchSettingDefault = false;
        const bool CellularDataEnabledSettingDefault = true;
        const bool RadioButton3SettingDefault = false;
        const string InstituteSettingDefault = "";
        const string PlatformSettingDefault = "Claroline";
        const User UserSettingDefault = null;

        public AppSettings()
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                // Get the Settings for this application.
                settings = IsolatedStorageSettings.ApplicationSettings;
            }

            if (UserSetting != null)
            {
                UserSetting.PropertyChanged += UserSetting_PropertyChanged;
            }
        }

        /// <summary>

        /// Update a setting value for our application. If the setting does not

        /// exist, then add the setting.

        /// </summary>

        /// <param name="Key"></param>

        /// <param name="value"></param>

        /// <returns></returns>

        public bool AddOrUpdateValue(string key, Object value)
        {
            bool valueChanged = false;

            // If the key exists

            if (settings.Contains(key))
            {
                // If the value has changed

                if (!settings[key].Equals(value))
                {
                    // Store the new value

                    settings[key] = value;
                    valueChanged = true;
                    NotifyPropertyChanged(key);
                }
            }
            // Otherwise create the key.

            else
            {
                settings.Add(key, value);
                valueChanged = true;
                NotifyPropertyChanged(key);
            }

            return valueChanged;
        }

        /// <summary>

        /// Get the current value of the setting, or if it is not found, set the 

        /// setting to the default setting.

        /// </summary>

        /// <typeparam name="T"></typeparam>

        /// <param name="Key"></param>

        /// <param name="defaultValue"></param>

        /// <returns></returns>

        public T GetValueOrDefault<T>(string key, T defaultValue)
        {
            T value;

            // If the key exists, retrieve the value.

            if (settings.Contains(key))
            {
                value = (T)settings[key];
            }
            // Otherwise, use the default value.

            else
            {
                value = defaultValue;
            }
            return value;
        }

        /// <summary>

        /// Save the Settings.

        /// </summary>

        public void Save()
        {
            settings.Save();
        }

        /// <summary>

        /// Property to get and set a Domain Setting Key.

        /// </summary>

        public string DomainSetting
        {
            get
            {
                return GetValueOrDefault<string>(DomainSettingKeyName, DomainSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(DomainSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>

        /// Property to get and set a WebService Setting Key.

        /// </summary>

        public string WebServiceSetting
        {
            get
            {
                return GetValueOrDefault<string>(WebServiceSettingKeyName, WebServiceSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(WebServiceSettingKeyName, value))
                {
                    Save();
                }
            }
        }


        /// <summary>

        /// Property to get and set a Auth Page Setting Key.

        /// </summary>

        public string AuthPageSetting
        {
            get
            {
                return GetValueOrDefault<string>(AuthPageSettingKeyName, AuthPageSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(AuthPageSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>

        /// Property to get and set a ListBox Setting Key.

        /// </summary>

        public int ListBoxSetting
        {
            get
            {
                return GetValueOrDefault<int>(ListBoxSettingKeyName, ListBoxSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(ListBoxSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>

        /// Property to get and set a RadioButton Setting Key.

        /// </summary>

        public bool AdvancedSwitchSetting
        {
            get
            {
                return GetValueOrDefault<bool>(AdvancedSwitchSettingKeyName, AdvancedSwitchSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(AdvancedSwitchSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>

        /// Property to get and set a RadioButton Setting Key.

        /// </summary>

        public bool CellularDataEnabledSetting
        {
            get
            {
                return GetValueOrDefault<bool>(CellularDataEnabledSettingKeyName, CellularDataEnabledSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(CellularDataEnabledSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>

        /// Property to get and set a RadioButton Setting Key.

        /// </summary>

        public bool RadioButton3Setting
        {
            get
            {
                return GetValueOrDefault<bool>(RadioButton3SettingKeyName, RadioButton3SettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(RadioButton3SettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>

        /// Property to get and set a Username Setting Key.

        /// </summary>

        public string UserNameSetting
        {
            get
            {
                return GetValueOrDefault<string>(UserNameSettingKeyName, UserNameSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(UserNameSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>

        /// Property to get and set a Password Setting Key.

        /// </summary>

        public string PasswordSetting
        {
            get
            {
                return GetValueOrDefault<string>(PasswordSettingKeyName, PasswordSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(PasswordSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        public string InstituteSetting
        {
            get
            {
                return GetValueOrDefault<string>(InstituteSettingKeyName, InstituteSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(InstituteSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        public string PlatformSetting
        {
            get
            {
                return GetValueOrDefault<string>(PlatformSettingKeyName, PlatformSettingDefault);
            }
            set
            {
                if (AddOrUpdateValue(PlatformSettingKeyName, value))
                {
                    Save();
                }
            }
        }

        /// <summary>

        /// Property to get and set a User Setting Key.

        /// </summary>

        public User UserSetting
        {
            get
            {
                return GetValueOrDefault<User>(UserSettingKeyName, UserSettingDefault);
            }
            set
            {
                if (UserSetting != null)
                {
                    UserSetting.PropertyChanged -= UserSetting_PropertyChanged;
                }

                if (AddOrUpdateValue(UserSettingKeyName, value))
                {
                    Save();
                }
                UserSetting.PropertyChanged += UserSetting_PropertyChanged;
            }
        }

        private void UserSetting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(UserSettingKeyName);
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName))
                );
            }
        }

        #endregion

        internal void Reset()
        {
            UserSetting = new User();
            PlatformSetting = PlatformSettingDefault;
            PasswordSetting = PasswordSettingDefault;
            UserNameSetting = UserNameSettingDefault;
            InstituteSetting = InstituteSettingDefault;
            DomainSetting = DomainSettingDefault;
        }
    }
}
