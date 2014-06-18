using ClarolineApp.Languages;
using ClarolineApp.Model;
using ClarolineApp.Settings;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace ClarolineApp.VM
{
    public class CoursPageVM : ClarolineVM, ICoursPageVM
    {
        private Cours _currentCours;

        public Cours currentCours
        {
            get
            {
                return _currentCours;
            }
            set
            {
                if (_currentCours != value)
                {
                    _currentCours = value;
                    LoadCollectionsFromDatabase();
                    RaisePropertyChanged("currentCours");
                }
            }
        }

        private Document _root;

        public Document root
        {
            get
            {
                return _root;
            }
            set
            {
                if (_root != value)
                {
                    _root = value;
                    RaisePropertyChanged("root");
                    RaisePropertyChanged("Content");
                }
            }
        }

        public ObservableCollection<Document> Content
        {
            get
            {
                if (IsInDesignMode)
                {
                    ObservableCollection<Document> ret = new ObservableCollection<Document>();
                    ret.Add(new Document() { title = "Hello Folder", description = "WTF", date = DateTime.Now, isFolder = false });
                    ret.Add(new Document() { title = "Document 1", size = 15653614, date = DateTime.Now, extension = "pdf", isFolder = false });
                    ret.Add(new Document() { title = "Document 2 Document 2 Document 2", size = 165, date = DateTime.Now, extension = "et2", isFolder = false });
                    ret.Add(new Document() { title = "Document 3", size = 184351861, date = DateTime.Now, extension = "ppt", isFolder = false });
                    return ret;
                }
                else
                {
                    return _root.getContent();
                }
            }
        }

        public ObservableCollection<Group<Event>> events
        {
            get
            {
                ObservableCollection<Group<Event>> _events = new ObservableCollection<Group<Event>>();

                if (currentCours != null)
                {
                    ResourceList rl = currentCours.Resources.FirstOrDefault(l => l.ressourceType == typeof(Event));
                    if (rl != null)
                    {
                        IEnumerable<Event> list = rl.Resources.Cast<Event>();

                        _events.Add(new Group<Event>(AppLanguage.Today, list.Where(e =>
                        {
                            return e.date.CompareTo(DateTime.Now) > 0
                            && e.date.Date.CompareTo(DateTime.Now.Date) == 0;
                        })));

                        _events.Add(new Group<Event>(AppLanguage.Tomorrow, list.Where(e =>
                        {
                            return e.date.Date.CompareTo(DateTime.Now.Date.AddDays(1.0)) == 0;
                        })));

                        _events.Add(new Group<Event>(AppLanguage.ThisWeek, list.Where(e =>
                        {
                            return e.date.Date.CompareTo(DateTime.Now.Date.AddDays(1.0)) >= 0
                            && e.date.Date.CompareTo(DateTime.Now.Date.AddDays(7.0)) < 0;
                        })));

                        _events.Add(new Group<Event>(AppLanguage.Later, list.Where(e =>
                        {
                            return e.date.Date.CompareTo(DateTime.Now.Date.AddDays(7.0)) >= 0;
                        })));

                        _events.Add(new Group<Event>(AppLanguage.Passed, list.Where(e =>
                        {
                            return e.date.CompareTo(DateTime.Now) < 0;
                        })));

                    }
                }
                return _events;
            }
        }

        public ObservableCollection<Group<Description>> descriptions
        {
            get
            {

                ObservableCollection<Group<Description>> _descriptions = new ObservableCollection<Group<Description>>();

                ResourceList rl = currentCours.Resources.FirstOrDefault(r => r.ressourceType == typeof(Description));
                if (rl != null)
                {
                    IEnumerable<Description> list = rl.Resources.Cast<Description>();
                    if (list.Count() == 0)
                    {
                        _descriptions = new ObservableCollection<Group<Description>>();
                    }
                    else
                    {
                        int otherCat = list.Max(d => d.category) + 1;

                        _descriptions = new ObservableCollection<Group<Description>>(
                                                list.GroupBy(d => d.category)
                                                .OrderBy(g => g.Key)
                                                .Select(g => new Group<Description>(g.First().title, g))
                                            );
                    }
                }

                return _descriptions;
            }
        }

        public ObservableCollection<Group<Forum>> forums
        {
            get
            {
                ObservableCollection<Group<Forum>> _forums = new ObservableCollection<Group<Forum>>();

                ResourceList rl = currentCours.Resources.FirstOrDefault(r => r.ressourceType == typeof(Forum));
                if (rl != null)
                {
                    IEnumerable<Forum> list = rl.Resources.Cast<Forum>();

                    if (list.Count() > 0)
                    {
                        _forums = new ObservableCollection<Group<Forum>>(
                                                list.OrderBy(f => f.Rank)
                                                .GroupBy(f => f.CategoryId)
                                                .OrderBy(g => g.First().CategoryRank)
                                                .Select(g => new Group<Forum>(g.First().CategoryName, g))
                                            );
                    }
                }

                return _forums;
            }
        }

        public ResourceList PivotMenu = new ResourceList() { label = "MENU", name = "Menu", ressourceType = typeof(Object), visibility = true };

        private ObservableCollection<ResourceList> _resources;
        public ObservableCollection<ResourceList> resources
        {
            get
            {
                if (_resources == null)
                {
                    _resources = new ObservableCollection<ResourceList>(currentCours.Resources.Where(l => l.ListVisibility));
                    _resources.Insert(0, PivotMenu);
                }
                return _resources;
            }
            set
            {
                if (_resources != value)
                {
                    _resources = value;
                    RaisePropertyChanged("resources");
                }
            }
        }

#if(DEBUG)
        public CoursPageVM()
            : this(null, "")
        {
        }
#endif

        public CoursPageVM(ISettings settings, string sysCode)
            : base(settings)
        {
            if (IsInDesignMode)
            {
                currentCours = new Cours()
                {
                    officialCode = "DESIGN",
                    sysCode = "DESIGN",
                    title = "Design test"
                };

                ResourceList l1 = new ResourceList() { Cours = currentCours, name = "Resource L1", label = "CLANN", ressourceType = typeof(Annonce) };
                ResourceList l2 = new ResourceList() { Cours = currentCours, name = "Resource L2", label = "CLDOC", ressourceType = typeof(Document) };
                ResourceList l3 = new ResourceList() { Cours = currentCours, name = "Resource L3", label = "CLDSC", ressourceType = typeof(Description) };
                ResourceList l4 = new ResourceList() { Cours = currentCours, name = "Resource L4", label = "CLCAL", ressourceType = typeof(Event) };
                ResourceList l5 = new ResourceList() { Cours = currentCours, name = "Resource L5", label = "CLFRM", ressourceType = typeof(Forum) };

                currentCours.Resources.Add(l5);
                _resources = new ObservableCollection<ResourceList>();
                _resources.Add(l5);
                _resources.Add(PivotMenu);
                currentCours.Resources.Add(new ResourceList() { Cours = currentCours, name = "L4", label = "L4" });

                l1.Resources.Add(new Annonce() { title = "Annonce 1", content = "Contenu 1 Contenu 1 v Contenu 1 Contenu 1 Contenu 1 v Contenu 1", date = DateTime.Now });
                l1.Resources.Add(new Annonce() { title = "Annonce 2 Annonce 2 Annonce 2", content = "Contenu 2", date = DateTime.Now });
                l1.Resources.Add(new Annonce() { title = "Annonce 3", content = "Contenu 3", date = DateTime.Now });
                l1.Resources.Add(new Annonce() { title = "Annonce 4", content = "Contenu 4", date = DateTime.Now });

                l3.Resources.Add(new Description() { title = "Annonce 1", content = "Contenu 1", date = DateTime.Now });

                l4.Resources.Add(new Event() { title = "Event 1", content = "Contenu 1", date = DateTime.Now, speakers = "Mr Nobody, Someone else" });
                l4.Resources.Add(new Event() { title = "Event 2", content = "Contenu 1", date = DateTime.Now, location = "This is a very very very very veyr vey long location" });
                l4.Resources.Add(new Event() { title = "Event 3", content = "Contenu 1", date = DateTime.Now, location = "B543", speakers = "Mr Nobody, Someone else" });

                l5.Resources.Add(new Forum() { title = "Forum 1", ForumDescription = "ForumDescription 1", CategoryId = 1, CategoryName = "Cat 1", CategoryRank = 1, Rank = 2 });
                l5.Resources.Add(new Forum() { title = "Forum 2", ForumDescription = "ForumDescription 2", CategoryId = 1, CategoryName = "Cat 1", CategoryRank = 1, Rank = 1 });
                l5.Resources.Add(new Forum() { title = "Forum 3", ForumDescription = "ForumDescription 3", CategoryId = 2, CategoryName = "Cat 2", CategoryRank = 2, Rank = 3 });
                l5.Resources.Add(new Forum() { title = "Forum 4", ForumDescription = "ForumDescription 4", CategoryId = 2, CategoryName = "Cat 2", CategoryRank = 2, Rank = 0 });
            }
            else
            {
                currentCours = (from Cours c
                                in ClarolineDB.Cours_Table
                                where c.sysCode.Equals(sysCode)
                                select c).FirstOrDefault();

                LoadCollectionsFromDatabase();

                if (!currentCours.loadedToday() || currentCours.Resources.Count == 0)
                {
                    PrepareCoursForOpeningAsync(currentCours);
                }
            }
        }

        public override void LoadCollectionsFromDatabase()
        {
            if (!IsInDesignMode)
            {
                base.LoadCollectionsFromDatabase();

                if (currentCours != null)
                {
                    root = Document.GetRootDocument(currentCours);
                    RaisePropertyChanged("Content");
                    RaisePropertyChanged("descriptions");
                    RaisePropertyChanged("forums");
                    RaisePropertyChanged("events");
                    resources = null;

                    currentCours.ReloadPropertyChangedHandler();
                }
            }
        }

        public void OnGenericItemSelected(ResourceModel item)
        {
            if (item != null)
            {
                item.seenDate = DateTime.Now;
                SaveChangesToDB();
            }
        }

        public void OnItemWithDetailsSelected(ResourceModel item)
        {
            NavigationTarget = new Uri(String.Format("/View/DetailPage.xaml?resource={0}&list={1}", item.resourceId, item.ResourceList.Id), UriKind.Relative);
        }

        public void OnDocumentItemSelected(Document item)
        {
            if (item != null)
            {
                item.seenDate = DateTime.Now;
                ClarolineDB.Log = new DebugStreamWriter();
                SaveChangesToDB();

                if (item.isFolder)
                {
                    root = item;
                }
                else
                {
                    item.OpenDocumentAsync();
                }
            }
        }

        public void GoUp()
        {
            root = root.getRoot();
        }

        public bool IsOnRoot()
        {
            return root.Equals(Document.GetRootDocument(currentCours));
        }

        public bool IsPivotSelectedOfType(object SelectedItem, Type type)
        {
            return (SelectedItem as ResourceList).ressourceType.Equals(type);
        }

        public bool IsModuleVisible(object module)
        {
            if (module != null && module is ResourceList)
            {
                ResourceList mod = module as ResourceList;
                return mod.visibility && mod.Resources.Count > 0;
            }
            return false;
        }

        public override void AddResourceList(ResourceList newList, int coursId)
        {
            base.AddResourceList(newList, coursId);

            newList.PropertyChanged += RL_PropertyChanged;
        }

        public override void DeleteResourceList(ResourceList listForDelete)
        {
            listForDelete.PropertyChanged -= RL_PropertyChanged;

            base.DeleteResourceList(listForDelete);
        }

        void RL_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ListVisibility":
                    if ((sender as ResourceList).ListVisibility)
                    {
                        resources.Add(sender as ResourceList);
                    }
                    else
                    {
                        resources.Remove(sender as ResourceList);
                    }
                    break;
                default:
                    break;
            }
        }

        public override void AddResource(ResourceModel newRes, int containerId)
        {
            base.AddResource(newRes, containerId);

            switch (newRes.DiscKey)
            {
                case SupportedModules.CLDOC:
                    RaisePropertyChanged("Content");
                    break;
                case SupportedModules.CLDSC:
                    RaisePropertyChanged("descriptions");
                    break;
                case SupportedModules.CLFRM:
                    RaisePropertyChanged("forums");
                    break;
                case SupportedModules.CLCAL:
                    RaisePropertyChanged("events");
                    break;
                default:
                    break;
            }
        }

        public override void DeleteResource(ResourceModel resForDelete)
        {
            switch (resForDelete.DiscKey)
            {
                case SupportedModules.CLDOC:
                    RaisePropertyChanged("Content");
                    break;
                case SupportedModules.CLDSC:
                    RaisePropertyChanged("descriptions");
                    break;
                case SupportedModules.CLFRM:
                    RaisePropertyChanged("forums");
                    break;
                case SupportedModules.CLCAL:
                    RaisePropertyChanged("events");
                    break;
                default:
                    break;
            }

            base.DeleteResource(resForDelete);
        }
    }
}
