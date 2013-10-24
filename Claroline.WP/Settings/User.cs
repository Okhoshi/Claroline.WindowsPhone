using System;
using System.ComponentModel;

namespace ClarolineApp.Settings
{
    public class User : INotifyPropertyChanged
    {
        private String _firstName;
        private String _lastName;
        private int _userID;
        private String _officialCode;
        private bool _isCourseCreator;
        private bool _isPlatformAdmin;
        private String _language;

        public User()
        {
            _firstName = "";
            _lastName = "";
            _officialCode = "";
            _userID = -1;
            _isCourseCreator = false;
            _isPlatformAdmin = false;
            _language = "";
        }

        public String firstName 
        {
            get
            {
                return _firstName;
            }
            set
            {
                if (_firstName != value)
                {
                    _firstName = value;
                    NotifyPropertyChanged("firstName");
                }
            }
        }

        public String lastName
        {
            get
            {
                return _lastName;
            }
            set
            {
                if (_lastName != value)
                {
                    _lastName = value;
                    NotifyPropertyChanged("lastName");
                }
            }
        }

        public String language
        {
            get
            {
                return _language;
            }
            set
            {
                if (_language != value)
                {
                    _language = value;
                    NotifyPropertyChanged("firstName");
                }
            }
        }

        public Boolean isCourseCreator
        {
            get
            {
                return _isCourseCreator;
            }
            set
            {
                if (_isCourseCreator != value)
                {
                    _isCourseCreator = value;
                    NotifyPropertyChanged("isCourseCreator");
                }
            }
        }

        public Boolean isPlatformAdmin
        {
            get
            {
                return _isPlatformAdmin;
            }
            set
            {
                if (_isPlatformAdmin != value)
                {
                    _isPlatformAdmin = value;
                    NotifyPropertyChanged("isPlatformAdmin");
                }
            }
        }

        public int userID
        {
            get
            {
                return _userID;
            }
            set
            {
                if (_userID != value)
                {
                    _userID = value;
                    NotifyPropertyChanged("userID");
                }
            }
        }

        public String officialCode
        {
            get
            {
                return _officialCode;
            }
            set
            {
                if (_officialCode != value)
                {
                    _officialCode = value;
                    NotifyPropertyChanged("officialCode");
                }
            }
        }

        public void setUser(User newUser)
        {
            this.firstName = newUser.firstName;
            this.lastName = newUser.lastName;
            this.language = newUser.language;
            this.isCourseCreator = newUser.isCourseCreator;
            this.isPlatformAdmin = newUser.isPlatformAdmin;
            this.officialCode = newUser.officialCode;
            this.userID = newUser.userID;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion
    }
}
