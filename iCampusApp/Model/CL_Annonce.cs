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
        public const string LABEL = "CLANN";

        private int _resourceId;

        [Column]
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

        [Column]
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
