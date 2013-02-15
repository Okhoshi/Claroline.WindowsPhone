using ClarolineApp.Model;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ClarolineApp.ViewModel
{
    class MainPageViewModel : ClarolineViewModel, IMainPageViewModel
    {

        private ObservableCollection<Cours> _allCours;
        public ObservableCollection<Cours> allCours
        {
            get
            {
                if (_allCours == null)
                {
                    _allCours = new ObservableCollection<Cours>();
                }
                return _allCours;
            }
            set
            {
                if (_allCours != value)
                {
                    _allCours = value;
                    NotifyPropertyChanged("allCours");
                }
            }
        }

        private ObservableCollection<CL_Notification> _topNotifications;
        public ObservableCollection<CL_Notification> topNotifications
        {
            get
            {
                if (_topNotifications == null)
                {
                    _topNotifications = new ObservableCollection<CL_Notification>();
                }
                return _topNotifications;
            }
            set
            {
                if (_topNotifications != value)
                {
                    _topNotifications = value;
                    NotifyPropertyChanged("topNotifications");
                }
            }
        }

        public override void LoadCollectionsFromDatabase()
        {
            base.LoadCollectionsFromDatabase();

            allCours = new ObservableCollection<Cours>(from Cours c
                                                       in ClarolineDB.Cours_Table
                                                       select c);
            SetTopNotifications();
        }

        public void SetTopNotifications(int limit = 10, int offset = 0)
        {
            List<CL_Notification> list = (from CL_Notification n
                                          in ClarolineDB.Notifications_Table
                                          orderby n.date descending
                                          select n).ToList();
            topNotifications = new ObservableCollection<CL_Notification>(list.Where(n => n.isNotified)
                                                                             .Skip(offset)
                                                                             .Take(limit));
        }

        public override void AddCours(Cours newCours)
        {
            base.AddCours(newCours);

            if (allCours.Contains(newCours))
            {
                allCours.Remove(newCours);
            }

            allCours.Add(newCours);

            SetTopNotifications();
        }

        public override void AddNotification(CL_Notification newNot)
        {
            base.AddNotification(newNot);

            SetTopNotifications();
        }

        public override void DeleteCours(Cours coursForDelete)
        {
            base.DeleteCours(coursForDelete);

            allCours.Remove(coursForDelete);

            SetTopNotifications();
        }

        public override void DeleteNotification(CL_Notification notForDelete)
        {
            base.DeleteNotification(notForDelete);

            topNotifications.Remove(notForDelete);
        }

        public override void ResetViewModel()
        {
            base.ResetViewModel();

            _allCours.Clear();
            _topNotifications.Clear();
        }
    }
}
