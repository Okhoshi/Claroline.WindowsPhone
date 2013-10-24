using ClarolineApp.Settings;
using ClarolineApp.VM;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

#if WINDOWS_PHONE
using System.Data.Linq.Mapping;
using Microsoft.Phone.Tasks; 
#endif

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

#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
                    RaisePropertyChanging("path");
                    _path = value;
                    RaisePropertyChanged("path");
                }
            }
        }

        public new bool isNotified
        {
            get
            {
                if (isFolder)
                {
                    return getContent().Any(f =>
                    {
                        return f.isNotified;
                    });
                }
                else
                {
                    return base.isNotified;
                }
            }
        }

        private string _desc;

#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
                    RaisePropertyChanging("description");
                    _desc = value;
                    RaisePropertyChanged("description");
                }
            }
        }

        private string _ext;

#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
                    RaisePropertyChanging("extension");
                    _ext = value;
                    RaisePropertyChanged("extension");
                }
            }
        }

        private bool _isFolder;

#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
                    RaisePropertyChanging("isFolder");
                    _isFolder = value;
                    RaisePropertyChanged("isFolder");
                }
            }
        }


        private double _size;

#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
                    RaisePropertyChanging("size");
                    _size = value;
                    RaisePropertyChanged("size");
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
            return path.GetHashCode();
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
                String token = await ViewModelLocator.Client.MakeOperationAsync(SupportedModules.CLDOC, SupportedMethods.GetSingleResource, ResourceList.Cours.sysCode, path);

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
                Uri TaskUri = new Uri(Uri.EscapeUriString(ViewModelLocator.Client.Settings.DomainSetting + ViewModelLocator.Client.Settings.WebServiceSetting + "download.php?token=" + token), UriKind.Absolute);

#if WINDOWS_PHONE
		WebBrowserTask open = new WebBrowserTask()
                {
                    Uri = TaskUri
                };
                open.Show();  
#endif
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

        public override string GetResourceString()
        {
            return path;
        }

        public override void UpdateFrom(ResourceModel newRes)
        {
            base.UpdateFrom(newRes);

            if (newRes is Document)
            {
                size = (newRes as Document).size;
                isFolder = (newRes as Document).isFolder;
                description = (newRes as Document).description;
                extension = (newRes as Document).extension;
            }
        }
    }
}
