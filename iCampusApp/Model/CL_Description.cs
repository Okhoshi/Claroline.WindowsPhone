﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{

    public class CL_Description : CL_Annonce
    {
        public new const string LABEL = "CLDSC";

        public CL_Description()
            : base()
        {
            DiscKey = SupportedModules.CLDSC;
        }

        private int _category = -1;

        [Column]
        public int category
        {
            get
            {
                return _category;
            }
            set
            {
                if (_category != value)
                {
                    NotifyPropertyChanging("category");
                    _category = value;
                    NotifyPropertyChanged("category");
                }
            }
        }

        public override string getNotificationText()
        {
            return _Title;
        }
    }
}
