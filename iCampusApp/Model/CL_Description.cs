using Newtonsoft.Json;
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
                    NotifyPropertyChanging("category");
                    _category = value;
                    NotifyPropertyChanged("category");
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(typeof(CL_Description)))
            {
                CL_Description dsc = obj as CL_Description;
                return this._resourceListId == dsc._resourceListId && this._resourceId == dsc._resourceId;
            }
            return false;
        }

        public override string getNotificationText()
        {
            return _Title;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool IsResIdMatching(string resource)
        {
            int val;
            return int.TryParse(resource, out val) && resourceId == val;
        }
    }
}
