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
    public class CL_Notification : INotifyPropertyChanged, INotifyPropertyChanging
    {

        public static CL_Notification CreateNotification(ResourceModel resource, bool isOld)
        {
            CL_Notification note = new CL_Notification();
            note.isOldRessource = isOld;
            note.Cours = resource.resourceList.Cours;
            note.resource = resource;
            note.date = resource.date;
            note.ressourceType = resource.GetType();

            resource.PropertyChanged += note.resource_PropertyChanged;

            return note;
        }

        void resource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "isNotified":
                    if (resource.isNotified)
                    {
                        resource.PropertyChanged -= this.resource_PropertyChanged;
                        NotifyPropertyChanged("isNotified");
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
                    NotifyPropertyChanging("Id");
                    _Id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        public bool isNotified
        {
            get
            {
                return resource.seenDate.CompareTo(date) > 0;
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
                    NotifyPropertyChanging("date");
                    _date = value;
                    NotifyPropertyChanged("date");
                }
            }
        }

        private Type _ressourceType;

        [Column]
        public Type ressourceType
        {
            get
            {
                return _ressourceType;
            }
            set
            {
                if (_ressourceType != value)
                {
                    NotifyPropertyChanging("ressourceType");
                    _ressourceType = value;
                    NotifyPropertyChanged("ressourceType");
                }
            }
        }

        private bool _isOldRessource;

        [Column]
        public bool isOldRessource
        {
            get
            {
                return _isOldRessource;
            }
            set
            {
                if (_isOldRessource != value)
                {
                    NotifyPropertyChanging("isOldRessource");
                    _isOldRessource = value;
                    NotifyPropertyChanged("isOldRessource");
                }
            }
        }


        #region Entity Side for Cours

        [Column]
        internal int _coursId;

        // Entity reference, to identify the ToDoCategory "storage" table

        private EntityRef<Cours> _cours;

        // Association, to describe the relationship between this key and that "storage" table

        [Association(Storage = "_cours", ThisKey = "_coursId", OtherKey = "Id", IsForeignKey = true)]
        public Cours Cours
        {
            get { return _cours.Entity; }
            set
            {
                NotifyPropertyChanging("Cours");
                _cours.Entity = value;

                if (value != null)
                {
                    _coursId = value.Id;
                }

                NotifyPropertyChanging("Cours");
            }
        }

        #endregion

        #region Entity Side for Resource

        [Column]
        internal int _resourceId;

        // Entity reference, to identify the ToDoCategory "storage" table

        private EntityRef<ResourceModel> _resource;

        // Association, to describe the relationship between this key and that "storage" table

        [Association(Storage = "_resource", ThisKey = "_resourceId", OtherKey = "Id", IsForeignKey = true)]
        public ResourceModel resource
        {
            get { return _resource.Entity; }
            set
            {
                NotifyPropertyChanging("resource");
                _resource.Entity = value;

                if (value != null)
                {
                    _resourceId = value.Id;
                }

                NotifyPropertyChanged("resource");
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
                    NotifyPropertyChanging("Updated");
                    _updated = value;
                    NotifyPropertyChanged("Updated");
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
            if (obj != null && obj.GetType().Equals(typeof(CL_Notification)))
            {
                CL_Notification notif = obj as CL_Notification;
                return this.resource.Equals(notif.resource);
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
    }
}
