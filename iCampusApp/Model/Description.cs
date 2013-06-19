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
                    RaisePropertyChanging("category");
                    _category = value;
                    RaisePropertyChanged("category");
                }
            }
        }

        public override string getNotificationText()
        {
            return _Title;
        }

        public override void UpdateFrom(ResourceModel newRes)
        {
            base.UpdateFrom(newRes);

            if (newRes is Description)
            {
                category = (newRes as Description).category;
            }
        }
    }
}
