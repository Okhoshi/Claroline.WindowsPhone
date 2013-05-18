using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{

    public class CL_Description : ResourceModel
    {
        public new const string LABEL = "CLDSC";

        public CL_Description()
            : base()
        {
            DiscKey = SupportedModules.CLDSC;
        }

        private int _resourceId;

        [Column(CanBeNull = true)]
        public int resourceId
        {
            get
            {
                return _resourceId;
            }
            set
            {
                if (_resourceId != value)
                {
                    NotifyPropertyChanging("resourceId");
                    _resourceId = value;
                    NotifyPropertyChanged("resourceId");
                }
            }
        }

        private string _Content;

        [Column(CanBeNull = true)]
        public string content
        {
            get
            {
                return _Content;
            }
            set
            {
                if (_Content != value)
                {
                    NotifyPropertyChanging("content");
                    _Content = value;
                    NotifyPropertyChanged("content");
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

        public new static List<CL_Description> ConvertFromJson(string json)
        {
            return JsonConvert.DeserializeObject<List<CL_Description>>(json);
        }

        public override bool IsResIdMatching(string resource)
        {
            int val;
            return int.TryParse(resource, out val) && resourceId == val;
        }
    }
}
