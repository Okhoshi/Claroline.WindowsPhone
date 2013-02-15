using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{
    [Table]
    public class Cours : INotifyPropertyChanged, INotifyPropertyChanging
    {

        public Cours()
        {
            _Notif = new EntitySet<CL_Notification>(
                new Action<CL_Notification>(this.attach_Notif),
                new Action<CL_Notification>(this.detach_Notif)
                );
            _Resources = new EntitySet<ResourceList>(
                new Action<ResourceList>(this.attach_Resources),
                new Action<ResourceList>(this.detach_Resources)
                );
        }

        // Define ID: private Notifications, public property and database column.

        private int _Id;

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    NotifyPropertyChanging("Id");
                    _Id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        // Define item name: private Notifications, public property and database column.

        private string _CoursName;

        [Column]
        public string title
        {
            get
            {
                return _CoursName;
            }
            set
            {
                if (_CoursName != value)
                {
                    NotifyPropertyChanging("title");
                    _CoursName = value;
                    NotifyPropertyChanged("title");
                }
            }
        }

        // Define Responsable value: private Notifications, public property and database column.

        private string _Titular;

        [Column]
        public string titular
        {
            get
            {
                return _Titular;
            }
            set
            {
                if (_Titular != value)
                {
                    NotifyPropertyChanging("titular");
                    _Titular = value;
                    NotifyPropertyChanged("titular");
                }
            }
        }

        private string _officialEmail;

        [Column]
        public string officialEmail
        {
            get
            {
                return _officialEmail;
            }
            set
            {
                if (_officialEmail != value)
                {
                    NotifyPropertyChanging("officialEmail");
                    _officialEmail = value;
                    NotifyPropertyChanged("officialEmail");
                }
            }
        }

        public bool isNotified
        {
            get
            {
                foreach (ResourceList list in _Resources)
                {
                    if (list.isNotified)
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        // Define Tag value: private Notifications, public property and database column.

        private string _CoursTag;

        [Column]
        public string sysCode
        {
            get
            {
                return _CoursTag;
            }
            set
            {
                if (_CoursTag != value)
                {
                    NotifyPropertyChanging("sysCode");
                    _CoursTag = value;
                    NotifyPropertyChanged("sysCode");
                }
            }
        }

        private string _officialCode;

        [Column]
        public string officialCode
        {
            get
            {
                return _officialCode;
            }
            set
            {
                if (_officialCode != value)
                {
                    NotifyPropertyChanging("officialCode");
                    _officialCode = value;
                    NotifyPropertyChanged("officialCode");
                }
            }
        }


        #region Collections

        #region Collection Side for Resource - COURS

        // Define the entity set for the collection side of the relationship.

        private EntitySet<ResourceList> _Resources;

        [Association(Storage = "_Resources", OtherKey = "_coursId", ThisKey = "Id")]
        public EntitySet<ResourceList> Resources
        {
            get { return this._Resources; }
            set { this._Resources.Assign(value); }
        }

        // Called during an add operation

        private void attach_Resources(ResourceList _resources)
        {
            NotifyPropertyChanging("Resources");
            _resources.Cours = this;
            _resources.PropertyChanged += resources_PropertyChanged;
        }

        // Called during a remove operation

        private void detach_Resources(ResourceList _resources)
        {
            NotifyPropertyChanging("Resources");
            _resources.Cours = null;
            _resources.PropertyChanged -= this.resources_PropertyChanged;
        }

        void resources_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "isNotified":
                    NotifyPropertyChanged("isNotified");
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Collection Side for NOTIF - COURS

        // Define the entity set for the collection side of the relationship.

        private EntitySet<CL_Notification> _Notif;

        [Association(Storage = "_Notif", OtherKey = "_coursId", ThisKey = "Id")]
        public EntitySet<CL_Notification> Notifications
        {
            get { return this._Notif; }
            set { this._Notif.Assign(value); }
        }

        // Called during an add operation

        private void attach_Notif(CL_Notification _notif)
        {
            NotifyPropertyChanging("Notification");
            _notif.Cours = this;
        }

        // Called during a remove operation

        private void detach_Notif(CL_Notification _notif)
        {
            NotifyPropertyChanging("Notification");
            _notif.Cours = null;
        }

        #endregion

        #endregion

        // Version column aids update performance.

        [Column(IsVersion = true)]
        private Binary _version;

        // Define updated value: private Notifications, public property and database column.

        private bool _updated;

        [Column]
        public bool updated
        {
            get
            {
                return _updated;
            }
            set
            {
                if (_updated != value)
                {
                    NotifyPropertyChanging("updated");
                    _updated = value;
                    NotifyPropertyChanged("updated");
                }
            }
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

        #region INotifyPropertyChanging Members

        public event PropertyChangingEventHandler PropertyChanging;

        // Used to notify that a property is about to change

        private void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(typeof(Cours)))
            {
                Cours Cou = obj as Cours;
                return (Cou._CoursTag == this._CoursTag);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private DateTime _Loaded = DateTime.Parse("01/01/1753");

        [Column]
        public DateTime loaded
        {
            get
            {
                return _Loaded;
            }
            set
            {
                if (value != _Loaded)
                {
                    NotifyPropertyChanging("loaded");
                    _Loaded = value;
                    NotifyPropertyChanged("loaded");
                }
            }
        }

        internal bool loadedToday()
        {
            return _Loaded.AddDays(1).CompareTo(DateTime.Now) >= 0;
        }
    }
}
