using ClarolineApp.Settings;
using System;
using System.Linq;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Collections.Generic;
using ClarolineApp.Model;
using System.ComponentModel;

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

        public ObservableCollection<Notification> topNotifications
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                {
                    return new ObservableCollection<Notification>();
                }
                else
                {
                    return GetTopNotifications();
                }
            }
        }

        public override void LoadCollectionsFromDatabase()
        {
            base.LoadCollectionsFromDatabase();

            if (DesignerProperties.IsInDesignTool)
            {
                Cours c1 = new Cours() { title = "Design Cours 1", titular = "Design Titular 1", officialCode = "Design Code 1" };
                allCours.Add(c1);
                allCours.Add(new Cours() { title = "Design Cours 2", titular = "Design Titular 2", officialCode = "Design Code 2" });
                allCours.Add(new Cours() { title = "Design Cours 3", titular = "Design Titular 3", officialCode = "Design Code 3" });
                allCours.Add(new Cours() { title = "Design Cours 4", titular = "Design Titular 4", officialCode = "Design Code 4" });

                topNotifications.Add(new Notification() { resource = new ResourceModel() { title = "Design Resource 1", date = DateTime.Now }, Cours = c1, date = DateTime.Now });
            }
            else
            {

                allCours = new ObservableCollection<Cours>(from Cours c
                                                           in ClarolineDB.Cours_Table
                                                           select c);
                RaisePropertyChanged("topNotifications");
            }
        }

        public ObservableCollection<Notification> GetTopNotifications(int limit = 10, int offset = 0)
        {
            List<Notification> list = (from Notification n
                                          in ClarolineDB.Notifications_Table
                                          orderby n.date descending
                                          select n).ToList();
            return new ObservableCollection<Notification>(list.Where(n => n.isNotified)
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

            RaisePropertyChanged("topNotifications");
        }

        public override void AddNotification(Notification newNot)
        {
            base.AddNotification(newNot);

            RaisePropertyChanged("topNotifications");
        }

        public override void DeleteCours(Cours coursForDelete)
        {
            base.DeleteCours(coursForDelete);

            allCours.Remove(coursForDelete);

            RaisePropertyChanged("topNotifications");
        }

        public override void DeleteNotification(Notification notForDelete)
        {
            base.DeleteNotification(notForDelete);

            RaisePropertyChanged("topNotifications");
        }

        public override void ResetViewModel()
        {
            base.ResetViewModel();

            _allCours.Clear();
            RaisePropertyChanged("topNotifications");
        }
    }
}
