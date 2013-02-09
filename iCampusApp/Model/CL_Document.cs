using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{
    [Table]
    public class CL_Document : ResourceModel
    {
        // Assign handlers for the add and remove operations, respectively.

        public CL_Document() : base()
        {
            _Date = new DateTime(DateTime.Today.Year, 9, 20);
            _desc = string.Empty;
            _ext = string.Empty;
            _isFolder = false;
            _size = 0.0;
            _url = string.Empty;
            _path = string.Empty;
        }

        // Define item name: private Notifications, public property and database column.

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
                    NotifyPropertyChanging("path");
                    _path = value;
                    NotifyPropertyChanged("path");
                }
            }
        }

        // Define url value: private Notifications, public property and database column.

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
        public string description
        {
            get
            {
                return _desc;
            }
            set
            {
                if (_desc != value)
                {
                    NotifyPropertyChanging("description");
                    _desc = value;
                    NotifyPropertyChanged("description");
                }
            }
        }

        private string _ext;

        [Column]
        public string extension
        {
            get
            {
                return _ext;
            }
            set
            {
                if (_ext != value)
                {
                    NotifyPropertyChanging("extension");
                    _ext = value;
                    NotifyPropertyChanged("extension");
                }
            }
        }

        private bool _isFolder;

        [Column]
        public bool isFolder
        {
            get
            {
                return _isFolder;
            }
            set
            {
                if (_isFolder != value)
                {
                    NotifyPropertyChanging("isFolder");
                    _isFolder = value;
                    NotifyPropertyChanged("isFolder");
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

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType() == typeof(CL_Document))
            {
                CL_Document fld = obj as CL_Document;
                return (fld._resourceListId == this._resourceListId) && (fld.path == this.path);
            }
            return false;
        }

        public CL_Document getRoot()
        {
            string Find = (_isFolder) ? ("/" + _Title) : ("/" + _Title + "." + _ext);
            string rootPath = _path.Remove(_path.LastIndexOf(Find, StringComparison.OrdinalIgnoreCase), Find.Length);
            if (rootPath == "")
            {
                return getRootDocument();
            }
            else
            {
                return (from CL_Document _doc
                        in App.ViewModel.AllFolders
                        where _doc._path == rootPath && _doc._resourceList.Entity.Cours.sysCode == this._resourceList.Entity.Cours.sysCode
                        select _doc).First();
            }
        }

        public ObservableCollection<CL_Document> getContent()
        {
            return new ObservableCollection<CL_Document>((from CL_Document _doc
                        in App.ViewModel.DocByCours[this._cours.Entity.sysCode]
                                                          where _doc._path == ((_doc._isFolder) ? (this._path + "/" + _doc._name) : (this._path + "/" + _doc._name + "." + _doc._ext))
                                                          select _doc).ToList<CL_Document>());
        }

        public CL_Document getRootDocument()
        {
            return new CL_Document()
            {
                _isFolder = true,
                _path = "",
                resourceList = this._resourceList.Entity
            };
        }

        public override string getNotificationText()
        {
            return _Title;
        }
    }
}
