using ClarolineApp.Settings;
using Microsoft.Phone.Tasks;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.IO;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{

    public class CL_Document : ResourceModel
    {
        // Assign handlers for the add and remove operations, respectively.

        public CL_Document()
            : base()
        {
            _Date = new DateTime(DateTime.Today.Year, 9, 20);
            _desc = string.Empty;
            _ext = string.Empty;
            _isFolder = false;
            _size = 0.0;
            _url = string.Empty;
            _path = string.Empty;
            DiscKey = SupportedModules.CLDOC;
        }

        public new const string LABEL = "CLDOC";

        private string _path;

        [Column(CanBeNull = true)]
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

        public bool isNotified
        {
            get
            {
                if (isFolder)
                {
                    return getContent().Any(f => f.isNotified);
                }
                else
                {
                    return base.isNotified;
                }
            }
        }
       
        private string _url;

        [Column(CanBeNull = true)]
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

        [Column(CanBeNull = true)]
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

        [Column(CanBeNull = true)]
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

        [Column(CanBeNull = true)]
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

        [Column(CanBeNull = true)]
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
            if (obj != null && obj.GetType().Equals(typeof(CL_Document)))
            {
                CL_Document fld = obj as CL_Document;
                return (fld._resourceListId == this._resourceListId) && (fld.path == this.path);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public CL_Document getRoot()
        {
            string Find = (_isFolder) ? ("/" + _Title) : ("/" + _Title + "." + _ext);
            string rootPath = _path.Remove(_path.LastIndexOf(Find, StringComparison.OrdinalIgnoreCase), Find.Length);
            if (rootPath == "")
            {
                return GetRootDocument();
            }
            else
            {
                string DBConnectionString = "Data Source=isostore:/Claroline.sdf";
                ClarolineDataContext db = new ClarolineDataContext(DBConnectionString);

                return (from CL_Document _doc
                        in db.Documents_Table
                        where _doc.path == rootPath && _doc.resourceList.Cours.sysCode == this._resourceList.Entity.Cours.sysCode
                        select _doc).First();
            }
        }

        public ObservableCollection<CL_Document> getContent()
        {
            if (isFolder)
            {
                string DBConnectionString = "Data Source=isostore:/Claroline.sdf";
                ClarolineDataContext db = new ClarolineDataContext(DBConnectionString);

                return new ObservableCollection<CL_Document>((from CL_Document d
                                                              in db.Documents_Table
                                                              where d.path == ((d.isFolder)
                                                                                    ? (this._path + "/" + d.title)
                                                                                    : (this._path + "/" + d.title + "." + d.extension)
                                                                                    )
                                                              && d.resourceList.Cours.Equals(this.resourceList.Cours)
                                                              select d).ToList<CL_Document>()
                                                              );
            }
            else
            {
                return new ObservableCollection<CL_Document>();
            }
        }

        public CL_Document GetRootDocument()
        {
            return GetRootDocument(this.resourceList.Cours);
        }

        public static CL_Document GetRootDocument(Cours cours)
        {
            ResourceList list = cours.Resources.FirstOrDefault(rl => rl.ressourceType.Equals(typeof(CL_Document)));
            return new CL_Document()
            {
                _isFolder = true,
                _path = "",
                resourceList = list == null ? new ResourceList() { ressourceType = typeof(CL_Document), Cours = cours } : list
            };
        }

        public override string getNotificationText()
        {
            return _Title;
        }

        public async void OpenDocumentAsync()
        {
            if (!isFolder)
            {
                String token = await ClaroClient.instance.makeOperationAsync(SupportedModules.CLDOC, SupportedMethods.getSingleResource, resourceList.Cours, path);

                using (JsonTextReader reader = new JsonTextReader(new StringReader(token)))
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == "token")
                        { 
                            reader.Read();
                            token = (string) reader.Value;
                            break;
                        }
                    }
                }

                WebBrowserTask open = new WebBrowserTask()
                {
                    Uri = new Uri(Uri.EscapeUriString(AppSettings.instance.DomainSetting + AppSettings.instance.WebServiceSetting + "download.php?token=" + token), UriKind.Absolute)
                };
                open.Show();
            }
            else
            {
                return;
            }
        }

        public override bool IsResIdMatching(string resource)
        {
            return path == resource;
        }

        public override List<ResourceModel> GetSubRes()
        {
            return getContent().Cast<ResourceModel>().ToList();
        }
    }
}
