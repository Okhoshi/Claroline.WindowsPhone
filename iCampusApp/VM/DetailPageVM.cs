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
        private Annonce _currentAnnonce;

        public Annonce currentAnnonce
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
                    RaisePropertyChanged("currentAnnonce");
                }
            }
        }

        public DetailPageVM(int resid, int listid)
        {
            currentAnnonce = (from Annonce a
                              in ClarolineDB.Resources_Table
                              where a.resourceId == resid
                              && a.ResourceList.Id == listid
                              select a).FirstOrDefault();
        }
    }
}
