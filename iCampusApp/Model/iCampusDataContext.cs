using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Collections.ObjectModel;
using System.Collections.Generic;

namespace ClarolineApp.Model
{
    public class iCampusDataContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public static string DBConnectionString = "Data Source=isostore:/iCampus.sdf";

        // Pass the connection string to the base class.
        public iCampusDataContext(string connectionString)
            : base(connectionString)
        {
        }

        public Table<Cours> Cours_T;
        public Table<Documents> Documents;
        public Table<Annonce> Annonces;
    }

    [Table]
    public class Cours : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property and database column.

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

        // Define item name: private field, public property and database column.

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

        // Define Responsable value: private field, public property and database column.

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

        // Define url value: private field, public property and database column.

        // Define status value: private field, public property and database column.

        private bool _notified;

        [Column]
        public bool notified
        {
            get
            {
                return _notified;
            }
            set
            {
                if (_notified != value)
                {
                    NotifyPropertyChanging("notified");
                    _notified = value;
                    NotifyPropertyChanged("notified");
                }
            }
        }

        // Define Tag value: private field, public property and database column.

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

        #region Presence of Section

        // Define item presence: private field, public property and database column.

        private bool _isDnL;

        [Column]
        public bool isDnL
        {
            get
            {
                return _isDnL;
            }
            set
            {
                if (_isDnL != value)
                {
                    NotifyPropertyChanging("isDnL");
                    _isDnL = value;
                    NotifyPropertyChanged("isDnL");
                }
            }
        }

        private bool _DnLNotif;

        [Column]
        public bool dnlNotif
        {
            get
            {
                return _DnLNotif;
            }
            set
            {
                if (_DnLNotif != value)
                {
                    NotifyPropertyChanging("dnlNotif");
                    _DnLNotif = value;
                    NotifyPropertyChanged("dnlNotif");
                }
            }
        }

        // Define item presence: private field, public property and database column.

        private bool _isAnn;

        [Column]
        public bool isAnn
        {
            get
            {
                return _isAnn;
            }
            set
            {
                if (_isAnn != value)
                {
                    NotifyPropertyChanging("isAnn");
                    _isAnn = value;
                    NotifyPropertyChanged("isAnn");
                }
            }
        }

        private bool _AnnNotif;

        [Column]
        public bool annNotif
        {
            get
            {
                return _AnnNotif;
            }
            set
            {
                if (_AnnNotif != value)
                {
                    NotifyPropertyChanging("annNotif");
                    _AnnNotif = value;
                    NotifyPropertyChanged("annNotif");
                }
            }
        }

        #endregion

        
        #region Collections
        
        #region Collection Side for DOCS - COURS

        // Define the entity set for the collection side of the relationship.

        private EntitySet<Documents> _DnLDocs;

        [Association(Storage = "_DnLDocs", OtherKey = "_coursId", ThisKey = "Id")]
        public EntitySet<Documents> Documents
        {
            get { return this._DnLDocs; }
            set { this._DnLDocs.Assign(value); }
        }

        // Called during an add operation

        private void attach_Doc(Documents _doc)
        {
            NotifyPropertyChanging("Documents");
            _doc.Cours = this;
        }

        // Called during a remove operation

        private void detach_Doc(Documents _doc)
        {
            NotifyPropertyChanging("Documents");
            _doc.Cours = null;
        }

        #endregion

        #region Collection Side for ANN - COURS

        // Define the entity set for the collection side of the relationship.

        private EntitySet<Annonce> _Ann;

        [Association(Storage = "_Ann", OtherKey = "_coursId", ThisKey = "Id")]
        public EntitySet<Annonce> Annonces
        {
            get { return this._Ann; }
            set { this._Ann.Assign(value); }
        }

        // Called during an add operation

        private void attach_Ann(Annonce _ann)
        {
            NotifyPropertyChanging("Annonces");
            _ann.Cours = this;
        }

        // Called during a remove operation

        private void detach_Ann(Annonce _ann)
        {
            NotifyPropertyChanging("Annonces");
            _ann.Cours = null;
        }

        #endregion

        // Assign handlers for the add and remove operations, respectively.

        public Cours()
        {
            _DnLDocs = new EntitySet<Documents>(
                new Action<Documents>(this.attach_Doc),
                new Action<Documents>(this.detach_Doc)
                );
            _Ann = new EntitySet<Annonce>(
                new Action<Annonce>(this.attach_Ann),
                new Action<Annonce>(this.detach_Ann)
                );
        }

        #endregion

        // Version column aids update performance.

        [Column(IsVersion = true)]
        private Binary _version;

        // Define updated value: private field, public property and database column.

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
            if (obj != null && obj.GetType() == typeof(Cours))
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

        private bool _isLoaded = false;

        [Column]
        public bool isLoaded
        {
            get
            {
                return _isLoaded;
            }
            set
            {
                if (value != _isLoaded)
                {
                    NotifyPropertyChanging("isLoaded");
                    _isLoaded = value;
                    NotifyPropertyChanged("isLoaded");
                }
            }
        }
    }

    [Table]
    public class Documents : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Assign handlers for the add and remove operations, respectively.

        public Documents()
        {
            _date = new DateTime(DateTime.Today.Year, 9, 20);
            _desc = string.Empty;
            _ext = string.Empty;
            _isFolder = false;
            _size = 0.0;
            _notified = false;
            _url = string.Empty;
            _path = string.Empty;
        }

        // Define ID: private field, public property and database column.

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

        // Define item name: private field, public property and database column.

        private string _path;

        [Column]
        public string path
        {
            get
            {
                return _path;
            }
            set
            {
                if (_path != value)
                {
                    NotifyPropertyChanging("Path");
                    _path = value;
                    NotifyPropertyChanged("Path");
                }
            }
        }

        private string _name;

        [Column]
        public string name
        {
            get
            {
                return _name;
            }
            set
            {
                if (value != _name)
                {
                    NotifyPropertyChanging("Name");
                    _name = value;
                    NotifyPropertyChanged("Name");
                }
            }
        }

        // Define url value: private field, public property and database column.

        private string _url;

        [Column]
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
                    NotifyPropertyChanging("url");
                    _url = value;
                    NotifyPropertyChanged("url");
                }
            }
        }

        private string _desc;

        [Column]
        public string Description
        {
            get
            {
                return _desc;
            }
            set
            {
                if (_desc != value)
                {
                    NotifyPropertyChanging("Description");
                    _desc = value;
                    NotifyPropertyChanged("Description");
                }
            }
        }
        
        private string _ext;

        [Column]
        public string Extension
        {
            get
            {
                return _ext;
            }
            set
            {
                if (_ext != value)
                {
                    NotifyPropertyChanging("Extension");
                    _ext = value;
                    NotifyPropertyChanged("Extension");
                }
            }
        }
        
        private bool _isFolder;

        [Column]
        public bool IsFolder
        {
            get
            {
                return _isFolder;
            }
            set
            {
                if (_isFolder != value)
                {
                    NotifyPropertyChanging("IsFolder");
                    _isFolder = value;
                    NotifyPropertyChanged("IsFolder");
                }
            }
        }

        // Define status value: private field, public property and database column.

        private bool _notified;

        [Column]
        public bool notified
        {
            get
            {
                return _notified;
            }
            set
            {
                if (_notified != value)
                {
                    NotifyPropertyChanging("notified");
                    _notified = value;
                    NotifyPropertyChanged("notified");
                }
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

        private double _size;

        [Column]
        public double size
        {
            get
            {
                return _size;
            }
            set
            {
                if (_size != value)
                {
                    NotifyPropertyChanging("size");
                    _size = value;
                    NotifyPropertyChanged("size");
                }
            }
        }

        private bool _visibility;

        [Column]
        public bool visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                if (_visibility != value)
                {
                    NotifyPropertyChanging("visibility");
                    _visibility = value;
                    NotifyPropertyChanged("visibility");
                }
            }
        }

        #region Entity Side for DOCS - COURS

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

        // Version column aids update performance.

        [Column(IsVersion = true)]
        private Binary _version;

        // Define updated value: private field, public property and database column.

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
            if (obj != null && obj.GetType() == typeof(Documents))
            {
                Documents fld = obj as Documents;
                    return (fld._url == this._url) && (fld.Cours == this.Cours) && (fld.path == this.path);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Documents getRoot()
        {
            string Find = (_isFolder) ? ("/" + _name) : ("/" + _name + "." + _ext);
            string rootPath = _path.Remove(_path.LastIndexOf(Find), Find.Length);
            if (rootPath == "")
            {
                return new Documents()
                {
                    Cours = this._cours.Entity,
                    name = null,
                    IsFolder = true
                };
            }
            else
            {
                return (from Documents _doc
                        in App.ViewModel.AllFolders
                        where _doc._path == rootPath && _doc._cours.Entity.sysCode == this._cours.Entity.sysCode
                        select _doc).First();
            }
        }

        public ObservableCollection<Documents> getContent()
        {
            return new ObservableCollection<Documents>((from Documents _doc
                        in App.ViewModel.DocByCours[this._cours.Entity.sysCode]
                        where _doc._path == ((_doc._isFolder)?(this._path + "/" + _doc._name):(this._path + "/" + _doc._name + "." + _doc._ext))
                        select _doc).ToList<Documents>());
        }
    }

    [Table]
    public class Annonce : INotifyPropertyChanged, INotifyPropertyChanging
    {
        // Define ID: private field, public property and database column.

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

        // Define item name: private field, public property and database column.

        private string _Title;

        [Column]
        public string title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (_Title != value)
                {
                    NotifyPropertyChanging("Titre");
                    _Title = value;
                    NotifyPropertyChanged("Titre");
                }
            }
        }

        private string _Text;

        [Column]
        public string content
        {
            get
            {
                return _Text;
            }
            set
            {
                if (_Text != value)
                {
                    NotifyPropertyChanging("Texte");
                    _Text = value;
                    NotifyPropertyChanged("Texte");
                }
            }
        }

        // Define status value: private field, public property and database column.

        private bool _notified;

        [Column]
        public bool notified
        {
            get
            {
                return _notified;
            }
            set
            {
                if (_notified != value)
                {
                    NotifyPropertyChanging("notified");
                    _notified = value;
                    NotifyPropertyChanged("notified");
                }
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

        #region Entity Side for ANN - COURS

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

        // Version column aids update performance.

        [Column(IsVersion = true)]
        private Binary _version;

        // Define updated value: private field, public property and database column.

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

        private bool _visibility;

        [Column]
        public bool visibility
        {
            get
            {
                return _visibility;
            }
            set
            {
                if (_visibility != value)
                {
                    NotifyPropertyChanging("visibility");
                    _visibility = value;
                    NotifyPropertyChanged("visibility");
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
            if (obj != null && obj.GetType() == typeof(Annonce))
            {
                Annonce ann = obj as Annonce;
                return this._Title == ann._Title && this._Text == ann._Text;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}

