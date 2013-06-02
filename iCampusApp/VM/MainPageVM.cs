using ClarolineApp.Settings;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClarolineApp.Model;

namespace ClarolineApp.VM
{
    public class MainPageVM : ClarolineVM, IMainPageVM
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
                    RaisePropertyChanged("allCours");
                }
            }
        }

        private ObservableCollection<Notification> _topNotifications;
        public ObservableCollection<Notification> topNotifications
        {
            get
            {
                if (_topNotifications == null)
                {
                    _topNotifications = new ObservableCollection<Notification>();
                }
                return _topNotifications;
            }
            set
            {
                if (_topNotifications != value)
                {
                    _topNotifications = value;
                    RaisePropertyChanged("topNotifications");
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
            List<Notification> list = (from Notification n
                                          in ClarolineDB.Notifications_Table
                                          orderby n.date descending
                                          select n).ToList();
            topNotifications = new ObservableCollection<Notification>(list.Where(n => n.isNotified)
                                                                             .Skip(offset)
                                                                             .Take(limit));
        }

        public override async Task RefreshAsync(bool force = false)
        {
            if (allCours.Count == 0)
            {
                await GetCoursListAsync();
            }
            else
            {
                await base.RefreshAsync(force);
            }
        }

        public override void AddCours(Cours newCours)
        {
            base.AddCours(newCours);

            if (!allCours.Contains(newCours))
            {
                allCours.Add(newCours);
            }

            SetTopNotifications();
        }

        public override void AddNotification(Notification newNot)
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

        public override void DeleteNotification(Notification notForDelete)
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
