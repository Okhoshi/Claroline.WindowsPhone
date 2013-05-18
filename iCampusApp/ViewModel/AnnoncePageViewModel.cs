﻿using ClarolineApp.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ClarolineApp.ViewModel
{
    public class AnnoncePageViewModel : ClarolineViewModel, IAnnoncePageViewModel
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

        public AnnoncePageViewModel(int resid, string DBConnectionString = ClarolineDataContext.DBConnectionString)
        {
            ClarolineDB = new ClarolineDataContext(DBConnectionString);

            currentAnnonce = (from CL_Annonce a
                              in ClarolineDB.Annonces_Table
                              where a.resourceId == resid
                              select a).FirstOrDefault();
        }
    }
}
