using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{

    public class Description : Annonce
    {
        public new const string Label = "CLDSC";

        public Description()
            : base()
        {
            DiscKey = SupportedModules.CLDSC;
        }

        private int _category;

        [Column(CanBeNull = true)]
        public int Category
        {
            get
            {
                return _category;
            }
            set
            {
                if (_category != value)
                {
                    NotifyPropertyChanging("Category");
                    _category = value;
                    NotifyPropertyChanged("Category");
                }
            }
        }

        public override string getNotificationText()
        {
            return _Title;
        }
    }
}
