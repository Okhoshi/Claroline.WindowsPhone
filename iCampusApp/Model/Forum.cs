using Microsoft.Phone.Data.Linq.Mapping;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{
    [Index(Name = "i_UID", IsUnique = true, Columns = "_resourceListId,resourceId")]
    public class Forum : ResourceModel
    {
        public new const string Label = "CLFRM";

        public Forum()
            : base()
        {
            DiscKey = SupportedModules.CLFRM;
            _Topics = new EntitySet<Topic>(attach_Topics, detach_Topics);

            PropertyChanged += Forum_PropertyChanged;
        }

        void Forum_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ResourceList":
                case "resourceId":
                    RaisePropertyChanging("UniqueIdentifier");
                    RaisePropertyChanged("UniqueIdentifier");
                    break;
                default:
                    break;
            }
        }

        private string _ForumDescription;

        [JsonProperty("forum_desc")]
        [Column(CanBeNull = true)]
        public string ForumDescription
        {
            get
            {
                return _ForumDescription;
            }
            set
            {
                if (_ForumDescription != value)
                {
                    RaisePropertyChanging("ForumDescription");
                    _ForumDescription = value;
                    RaisePropertyChanged("ForumDescription");
                }
            }
        }

        [Column(CanBeNull = true)]
        public int UniqueIdentifier
        {
            get
            {
                return (_resourceListId + "-" + _resourceId).GetHashCode();
            }
            private set
            {
                return;
            }
        }

        private int _Rank;

        [JsonProperty("forum_order")]
        [Column(CanBeNull = true)]
        public int Rank
        {
            get
            {
                return _Rank;
            }
            set
            {
                if (_Rank != value)
                {
                    RaisePropertyChanging("Rank");
                    _Rank = value;
                    RaisePropertyChanged("Rank");
                }
            }
        }


        private int _CategoryId;

        [JsonProperty("cat_id")]
        [Column(CanBeNull = true)]
        public int CategoryId
        {
            get
            {
                return _CategoryId;
            }
            set
            {
                if (_CategoryId != value)
                {
                    RaisePropertyChanging("CategoryId");
                    _CategoryId = value;
                    RaisePropertyChanged("CategoryId");
                }
            }
        }


        private string _CategoryName;

        [JsonProperty("cat_title")]
        [Column(CanBeNull = true)]
        public string CategoryName
        {
            get
            {
                return _CategoryName;
            }
            set
            {
                if (_CategoryName != value)
                {
                    RaisePropertyChanging("CategoryName");
                    _CategoryName = value;
                    RaisePropertyChanged("CategoryName");
                }
            }
        }


        private int _CategoryRank;

        [JsonProperty("cat_order")]
        [Column(CanBeNull = true)]
        public int CategoryRank
        {
            get
            {
                return _CategoryRank;
            }
            set
            {
                if (_CategoryRank != value)
                {
                    RaisePropertyChanging("CategoryRank");
                    _CategoryRank = value;
                    RaisePropertyChanged("CategoryRank");
                }
            }
        }
        
        #region Association Forum2Topic Many Side

        // Define the entity set for the collection side of the relationship.

        private EntitySet<Topic> _Topics;

        [Association(Name = "Forum2Topic", Storage = "_Topics", OtherKey = "_ForumUId", ThisKey = "UniqueIdentifier", DeleteRule = "Cascade")]
        public EntitySet<Topic> Topics
        {
            get { return this._Topics; }
            set { this._Topics.Assign(value); }
        }

        // Called during an add operation

        private void attach_Topics(Topic _Topics)
        {
            RaisePropertyChanging("Topics");
            _Topics.Forum = this;
            RaisePropertyChanged("Topics");
        }

        // Called during a remove operation

        private void detach_Topics(Topic _Topics)
        {
            RaisePropertyChanging("Topics");
            _Topics.Forum = null;
            RaisePropertyChanged("Topics");
        }

        #endregion
    }
}
