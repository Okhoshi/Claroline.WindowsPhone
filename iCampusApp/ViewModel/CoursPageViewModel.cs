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

        public CoursPageViewModel(string sysCode, string DBConnectionString = ClarolineDataContext.DBConnectionString)
        {
            if (System.ComponentModel.DesignerProperties.IsInDesignTool)
            {
                currentCours = new Cours()
                {
                    officialCode = "DESIGN",
                    sysCode = "DESIGN",
                    title = "Design test"
                };

                ResourceList l1 = new ResourceList() { Cours = currentCours, name = "Resource L1", label = "L1", ressourceType = typeof(CL_Annonce) };
                ResourceList l2 = new ResourceList() { Cours = currentCours, name = "Resource L2", label = "L2", ressourceType = typeof(CL_Document) };
                currentCours.Resources.Add(l1);
                currentCours.Resources.Add(l2);
                currentCours.Resources.Add(new ResourceList() { Cours = currentCours, name = "Resource L4", label = "L4" });

                l1.Resources.Add(new CL_Annonce() { title = "Annonce 1", content = "Contenu 1", date = DateTime.Now });
                l1.Resources.Add(new CL_Annonce() { title = "Annonce 2 Annonce 2 Annonce 2", content = "Contenu 2", date = DateTime.Now });
                l1.Resources.Add(new CL_Annonce() { title = "Annonce 3", content = "Contenu 3", date = DateTime.Now });
                l1.Resources.Add(new CL_Annonce() { title = "Annonce 4", content = "Contenu 4", date = DateTime.Now });

                l2.Resources.Add(new CL_Document() { title = "Document 1", size = 15653614, date = DateTime.Now, extension = "ext1", isFolder = false });
                l2.Resources.Add(new CL_Document() { title = "Document 2 Document 2 Document 2", size = 165, date = DateTime.Now, extension = "et2", isFolder = false });
                l2.Resources.Add(new CL_Document() { title = "Document 3", size = 184351861, date = DateTime.Now, extension = "et3", isFolder = false });
                l2.Resources.Add(new CL_Document() { title = "Document 4", date = DateTime.Now, isFolder = true });
      
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
        }

        public override void LoadCollectionsFromDatabase()
        {
            base.LoadCollectionsFromDatabase();

            if (currentCours != null)
            {
                root = CL_Document.GetRootDocument(currentCours);
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
                    item.OpenDocumentAsync();
                }
                item = null;
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
