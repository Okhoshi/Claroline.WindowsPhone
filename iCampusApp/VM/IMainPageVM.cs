using ClarolineApp.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarolineApp.VM
{
    interface IMainPageVM : IClarolineVM
    {
        void SetTopNotifications(int limit = 0, int offset = 0);
    }
}
