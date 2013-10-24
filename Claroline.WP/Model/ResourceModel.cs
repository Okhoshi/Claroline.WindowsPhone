using ClarolineApp.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
#if WINDOWS_PHONE
using System.Data.Linq;
using System.Data.Linq.Mapping; 
#endif

namespace ClarolineApp.Model
{
#if WINDOWS_PHONE
    [Table]
    [InheritanceMapping(Code = SupportedModules.NOMOD, Type = typeof(ResourceModel), IsDefault = true)]
    [InheritanceMapping(Code = SupportedModules.CLANN, Type = typeof(Annonce))]
    [InheritanceMapping(Code = SupportedModules.CLDSC, Type = typeof(Description))]
    [InheritanceMapping(Code = SupportedModules.CLDOC, Type = typeof(Document))]
    [InheritanceMapping(Code = SupportedModules.CLCAL, Type = typeof(Event))]
    [InheritanceMapping(Code = SupportedModules.CLFRM, Type = typeof(Forum))]
#endif
    public class ResourceModel : ModelBase
    {

        public ResourceModel()
        {
            DiscKey = SupportedModules.NOMOD;
#if WINDOWS_PHONE
            _notifications = new EntitySet<Notification>(
                    new Action<Notification>(attach_Notification),
                    new Action<Notification>(detach_Notification)
                    );
#else
            _notifications.CollectionChanged += (s, e) =>
            {
                foreach (Notification item in e.NewItems)
                {
                    item.resource = this;
                }
                foreach (Notification item in e.OldItems)
                {
                    item.resource = null;
                }
            };
#endif
        }

        public const string Label = "NOMOD";

        protected int _Id;

        // Define ID: internal Notifications, public property and database column.

#if WINDOWS_PHONE
		        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)]
#endif
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
                    RaisePropertyChanging("Id");
                    _Id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

#if WINDOWS_PHONE
		        [Column(IsDiscriminator = true)]
 #endif        
        public SupportedModules DiscKey;

        protected string _Title;

#if WINDOWS_PHONE
        [Column] 
#endif
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
                    RaisePropertyChanging("title");
                    _Title = value;
                    RaisePropertyChanged("title");
                }
            }
        }

        protected string _resourceId;

#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
        public string resourceId
        {
            get
            {
                return _resourceId;
            }
            set
            {
                if (_resourceId != value)
                {
                    RaisePropertyChanging("resourceId");
                    _resourceId = value;
                    RaisePropertyChanged("resourceId");
                }
            }
        }

        protected DateTime _NotifiedDate = DateTime.Parse("01/01/1753");

#if WINDOWS_PHONE
        [Column] 
#endif
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
                    RaisePropertyChanging("notifiedDate");
                    _NotifiedDate = value;
                    RaisePropertyChanged("notifiedDate");
                    RaisePropertyChanged("isNotified");
                }
            }
        }

        public bool isNotified
        {
            get
            {
                return seenDate.CompareTo(notifiedDate) < 0;
            }
        }

        protected DateTime _SeenDate = DateTime.Parse("01/01/1753");

#if WINDOWS_PHONE
        [Column] 
#endif
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
                    RaisePropertyChanging("seenDate");
                    _SeenDate = value;
                    RaisePropertyChanged("seenDate");
                    RaisePropertyChanged("isNotified");
                }
            }
        }

        protected DateTime _Date = DateTime.Parse("01/01/1753");

#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
                    RaisePropertyChanging("date");
                    _Date = value;
                    RaisePropertyChanged("date");
                }
            }
        }

        protected bool _Visibility = true;

#if WINDOWS_PHONE
        [Column] 
#endif
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
                    RaisePropertyChanging("visibility");
                    _Visibility = value;
                    RaisePropertyChanged("visibility");
                }
            }
        }

        protected string _url;

#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
        public string url
        {
            get
            {
                return _url;
            }
            set
            {
                if (_url != value)
                {
                    RaisePropertyChanging("url");
                    _url = value;
                    RaisePropertyChanged("url");
                }
            }
        }

        protected bool _Updated;

#if WINDOWS_PHONE
        [Column] 
#endif
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
                    RaisePropertyChanging("updated");
                    _Updated = value;
                    RaisePropertyChanged("updated");
                }
            }
        }

        protected DateTime _Loaded = DateTime.Parse("01/01/1753");

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
                    RaisePropertyChanging("loaded");
                    _Loaded = value;
                    RaisePropertyChanged("loaded");
                }
            }
        }

