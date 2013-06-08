using ClarolineApp.Model;
using ClarolineApp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClarolineApp.VM
{
    public class DetailPageVM : ClarolineVM, IDetailPageVM
    {
        private ResourceModel _currentResource;

        public ResourceModel currentResource
        {
            get
            {
                return _currentResource;
            }
            set
            {
                if (_currentResource != value)
                {
                    _currentResource = value;
                    RaisePropertyChanged("currentResource");
                }
            }
        }

        public DetailPageVM(int resid, int listid)
        {
            currentResource = (from a
                              in ClarolineDB.Resources_Table
                              where a.resourceId == resid
                              && a.ResourceList.Id == listid
                              select a).FirstOrDefault();
        }
    }
}
