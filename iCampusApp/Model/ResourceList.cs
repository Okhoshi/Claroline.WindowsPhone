using ClarolineApp.Languages;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ClarolineApp.Model
{
    [Table]
    public class ResourceList : INotifyPropertyChanged, INotifyPropertyChanging
    {

        public ResourceList()
        {
            _resources = new EntitySet<ResourceModel>(
                new Action<ResourceModel>(this.attach_Resource),
                new Action<ResourceModel>(this.detach_Resource)
                );
            _resources.CollectionChanged += _resources_CollectionChanged;
        }

        void _resources_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.OldItems != null)
            {
                foreach (ResourceModel item in e.OldItems)
                {
                    item.PropertyChanged -= resource_PropertyChanged;
                }
            }
            if (e.NewItems != null)
            {
                foreach (ResourceModel item in e.NewItems)
                {
                    item.PropertyChanged += resource_PropertyChanged;
                }
            }
        }

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

        private string _Label;

        [Column]
        public string label
        {
            get
            {
                return _Label;
            }
            set
            {
                if (_Label != value)
                {
                    NotifyPropertyChanging("label");
                    _Label = value;
                    NotifyPropertyChanged("label");
                }
            }
        }

        private string _Name;

        [Column]
        public string name
        {
            get
            {
                return _Name;
            }
            set
            {
                if (_Name != value)
                {
                    NotifyPropertyChanging("name");
                    _Name = value;
                    NotifyPropertyChanged("name");
                }
            }
        }

        public bool isNotified
        {
            get
            {
                return Resources.Any(r => r.isNotified);
            }
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

        public bool listVisibility
        {
            get
            {
                return visibility && Resources.Count > 0;
            }
        }

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

        private string _ressourceType;

        [Column]
        public string ressourceTypeStr
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

        public Type ressourceType
        {
            get
            {
                return Type.GetType(_ressourceType);
            }
            set
            {
                if (_ressourceType != value.FullName)
                {
                    ressourceTypeStr = value.FullName;
                }
            }
        }

        public Type ressourceListType
        {
            get
            {
                return typeof(List<>).MakeGenericType(ressourceType);
            }
        }

        #region Collection Side for ResourceModel

        // Define the entity set for the collection side of the relationship.

        private EntitySet<ResourceModel> _resources;

        [Association(Storage = "_resources", OtherKey = "_resourceListId", ThisKey = "Id")]
        public EntitySet<ResourceModel> Resources
        {
            get { return this._resources; }
            set { this._resources.Assign(value); }
        }

        // Called during an add operation

        private void attach_Resource(ResourceModel _resource)
        {
            NotifyPropertyChanging("Resources");
            _resource.resourceList = this;
            //_resource.PropertyChanged += resource_PropertyChanged;
            NotifyPropertyChanged("Resources");
        }

        // Called during a remove operation

        private void detach_Resource(ResourceModel _resource)
        {
            NotifyPropertyChanging("Resources");
            _resource.resourceList = null;
            //_resource.PropertyChanged -= resource_PropertyChanged;
            NotifyPropertyChanged("Resources");
        }

        public void resource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "seenDate":
                case "isNotified":
                    NotifyPropertyChanged("isNotified");
                    break;
                default:
                    break;
            }
        }

        #endregion

        #region Entity Side for Cours

        [Column]
        internal int _coursId;

        private EntityRef<Cours> _cours;

        // Association, to describe the relationship between this key and that "storage" table

        [Association(Storage = "_cours", ThisKey = "_coursId", OtherKey = "Id", IsForeignKey = true)]
        public Cours Cours
        {
            get { return _cours.Entity; }
            set
            {
                NotifyPropertyChanging("Cours");

                if (value != null)
                {
                    Cours previousValue = this._cours.Entity;
                    if (((previousValue != value) || (this._cours.HasLoadedOrAssignedValue == false)))
                    {
                        if ((previousValue != null))
                        {
                            this._cours.Entity = null;
                            previousValue.Resources.Remove(this);
                        }
                        this._cours.Entity = value;

                        value.Resources.Add(this);
                        this._coursId = value.Id;
                    }
                }

                NotifyPropertyChanged("Cours");
            }
        }

        #endregion

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(typeof(ResourceList)))
            {
                ResourceList rl = obj as ResourceList;
                return (rl._coursId == this._coursId && rl.label.Equals(this.label));
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public SupportedModules GetSupportedModule()
        {
            SupportedModules module;
            try
            {
                module = (SupportedModules)Enum.Parse(typeof(SupportedModules), label, true);
            }
            catch (ArgumentException)
            {
                module = SupportedModules.GENERIC;
            }
            return module;
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

        internal ResourceModel GetResourceByResId(string resource)
        {
            return Resources.FirstOrDefault(r => r.IsResIdMatching(resource));
        }

        internal void ReloadPropertyChangedHandler()
        {
            foreach (ResourceModel item in Resources)
            {
                item.PropertyChanged += resource_PropertyChanged;
            }
        }
    }
}
