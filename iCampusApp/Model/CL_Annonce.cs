using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{

    [Table]
    public class CL_Annonce : ResourceModel
    {

        private int _ressourceId;

        [Column]
        public int ressourceId
        {
            get
            {
                return _ressourceId;
            }
            set
            {
                if (_ressourceId != value)
                {
                    NotifyPropertyChanging("ressourceId");
                    _ressourceId = value;
                    NotifyPropertyChanged("ressourceId");
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
            if (obj != null && obj.GetType() == typeof(CL_Annonce))
            {
                CL_Annonce ann = obj as CL_Annonce;
                return this._resourceListId == ann._resourceListId && this._ressourceId == ann._ressourceId;
            }
            return false;
        }

        public override string getNotificationText()
        {
            return _Title;
        }
    }
}
