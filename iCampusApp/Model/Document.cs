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

    public class Document : ResourceModel
    {
        // Assign handlers for the add and remove operations, respectively.

        public Document()
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

        public new const string Label = "CLDOC";

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
                    _resourceId = value.GetHashCode();
                    NotifyPropertyChanged("path");
                }
            }
        }

        public new bool isNotified
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
            if (obj != null && obj.GetType().Equals(typeof(Document)))
            {
                Document fld = obj as Document;
                return (fld._resourceListId == this._resourceListId) && (fld.path == this.path);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public Document getRoot()
        {
            string Find = (_isFolder) ? ("/" + _Title) : ("/" + _Title + "." + _ext);
            string rootPath = _path.Remove(_path.LastIndexOf(Find, StringComparison.OrdinalIgnoreCase), Find.Length);
            if (rootPath == "")
            {
                return GetRootDocument();
            }
            else
            {
                IEnumerable<Document> list = ResourceList.Resources.Cast<Document>();

                return list.First(d => d.path == rootPath);
            }
        }

        private Cours rootCours;

        public ObservableCollection<Document> getContent()
        {
            if (isFolder)
            {
                ResourceList rl = this.title == "/"
                                ? rootCours.Resources.FirstOrDefault(l => l.ressourceType == typeof(Document))
                                : ResourceList;

                if (rl == null)
                {
                    return new ObservableCollection<Document>();
                }

                IEnumerable<Document> list = rl.Resources.Cast<Document>();

                return new ObservableCollection<Document>(
                                list.Where(d => d.path == ((d.isFolder)
                                                         ? (this._path + "/" + d.title)
                                                         : (this._path + "/" + d.title + "." + d.extension)
                                                          )
                                           ));
            }
            else
            {
                return new ObservableCollection<Document>();
            }
        }

        public Document GetRootDocument()
        {
            return GetRootDocument(this.ResourceList.Cours);
        }

        public static Document GetRootDocument(Cours cours)
        {
            ResourceList list = cours.Resources.FirstOrDefault(rl => rl.ressourceType.Equals(typeof(Document)));
            return new Document()
            {
                path = "",
                isFolder = true,
                _resourceListId = list != null ? list.Id : -1,
                title = "/",
                rootCours = cours
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
                String token = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.CLDOC, SupportedMethods.GetSingleResource, ResourceList.Cours, path);

                using (JsonTextReader reader = new JsonTextReader(new StringReader(token)))
                {
                    while (reader.Read())
                    {
                        if (reader.TokenType == JsonToken.PropertyName && reader.Value as string == "token")
                        {
                            reader.Read();
                            token = (string)reader.Value;
                            break;
                        }
                    }
                }

                WebBrowserTask open = new WebBrowserTask()
                {
                    Uri = new Uri(Uri.EscapeUriString(AppSettings.Instance.DomainSetting + AppSettings.Instance.WebServiceSetting + "download.php?token=" + token), UriKind.Absolute)
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
