using ClarolineApp.Model;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
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

        public ICommand eventSelectedItem
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
                    NotifyPropertyChanged("currentCours");
                }
            }
        }

        private CL_Document _root;

        public CL_Document root
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
                    NotifyPropertyChanged("root");
                    NotifyPropertyChanged("content");
                }
            }
        }

        public ObservableCollection<CL_Document> content
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                {
                    ObservableCollection<CL_Document> ret = new ObservableCollection<CL_Document>();
                    ret.Add(new CL_Document() { title = "Document 1", size = 15653614, date = DateTime.Now, extension = "pdf", isFolder = false });
                    ret.Add(new CL_Document() { title = "Document 2 Document 2 Document 2", size = 165, date = DateTime.Now, extension = "et2", isFolder = false });
                    ret.Add(new CL_Document() { title = "Document 3", size = 184351861, date = DateTime.Now, extension = "ppt", isFolder = false });
                    ret.Add(new CL_Document() { title = "Hello Folder", description = "WTF", date = DateTime.Now, isFolder = false });
                    return ret;
                }
                else
                {
                    return _root.getContent();
                }
            }
        }

        private ObservableCollection<Group<CL_Event>> _events;

        public ObservableCollection<Group<CL_Event>> events
        {
            get
            {
                return _events;
            }
            set
            {
                if (_events != value)
                {
                    _events = value;
                    NotifyPropertyChanged("events");
                }
            }
        }

        private ObservableCollection<Group<CL_Description>> _descriptions;

        public ObservableCollection<Group<CL_Description>> descriptions
        {
            get
            {
                if (_descriptions == null)
                {
                    _descriptions = new ObservableCollection<Group<CL_Description>>(from CL_Description d
                                                                                    in ClarolineDB.Descriptions_Table
                                                                                    where d.resourceList.Cours.Equals(currentCours)
                                                                                    group d by d.category into g
                                                                                    orderby g.Key
                                                                                    select new Group<CL_Description>(g.Key != -1 ? g.First().title : "Autres", g));
                }
                return _descriptions;
            }
        }

#if(DEBUG)
        public CoursPageVM()
            : this("")
        {
        }
