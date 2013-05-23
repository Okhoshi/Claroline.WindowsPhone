using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{

    public class CL_Annonce : ResourceModel
    {
        public new const string LABEL = "CLANN";

        public CL_Annonce()
            : base()
        {
            DiscKey = SupportedModules.CLANN;
        }

        protected string _Content;

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
            if (obj != null && obj.GetType().Equals(typeof(CL_Annonce)))
            {
                CL_Annonce ann = obj as CL_Annonce;
                return this._resourceListId == ann._resourceListId && this._resourceId == ann._resourceId;
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
    }
}
