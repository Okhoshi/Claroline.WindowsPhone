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
    public abstract class ResourceModel : INotifyPropertyChanged, INotifyPropertyChanging
    {

        public ResourceModel()
        {
            _notifications = new EntitySet<CL_Notification>(
                new Action<CL_Notification>(attach_Notification),
                new Action<CL_Notification>(detach_Notification)
                );
        }

        public const string LABEL = "NOMOD";

        protected int _Id;

        // Define ID: internal Notifications, public property and database column.

        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
        public virtual int Id
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

        protected string _Title;

        [Column]
        public string title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (value != _Title)
                {
                    NotifyPropertyChanging("title");
                    _Title = value;
                    NotifyPropertyChanged("title");
                }
            }
        }

        protected DateTime _NotifiedDate = DateTime.Parse("01/01/1753");

        [Column]
        public DateTime notifiedDate
        {
            get
            {
                return _NotifiedDate;
            }
            set
            {
                if (_NotifiedDate != value)
                {
                    NotifyPropertyChanging("notifiedDate");
                    _NotifiedDate = value;
                    NotifyPropertyChanged("notifiedDate");
                }
            }
        }

        public bool isNotified
        {
            get
            {
                return _SeenDate.CompareTo(notifiedDate) < 0;
            }
        }

        protected DateTime _SeenDate = DateTime.Parse("01/01/1753");

        [Column]
        public DateTime seenDate
        {
            get
            {
                return _SeenDate;
            }
            set
            {
                if (_SeenDate != value)
                {
                    NotifyPropertyChanging("seenDate");
                    _SeenDate = value;
                    NotifyPropertyChanged("seenDate");
                }
            }
        }

        protected DateTime _Date = DateTime.Parse("01/01/1753");

        [Column]
        public DateTime date
        {
            get
            {
                return _Date;
            }
            set
            {
                if (_Date != value)
                {
                    NotifyPropertyChanging("date");
                    _Date = value;
                    NotifyPropertyChanged("date");
                }
            }
        }

        protected bool _Visibility;

        [Column]
        public bool visibility
        {
            get
            {
                return _Visibility;
            }
            set
            {
                if (_Visibility != value)
                {
                    NotifyPropertyChanging("visibility");
                    _Visibility = value;
                    NotifyPropertyChanged("visibility");
                }
            }
        }

        // Define updated value: internal Notifications, public property and database column.

        protected bool _Updated;

        [Column]
        public bool updated
        {
            get
            {
                return _Updated;
            }
            set
            {
                if (_Updated != value)
                {
                    NotifyPropertyChanging("updated");
                    _Updated = value;
                    NotifyPropertyChanged("updated");
                }
            }
        }

        protected DateTime _Loaded = DateTime.Parse("01/01/1753");

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

        #region Collection Side for Notification

        // Define the entity set for the collection side of the relationship.

        private EntitySet<CL_Notification> _notifications;

        [Association(Storage = "_notifications", OtherKey = "_resourceId", ThisKey = "Id")]
        public EntitySet<CL_Notification> notifications
        {
            get { return this._notifications; }
            set { this._notifications.Assign(value); }
        }

        // Called during an add operation

        private void attach_Notification(CL_Notification _notification)
        {
            NotifyPropertyChanging("notifications");
            _notification.resource = this;
        }

        // Called during a remove operation

        private void detach_Notification(CL_Notification _notification)
        {
            NotifyPropertyChanging("notifications");
            _notification.resource = null;
        }

        #endregion

        #region Entity Side for ResourceList

        [Column]
        protected int _resourceListId;

        protected EntityRef<ResourceList> _resourceList;

        // Association, to describe the relationship between this key and that "storage" table

        [Association(Storage = "_resourceList", ThisKey = "_resourceListId", OtherKey = "Id", IsForeignKey = true)]
        public ResourceList resourceList
        {
            get { return _resourceList.Entity; }
            set
            {
                NotifyPropertyChanging("resourceList");

                _resourceList.Entity = value;
                if (value != null)
                {
                    _resourceListId = value.Id;
                }

                NotifyPropertyChanged("resourceList");
            }
        }

        #endregion

        // Version column aids update performance.

        [Column(IsVersion = true)]
        protected Binary _version;

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify that a property changed

        protected void NotifyPropertyChanged(string propertyName)
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

        protected void NotifyPropertyChanging(string propertyName)
        {
            if (PropertyChanging != null)
            {
                PropertyChanging(this, new PropertyChangingEventArgs(propertyName));
            }
        }

        #endregion

        public abstract string getNotificationText();

        public void markAsRead()
        {
            NotifyPropertyChanging("isNotified");
            seenDate = DateTime.Now;
            NotifyPropertyChanged("isNotified");
        }
    }
}
