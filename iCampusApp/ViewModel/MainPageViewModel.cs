using ClarolineApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ClarolineApp.ViewModel
{
    class MainPageViewModel : ClarolineViewModel, IMainPageViewModel
    {

        public MainPageViewModel(string DBConnectionString)
            :base(DBConnectionString)
        {
        }

        public ObservableCollection<CL_Notification> GetTopNotifications(int limit = 0, int offset = 0)
        {
            throw new NotImplementedException();
        }

        public async void PrepareCoursForOpeningAsync(Cours coursToPrepare)
        {
            throw new NotImplementedException();
        }

        public override void LoadCollectionsFromDatabase()
        {
            throw new NotImplementedException();
        }

        public override async void RefreshAsync()
        {
            throw new NotImplementedException();
        }
    }
}
