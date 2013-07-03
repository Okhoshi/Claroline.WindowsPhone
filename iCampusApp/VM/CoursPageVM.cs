using ClarolineApp.Model;
using ClarolineApp.Settings;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace ClarolineApp.VM
{
    public class CoursPageVM : ClarolineVM, ICoursPageVM
    {
        public ICommand genericSelectedItem
        {
            get;
            private set;
        }

        public ICommand documentSelectedItem
        {
            get;
            private set;
        }

        public ICommand annonceSelectedItem
        {
            get;
            private set;
        }

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
                    RaisePropertyChanged("events");
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
                    RaisePropertyChanged("content");
                }
            }
        }

        public ObservableCollection<Document> content
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

        private ObservableCollection<Group<Event>> _events;

        public ObservableCollection<Group<Event>> events
        {
            get
            {
                if (_events == null)
                {
                    _events = new ObservableCollection<Group<Event>>();

                    if (currentCours != null)
                    {
                        ResourceList rl = currentCours.Resources.FirstOrDefault(l => l.ressourceType == typeof(Event));
                        if (rl != null)
                        {
                            IEnumerable<Event> list = rl.Resources.Cast<Event>();

                            _events.Add(new Group<Event>("Aujourd'hui", list.Where(e =>
                            {
                                return e.date.CompareTo(DateTime.Now) > 0
                                && e.date.Date.CompareTo(DateTime.Now.Date) == 0;
                            })));

                            _events.Add(new Group<Event>("Demain", list.Where(e =>
                            {
                                return e.date.Date.CompareTo(DateTime.Now.Date.AddDays(1.0)) == 0;
                            })));

                            _events.Add(new Group<Event>("Cette semaine", list.Where(e =>
                            {
                                return e.date.Date.CompareTo(DateTime.Now.Date.AddDays(1.0)) >= 0
                                && e.date.Date.CompareTo(DateTime.Now.Date.AddDays(7.0)) < 0;
                            })));

                            _events.Add(new Group<Event>("Plus tard", list.Where(e =>
                            {
                                return e.date.Date.CompareTo(DateTime.Now.Date.AddDays(7.0)) >= 0;
                            })));

                            _events.Add(new Group<Event>("Passés", list.Where(e =>
                            {
                                return e.date.CompareTo(DateTime.Now) < 0;
                            })));
                        }
                    }
                }
                return _events;
            }
            set
            {
                if (_events != value)
                {
                    _events = value;
                    RaisePropertyChanged("events");
                }
            }
        }

        private ObservableCollection<Group<Description>> _descriptions;

        public ObservableCollection<Group<Description>> descriptions
        {
            get
            {
                if (_descriptions == null)
                {
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
                                                    list.Select(d =>
                                                    {
                                                        if (d.category == -1)
                                                        {
                                                            d.title = "Autres";
                                                            d.category = otherCat;
                                                        }
                                                        return d;
                                                    })
                                                    .GroupBy(d => d.category)
                                                    .OrderBy(g => g.Key)
                                                    .Select(g => new Group<Description>(g.First().title, g))
                                                );
                        }
                    }
                    else
                    {
                        _descriptions = new ObservableCollection<Group<Description>>();
                    }
                }
                return _descriptions;
            }
        }

        private ObservableCollection<Group<Forum>> _forums;

        public ObservableCollection<Group<Forum>> forums
        {
            get
            {
                if (_forums == null)
                {
                    ResourceList rl = currentCours.Resources.First(r => r.ressourceType == typeof(Forum));
                    if (rl != null)
                    {
                        IEnumerable<Forum> list = rl.Resources.Cast<Forum>();

                        if (list.Count() == 0)
                        {
                            _forums = new ObservableCollection<Group<Forum>>();
                        }
                        else
                        {
                            _forums = new ObservableCollection<Group<Forum>>(
                                                    list.OrderBy(f => f.Rank)
                                                    .GroupBy(f => f.CategoryId)
                                                    .OrderBy(g => g.First().CategoryRank)
                                                    .Select(g => new Group<Forum>(g.First().CategoryName, g))
                                                );
                        }
                    }
                    else
                    {
                        _forums = new ObservableCollection<Group<Forum>>();
                    }
                }
                return _forums;
            }
            set
            {
                if (_forums != value)
                {
                    _forums = value;
                    RaisePropertyChanged("forums");
                }
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
                    _resources = new ObservableCollection<ResourceList>(currentCours.Resources.Where(l => l.visibility && l.Resources.Count > 0));
                    _resources.Insert(0, PivotMenu);
                }
                return _resources;
            }
        }

#if(DEBUG)
        public CoursPageVM()
            : this("")
        {
        }
#endif

        public CoursPageVM(string sysCode)
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
            }

            genericSelectedItem = new RelayCommand<ResourceModel>(this.OnGenericItemSelected);
            documentSelectedItem = new RelayCommand<Document>(this.OnDocumentItemSelected);
            annonceSelectedItem = new RelayCommand<Annonce>(this.OnItemWithDetailsSelected);
        }

        public override void LoadCollectionsFromDatabase()
        {
            if (!IsInDesignMode)
            {
                base.LoadCollectionsFromDatabase();

                if (currentCours != null)
                {
                    root = Document.GetRootDocument(currentCours);

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
                SaveChangesToDB();

                if (item.isFolder)
                {
                    root = item;
                }
                else
                {
                    //dbItem.OpenDocumentAsync();
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
    }
}
