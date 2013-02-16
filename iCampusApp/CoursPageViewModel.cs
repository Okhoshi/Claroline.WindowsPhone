using ClarolineApp.Model;
using ClarolineApp.ViewModel;
using System.Collections.ObjectModel;
using System.Linq;

namespace ClarolineApp
{
    public class CoursPageViewModel : ClarolineViewModel, ICoursPageViewModel
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
                    NotifyPropertyChanged("currentCours");
                    NotifyPropertyChanged("documentPanel");
                    NotifyPropertyChanged("annoncePanel");
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

        public ResourceList documentPanel
        {
            get
            {
                return currentCours.Resources.First(rl => rl.ressourceType.Equals(typeof(CL_Document)));
            }
        }

        public ResourceList annoncePanel
        {
            get
            {
                return currentCours.Resources.First(rl => rl.ressourceType.Equals(typeof(CL_Annonce)));
            }
        }

        public CoursPageViewModel(string sysCode, string DBConnectionString = ClarolineDataContext.DBConnectionString)
            : base(DBConnectionString)
        {
            currentCours = (from Cours c
                            in ClarolineDB.Cours_Table
                            where c.sysCode.Equals(sysCode)
                            select c).FirstOrDefault();
        }

        public override void LoadCollectionsFromDatabase()
        {
            base.LoadCollectionsFromDatabase();

            root = CL_Document.GetRootDocument(currentCours);
        }
    }
}
