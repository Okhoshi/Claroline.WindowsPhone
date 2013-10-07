using ClarolineApp.Common;
using Microsoft.Phone.Data.Linq.Mapping;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;

namespace ClarolineApp.Model
{
    [Table]
    [Index(Name="i_Label", IsUnique = false, Columns = "label")]
    public class ResourceList : ModelBase
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
                if (Resources.Count == 0)
                {
                    RaisePropertyChanged("ListVisibility");
                }
            }
            if (e.NewItems != null)
            {
                foreach (ResourceModel item in e.NewItems)
                {
                    if (item.isNotified)
                    {
                        RaisePropertyChanged("isNotified");
                    }
                    item.PropertyChanged += resource_PropertyChanged;
                }
                if (Resources.Count == 1)
                {
                    RaisePropertyChanged("ListVisibility");
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
                    RaisePropertyChanging("Id");
                    _Id = value;
                    RaisePropertyChanged("Id");
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
                    RaisePropertyChanging("label");
                    _Label = value;
                    RaisePropertyChanged("label");
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
                    RaisePropertyChanging("name");
                    _Name = value;
                    RaisePropertyChanged("name");
                }
            }
        }

        public bool isNotified
        {
            get
            {
                return Resources.Any(r => {
                    return r.isNotified;
                    });
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
                    RaisePropertyChanging("loaded");
                    _Loaded = value;
                    RaisePropertyChanged("loaded");
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
                    RaisePropertyChanging("visibility");
                    _Visibility = value;
                    RaisePropertyChanged("visibility");
                }
            }
        }

        public bool ListVisibility
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
                    RaisePropertyChanging("updated");
                    _Updated = value;
                    RaisePropertyChanged("updated");
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
                    RaisePropertyChanging("ressourceType");
                    _ressourceType = value;
                    RaisePropertyChanged("ressourceType");
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

        [Association(Storage = "_resources", OtherKey = "_resourceListId", ThisKey = "Id", DeleteRule="Cascade")]
        public EntitySet<ResourceModel> Resources
        {
            get { return this._resources; }
            set { this._resources.Assign(value); }
        }

        // Called during an add operation

        private void attach_Resource(ResourceModel _resource)
        {
            RaisePropertyChanging("Resources");
            _resource.ResourceList = this;
            //_resource.PropertyChanged += resource_PropertyChanged;
            RaisePropertyChanged("Resources");
        }

        // Called during a remove operation

        private void detach_Resource(ResourceModel _resource)
        {
            RaisePropertyChanging("Resources");
            _resource.ResourceList = null;
            //_resource.PropertyChanged -= resource_PropertyChanged;
            RaisePropertyChanged("Resources");
        }

        public void resource_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "seenDate":
                case "isNotified":
                    RaisePropertyChanged("isNotified");
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
                RaisePropertyChanging("Cours");

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

                RaisePropertyChanged("Cours");
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

        [Column(IsVersion=true)]
        private Binary _version;

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

        internal void UpdateFrom(ResourceList newList)
        {
            name = newList.name;
            visibility = newList.visibility;
            updated = newList.updated;
            loaded = newList.loaded;
        }
    }
}
