using ClarolineApp.Common;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
#if WINDOWS_PHONE
using Microsoft.Phone.Data.Linq.Mapping;
using System.Data.Linq.Mapping;
using System.Data.Linq; 
#endif

namespace ClarolineApp.Model
{

#if WINDOWS_PHONE
    [Table]
    [Index(Name = "i_Label", IsUnique = false, Columns = "label")] 
#endif
    public class ResourceList : ModelBase
    {

        public ResourceList()
        {
#if WINDOWS_PHONE
            _resources = new EntitySet<ResourceModel>(
                    new Action<ResourceModel>(this.attach_Resource),
                    new Action<ResourceModel>(this.detach_Resource)
                    ); 
#endif
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
                    RaisePropertyChanging("Id");
                    _Id = value;
                    RaisePropertyChanged("Id");
                }
            }

        }

        private string _Label;

#if WINDOWS_PHONE
        [Column] 
#endif
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

#if WINDOWS_PHONE
        [Column] 
#endif
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

        protected bool _Visibility;

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

        public bool ListVisibility
        {
            get
            {
                return visibility && Resources.Count > 0;
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

        private string _ressourceType;

#if WINDOWS_PHONE
        [Column] 
#endif
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


#if WINDOWS_PHONE
        #region Collection Side for ResourceModel

        // Define the entity set for the collection side of the relationship.

        private EntitySet<ResourceModel> _resources;

        [Association(Storage = "_resources", OtherKey = "_resourceListId", ThisKey = "Id", DeleteRule = "Cascade")]
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

        #endregion 
#else
        private ObservableCollection<ResourceModel> _resources;

        public ObservableCollection<ResourceModel> Resources
        {
            get
            {
                return _resources;
            }
            set
            {
                if (value != _resources)
                {
                    _resources = value;
                    RaisePropertyChanged("Resources");
                }
            }
        }
#endif

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


#if WINDOWS_PHONE
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
#else
        internal int _coursId;

        private Cours _cours;

        public Cours Cours
        {
            get { return _cours; }
            set
            {
                RaisePropertyChanging("Cours");

                if (value != null)
                {
                    Cours previousValue = this._cours;
                    if (previousValue != value)
                    {
                        if (previousValue != null)
                        {
                            this._cours = null;
                            previousValue.Resources.Remove(this);
                        }
                        this._cours = value;

                        value.Resources.Add(this);
                        this._coursId = value.Id;
                    }
                }

                RaisePropertyChanged("Cours");
            }
        }
#endif

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

#if WINDOWS_PHONE

        [Column(IsVersion = true)]
        private Binary _version; 
#endif

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
