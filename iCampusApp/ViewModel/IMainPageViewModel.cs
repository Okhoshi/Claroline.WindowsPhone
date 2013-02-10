using ClarolineApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ClarolineApp.ViewModel
{
    interface IMainPageViewModel : IClarolineViewModel
    {
        ObservableCollection<CL_Notification> GetTopNotifications(int limit = 0, int offset = 0);
        void PrepareCoursForOpeningAsync(Cours coursToPrepare);
    }
}
