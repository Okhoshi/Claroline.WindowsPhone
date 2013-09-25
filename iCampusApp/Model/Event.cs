using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{

    public class Event : Annonce
    {
        public new const string Label = "CLCAL";

        public Event()
            : base()
        {
            DiscKey = SupportedModules.CLCAL;
        }
        
        private string _Speakers;

        [Column(CanBeNull = true)]
        public string speakers
        {
            get
            {
                return _Speakers;
            }
            set
            {
                if (_Speakers != value)
                {
                    RaisePropertyChanging("speakers");
                    _Speakers = value;
                    RaisePropertyChanged("speakers");
                }
            }
        }

        private string _Location;

        [Column(CanBeNull = true)]
        public string location
        {
            get
            {
                return _Location;
            }
            set
            {
                if (_Location != value)
                {
                    RaisePropertyChanging("location");
                    _Location = value;
                    RaisePropertyChanged("location");
                }
            }
        }
        
        private string _Lasting;

        [Column(CanBeNull = true)]
        public string lasting
        {
            get
            {
                return _Lasting;
            }
            set
            {
                if (_Lasting != value)
                {
                    RaisePropertyChanging("lasting");
                    _Lasting = value;
                    RaisePropertyChanged("lasting");
                }
            }
        }

        public override bool Equals(object obj)
        {
            if (obj != null && obj.GetType().Equals(typeof(Event)))
            {
                Event evt = obj as Event;
                return this._resourceListId == evt._resourceListId && this._resourceId == evt._resourceId;
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

        public override void UpdateFrom(ResourceModel newRes)
        {
            base.UpdateFrom(newRes);

            if (newRes is Event)
            {
                speakers = (newRes as Event).speakers;
                location = (newRes as Event).location;
                lasting = (newRes as Event).lasting;
            }
        }
    }
}
