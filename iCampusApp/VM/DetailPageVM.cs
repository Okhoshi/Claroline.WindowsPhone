using ClarolineApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClarolineApp.VM
{
    public class DetailPageVM : ClarolineVM, IDetailPageVM
    {
        private CL_Annonce _currentAnnonce;

        public CL_Annonce currentAnnonce
        {
            get
            {
                return _currentAnnonce;
            }
            set
            {
                if (_currentAnnonce != value)
                {
                    _currentAnnonce = value;
                    NotifyPropertyChanged("currentAnnonce");
                }
            }
        }

        public DetailPageVM(int resid, int listid)
        {
            currentAnnonce = (from CL_Annonce a
                              in ClarolineDB.Resources_Table
                              where a.resourceId == resid
                              && a.resourceList.Id == listid
                              select a).FirstOrDefault();
        }
    }
}
