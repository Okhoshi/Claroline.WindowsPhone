﻿using ClarolineApp.Model;
using System.Collections.ObjectModel;

namespace ClarolineApp.VM
{
    public interface IMainPageVM : IClarolineVM
    {
        ObservableCollection<Notification> GetTopNotifications(int limit = 0, int offset = 0);
    }
}