#if WINDOWS_PHONE
        #region Collection Side for Notification

        // Define the entity set for the collection side of the relationship.

        private EntitySet<Notification> _notifications;

        [Association(Name = "Resource2Notification", Storage = "_notifications", OtherKey = "_resourceId", ThisKey = "Id", DeleteRule = "Cascade")]
        public EntitySet<Notification> notifications
        {
            get { return this._notifications; }
            set { this._notifications.Assign(value); }
        }

        // Called during an add operation

        private void attach_Notification(Notification _notification)
        {
            RaisePropertyChanging("notifications");
            _notification.resource = this;
        }

        // Called during a remove operation

        private void detach_Notification(Notification _notification)
        {
            RaisePropertyChanging("notifications");
            _notification.resource = null;
        }

        #endregion 
#else
        private ObservableCollection<Notification> _notifications;

        public ObservableCollection<Notification> notifications
        {
            get
            {
                return _notifications;
            }
            set
            {
                if (value != _notifications)
                {
                    value = _notifications;
                    RaisePropertyChanged("notifications");
                }
            }
        }
#endif

#if WINDOWS_PHONE
        #region Entity Side for ResourceList

        [Column]
        protected int _resourceListId;
		
        protected EntityRef<ResourceList> _resourceList;

        // Association, to describe the relationship between this key and that "storage" table

        [Association(Storage = "_resourceList", ThisKey = "_resourceListId", OtherKey = "Id", IsForeignKey = true)]
        public ResourceList ResourceList
        {
            get { return _resourceList.Entity; }
            set
            {
                RaisePropertyChanging("ResourceList");

                if (value != null)
                {
                    ResourceList previousValue = this._resourceList.Entity;
                    if (((previousValue != value) || (this._resourceList.HasLoadedOrAssignedValue == false)))
                    {
                        if ((previousValue != null))
                        {
                            this._resourceList.Entity = null;
                            previousValue.Resources.Remove(this);
                        }
                        this._resourceList.Entity = value;

                        value.Resources.Add(this);
                        this._resourceListId = value.Id;
                    }
                }

                RaisePropertyChanged("ResourceList");
            }
        }

        #endregion  
#else
        protected int _resourceListId;
        
        protected ResourceList _resourceList;

        public ResourceList ResourceList
        {
            get { return _resourceList; }
            set
            {
                RaisePropertyChanging("ResourceList");

                if (value != null)
                {
                    ResourceList previousValue = this._resourceList;
                    if (previousValue != value)
                    {
                        if (previousValue != null)
                        {
                            this._resourceList = null;
                            previousValue.Resources.Remove(this);
                        }
                        this._resourceList = value;

                        value.Resources.Add(this);
                        this._resourceListId = value.Id;
                    }
                }

                RaisePropertyChanged("ResourceList");
            }
        }
#endif

#if WINDOWS_PHONE
        // Version column aids update performance.
        private Binary v;

        [Column(IsVersion = true)]
        private Binary _version
        {
            get
            {
                return v;
            }
            set
            {
                v = value;
            }
        } 
#endif

        public override int GetHashCode()
        {
            return title.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj is ResourceModel)
            {
                ResourceModel res = obj as ResourceModel;
                return (res._resourceListId == this._resourceListId) && (res._Title == this._Title);
            }
            return false;
        }

        public virtual string getNotificationText() { return title; }

        public void markAsRead()
        {
            RaisePropertyChanging("isNotified");
            seenDate = DateTime.Now;
            RaisePropertyChanged("isNotified");
        }

        public virtual bool IsResIdMatching(string resource)
        {
            return resource != null && resource.Equals(resourceId);
        }

        public virtual List<ResourceModel> GetSubRes()
        {
            return new List<ResourceModel>();
        }

        public virtual string GetResourceString()
        {
            return resourceId.ToString();
        }

        public virtual void UpdateFrom(ResourceModel newRes)
        {
            title = newRes.title;
            visibility = newRes.visibility;
            url = newRes.url;
            seenDate = newRes.seenDate;
            date = newRes.date;
            notifiedDate = newRes.notifiedDate;
        }
    }
}
