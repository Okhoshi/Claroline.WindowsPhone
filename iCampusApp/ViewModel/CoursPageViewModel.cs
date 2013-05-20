using ClarolineApp.Model;
using GalaSoft.MvvmLight.Command;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace ClarolineApp.ViewModel
{
    public class CoursPageViewModel : ClarolineViewModel, ICoursPageViewModel
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
                return _root.getContent();
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
                                                                                    group d by d.category into g
                                                                                    orderby g.Key
                                                                                    select new Group<CL_Description>(g.Key != -1 ? g.First().title : "Autres", g));
                }
                return _descriptions;
            }
        }

        public CoursPageViewModel(string sysCode, string DBConnectionString = ClarolineDataContext.DBConnectionString)
        {
                ClarolineDB = new ClarolineDataContext(DBConnectionString);

                currentCours = (from Cours c
                                in ClarolineDB.Cours_Table
                                where c.sysCode.Equals(sysCode)
                                select c).FirstOrDefault();

                LoadCollectionsFromDatabase();
            
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
                events.Add(new Group<CL_Event>("Passés", from CL_Event e in ClarolineDB.Events_Table
                                                         where e.date.CompareTo(DateTime.Now) < 0
                                                         select e));

                events.Add(new Group<CL_Event>("Aujourd'hui", from CL_Event e in ClarolineDB.Events_Table
                                                              where e.date.CompareTo(DateTime.Now) > 0
                                                              && e.date.Date.CompareTo(DateTime.Now.Date) == 0
                                                              select e));

                events.Add(new Group<CL_Event>("Demain", from CL_Event e in ClarolineDB.Events_Table
                                                         where e.date.Date.CompareTo(DateTime.Now.Date.AddDays(1.0)) == 0
                                                         select e));

                events.Add(new Group<CL_Event>("Cette semaine", from CL_Event e in ClarolineDB.Events_Table
                                                                where e.date.Date.CompareTo(DateTime.Now.Date.AddDays(1.0)) >= 0
                                                                && e.date.Date.CompareTo(DateTime.Now.Date.AddDays(7.0)) < 0
                                                                select e));

                events.Add(new Group<CL_Event>("Plus tard", from CL_Event e in ClarolineDB.Events_Table
                                                            where e.date.Date.CompareTo(DateTime.Now.Date.AddDays(7.0)) >= 0
                                                            select e));
            }
        }

        public void OnGenericItemSelected(ResourceModel item)
        {
            item.seenDate = DateTime.Now;
            SaveChangesToDB();
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
            item = null;
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
