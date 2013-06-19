using ClarolineApp.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;

namespace ClarolineApp.Model
{

    [Table]
    public class Notification : ModelBase
    {

        public static Notification CreateNotification(bool isOld)
        {
            Notification note = new Notification();
            note.isOldResource = isOld;

            return note;
        }

        void resource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "isNotified":
                case "seenDate":
                    if (resource.isNotified)
                    {
                        RaisePropertyChanged("isNotified");
                    }
                    break;
                default:
                    break;
            }
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
                    RaisePropertyChanging("Id");
                    _Id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        public bool isNotified
        {
            get
            {
                return _resource.Entity.seenDate.CompareTo(date) < 0;
            }
        }

        private DateTime _date;

        [Column]
        public DateTime date
        {
            get
            {
                return _date;
            }
            set
            {
                if (_date != value)
                {
                    RaisePropertyChanging("date");
                    _date = value;
                    RaisePropertyChanged("date");
                }
            }
        }

        private bool _isOldRessource;

        [Column]
        public bool isOldResource
        {
            get
            {
                return _isOldRessource;
            }
            set
            {
                if (_isOldRessource != value)
                {
                    RaisePropertyChanging("isOldResource");
                    _isOldRessource = value;
                    RaisePropertyChanged("isOldResource");
                }
            }
        }

        public Cours Cours
        {
            get
            {
                return resource.ResourceList.Cours;
            }
        }

        #region Entity Side for Resource

        [Column]
        internal int _resourceId;

        // Entity reference, to identify the ToDoCategory "storage" table

        private EntityRef<ResourceModel> _resource;

        // Association, to describe the relationship between this key and that "storage" table

        [Association(Name = "Resource2Notification", Storage = "_resource", ThisKey = "_resourceId", OtherKey = "Id", IsForeignKey = true)]
        public ResourceModel resource
        {
            get { return _resource.Entity; }
            set
            {
                RaisePropertyChanging("resource");
                if (resource != null)
                {
                    resource.PropertyChanged -= resource_PropertyChanged;
                }

                _resource.Entity = value;

                if (value != null)
                {
                    _resourceId = value.Id;
                    date = resource.notifiedDate;
                    resource.PropertyChanged += resource_PropertyChanged;
                }

                RaisePropertyChanged("resource");
            }
        }

        #endregion

        // Version column aids update performance.

        [Column(IsVersion = true)]
        private Binary _version;

        // Define updated value: private Notifications, public property and database column.

        private bool _updated;

        [Column]
        public bool Updated
        {
            get
            {
                return _updated;
            }
            set
            {
                if (_updated != value)
                {
                    RaisePropertyChanging("Updated");
                    _updated = value;
                    RaisePropertyChanged("Updated");
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(typeof(Notification)))
            {
                Notification notif = obj as Notification;
                return this._resourceId == notif._resourceId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public String Text
        {
            get
            {
                return resource.getNotificationText();
            }
        }

        public static List<Notification> ConvertFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<Notification>>(json);
        }
    }
}
