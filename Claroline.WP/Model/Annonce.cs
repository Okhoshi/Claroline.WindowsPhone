using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
#if WINDOWS_PHONE
using System.Data.Linq.Mapping; 
#endif
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{

    public class Annonce : ResourceModel
    {
        public new const string Label = "CLANN";

        public Annonce()
            : base()
        {
            DiscKey = SupportedModules.CLANN;
        }

        protected string _Content;
#if WINDOWS_PHONE
        [Column(CanBeNull = true)]
#endif
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
                    RaisePropertyChanging("Content");
                    _Content = value;
                    RaisePropertyChanged("Content");
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(this.GetType()))
            {
                Annonce ann = obj as Annonce;
                return this._resourceListId == ann._resourceListId && this._resourceId == ann._resourceId;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override void UpdateFrom(ResourceModel newRes)
        {
            base.UpdateFrom(newRes);

            if (newRes is Annonce)
            {
                content = (newRes as Annonce).content;
            }
        }
    }
}
