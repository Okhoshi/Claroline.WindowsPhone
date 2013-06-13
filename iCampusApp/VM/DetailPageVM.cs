using ClarolineApp.Model;
using ClarolineApp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        private bool IsNotified;

        public DetailPageVM(int resid, int listid)
        {
            currentResource = (from r
                              in ClarolineDB.Resources_Table
                               where r.resourceId == resid
                               && r.ResourceList.Id == listid
                               select r).FirstOrDefault();

            IsNotified = currentResource.isNotified;

            currentResource.seenDate = DateTime.Now;
            SaveChangesToDB();
        }

        public override async Task RefreshAsync(bool force = false)
        {
            if (IsNotified)
            {
                await GetSingleResourceAsync(currentResource.ResourceList, currentResource.GetResourceString());

                currentResource = (from r
                                   in ClarolineDB.Resources_Table
                                   where r.Id == currentResource.Id
                                   select r).FirstOrDefault();
                currentResource.seenDate = DateTime.Now;
                SaveChangesToDB();

                IsNotified = false;
            }
            else
            {
                await base.RefreshAsync(force);
            }
        }
    }
}