#endif

        public CoursPageVM(string sysCode, string DBConnectionString = ClarolineDataContext.DBConnectionString)
        {
            if (DesignerProperties.IsInDesignTool)
            {
                currentCours = new Cours()
                {
                    officialCode = "DESIGN",
                    sysCode = "DESIGN",
                    title = "Design test"
                };

                ResourceList l1 = new ResourceList() { Cours = currentCours, name = "Resource L1", label = "CLANN", ressourceType = typeof(CL_Annonce) };
                ResourceList l2 = new ResourceList() { Cours = currentCours, name = "Resource L2", label = "CLDOC", ressourceType = typeof(CL_Document) };
                ResourceList l3 = new ResourceList() { Cours = currentCours, name = "Resource L3", label = "CLDSC", ressourceType = typeof(CL_Description) };
                ResourceList l4 = new ResourceList() { Cours = currentCours, name = "Resource L4", label = "CLCAL", ressourceType = typeof(CL_Event) };

                currentCours.Resources.Add(l2);
                currentCours.Resources.Add(new ResourceList() { Cours = currentCours, name = "L4", label = "L4" });

                l1.Resources.Add(new CL_Annonce() { title = "Annonce 1", content = "Contenu 1 Contenu 1 v Contenu 1 Contenu 1 Contenu 1 v Contenu 1", date = DateTime.Now });
                l1.Resources.Add(new CL_Annonce() { title = "Annonce 2 Annonce 2 Annonce 2", content = "Contenu 2", date = DateTime.Now });
                l1.Resources.Add(new CL_Annonce() { title = "Annonce 3", content = "Contenu 3", date = DateTime.Now });
                l1.Resources.Add(new CL_Annonce() { title = "Annonce 4", content = "Contenu 4", date = DateTime.Now });

                l3.Resources.Add(new CL_Description() { title = "Annonce 1", content = "Contenu 1", date = DateTime.Now });

                l4.Resources.Add(new CL_Event() { title = "Event 1", content = "Contenu 1", date = DateTime.Now, speakers = "Mr Nobody, Someone else" });
                l4.Resources.Add(new CL_Event() { title = "Event 2", content = "Contenu 1", date = DateTime.Now, location = "This is a very very very very veyr vey long location" });
                l4.Resources.Add(new CL_Event() { title = "Event 3", content = "Contenu 1", date = DateTime.Now, location = "B543", speakers = "Mr Nobody, Someone else" });

                events = new ObservableCollection<Group<CL_Event>>();
                events.Add(new Group<CL_Event>("Aujourd'hui", l4.Resources.Cast<CL_Event>()));
                events.Add(new Group<CL_Event>("Demain", l4.Resources.Cast<CL_Event>()));
                events.Add(new Group<CL_Event>("Passés", l4.Resources.Cast<CL_Event>()));

                _descriptions = new ObservableCollection<Group<CL_Description>>();
                _descriptions.Add(new Group<CL_Description>("First", l3.Resources.Cast<CL_Description>()));


            }
            else
            {
                ClarolineDB = new ClarolineDataContext(DBConnectionString);

                currentCours = (from Cours c
                                in ClarolineDB.Cours_Table
                                where c.sysCode.Equals(sysCode)
                                select c).FirstOrDefault();

                LoadCollectionsFromDatabase();
            }

            genericSelectedItem = new RelayCommand<ResourceModel>(this.OnGenericItemSelected);
            documentSelectedItem = new RelayCommand<CL_Document>(this.OnDocumentItemSelected);
            annonceSelectedItem = new RelayCommand<CL_Annonce>(this.OnAnnonceItemSelected);
            eventSelectedItem = new RelayCommand<CL_Event>(this.OnGenericItemSelected);
        }

        public override void LoadCollectionsFromDatabase()
        {
            base.LoadCollectionsFromDatabase();

            if (currentCours != null)
            {
                root = CL_Document.GetRootDocument(currentCours);

                events = new ObservableCollection<Group<CL_Event>>();

                events.Add(new Group<CL_Event>("Aujourd'hui", from CL_Event e in ClarolineDB.Events_Table
                                                              where e.date.CompareTo(DateTime.Now) > 0
                                                              && e.date.Date.CompareTo(DateTime.Now.Date) == 0
                                                              && e.resourceList.Cours == currentCours
                                                              select e));

                events.Add(new Group<CL_Event>("Demain", from CL_Event e in ClarolineDB.Events_Table
                                                         where e.date.Date.CompareTo(DateTime.Now.Date.AddDays(1.0)) == 0
                                                              && e.resourceList.Cours == currentCours
                                                         select e));

                events.Add(new Group<CL_Event>("Cette semaine", from CL_Event e in ClarolineDB.Events_Table
                                                                where e.date.Date.CompareTo(DateTime.Now.Date.AddDays(1.0)) >= 0
                                                                && e.date.Date.CompareTo(DateTime.Now.Date.AddDays(7.0)) < 0
                                                              && e.resourceList.Cours == currentCours
                                                                select e));

                events.Add(new Group<CL_Event>("Plus tard", from CL_Event e in ClarolineDB.Events_Table
                                                            where e.date.Date.CompareTo(DateTime.Now.Date.AddDays(7.0)) >= 0
                                                              && e.resourceList.Cours == currentCours
                                                            select e));
                events.Add(new Group<CL_Event>("Passés", from CL_Event e in ClarolineDB.Events_Table
                                                         where e.date.CompareTo(DateTime.Now) < 0
                                                              && e.resourceList.Cours == currentCours
                                                         select e));
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

        public void OnAnnonceItemSelected(CL_Annonce item)
        {
            item.seenDate = DateTime.Now;
            SaveChangesToDB();

            navigationTarget = new Uri(String.Format("/View/AnnonceDetail.xaml?resource={0}", item.resourceId), UriKind.Relative);
        }

        public void OnDocumentItemSelected(CL_Document item)
        {
            var dbItem = (from CL_Document d
                            in ClarolineDB.Documents_Table
                          select d).FirstOrDefault(d => d.Equals(item));
            if (dbItem != null)
            {
                dbItem.seenDate = DateTime.Now;
                SaveChangesToDB();

                if (dbItem.isFolder)
                {
                    root = dbItem;
                }
                else
                {
                    dbItem.OpenDocumentAsync();
                }
            }
        }

        public void GoUp()
        {
            root = root.getRoot();
        }

        public bool IsOnRoot()
        {
            return root.Equals(CL_Document.GetRootDocument(currentCours));
        }

        public bool IsDocumentPivotSelected(object SelectedItem)
        {
            return (SelectedItem as ResourceList).ressourceType.Equals(typeof(CL_Document));
        }
    }
}
