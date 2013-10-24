using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
#if WINDOWS_PHONE
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq.Mapping;
#endif
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;

namespace ClarolineApp.Model
{
#if WINDOWS_PHONE
    [Table]
    [Index(Name = "i_SysCode", IsUnique = true, Columns = "sysCode")]
#endif
    public class Cours : INotifyPropertyChanged
#if WINDOWS_PHONE
                       , INotifyPropertyChanging
#endif
    {

        public Cours()
        {
#if WINDOWS_PHONE
            _Resources = new EntitySet<ResourceList>(
                new Action<ResourceList>(this.attach_Resources),
                new Action<ResourceList>(this.detach_Resources)
                );
#endif
            _Resources.CollectionChanged += _resources_CollectionChanged;
        }

        void _resources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ResourceList item in e.OldItems)
                {
                    item.PropertyChanged -= resources_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (ResourceList item in e.NewItems)
                {
                    item.PropertyChanged += resources_PropertyChanged;
                }
            }
        }

        // Define ID: private Notifications, public property and database column.

        private int _Id;
#if WINDOWS_PHONE
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
#endif
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
#if WINDOWS_PHONE
        [Column]
#endif
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

#if WINDOWS_PHONE
        [Column]
#endif
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

#if WINDOWS_PHONE
        [Column]
#endif
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
                return Resources.Any(rl => rl.isNotified);
            }
        }

        // Define Tag value: private Notifications, public property and database column.

        private string _CoursTag;

#if WINDOWS_PHONE
        [Column]
#endif
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

#if WINDOWS_PHONE
        [Column]
#endif
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

#if WINDOWS_PHONE
        #region Collections

        #region Collection Side for Resource - COURS

        // Define the entity set for the collection side of the relationship.

        private EntitySet<ResourceList> _Resources;

        [Association(Storage = "_Resources", OtherKey = "_coursId", ThisKey = "Id", DeleteRule = "Cascade")]
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
            //_resources.PropertyChanged += resources_PropertyChanged;
        }

        // Called during a remove operation

        private void detach_Resources(ResourceList _resources)
        {
            NotifyPropertyChanging("Resources");
            _resources.Cours = null;
            //_resources.PropertyChanged -= this.resources_PropertyChanged;
        }

        #endregion

        #endregion

        // Version column aids update performance.

        [Column(IsVersion = true)]
        private Binary _version;

#else
        private ObservableCollection<ResourceList> _Resources;
        public ObservableCollection<ResourceList> Resources
        {
            get
            {
                return _Resources;
            }
            set
            {
                if (value != _Resources)
                {
                    _Resources = value;
                    NotifyPropertyChanged("Resources");
                }
            }
        }
#endif

        private bool _updated;

#if WINDOWS_PHONE
        [Column]
#endif
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
#if WINDOWS_PHONE
        public event PropertyChangingEventHandler PropertyChanging;
#endif
        // Used to notify that a property is about to change

        private void NotifyPropertyChanging(string propertyName)
        {
#if WINDOWS_PHONE
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
#endif
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(typeof(Cours)))
            {
                Cours Cou = obj as Cours;
                return (Cou._CoursTag.Equals(this._CoursTag));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        private DateTime _Loaded = DateTime.Parse("01/01/1753");


#if WINDOWS_PHONE
        [Column]
#endif
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

        internal void ReloadPropertyChangedHandler()
        {
            foreach (ResourceList item in Resources)
            {
                item.PropertyChanged += resources_PropertyChanged;
                item.ReloadPropertyChangedHandler();
            }
        }

        private void resources_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        internal void UpdateFrom(Cours newCours)
        {
            updated = newCours.updated;
            loaded = newCours.loaded;
            title = newCours.title;
            titular = newCours.titular;
            officialCode = newCours.officialCode;
            officialEmail = newCours.officialEmail;
        }
    }
}
