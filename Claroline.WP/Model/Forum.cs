using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

#if WINDOWS_PHONE
using System.Data.Linq;
using System.Data.Linq.Mapping;
using Microsoft.Phone.Data.Linq.Mapping;
#endif

namespace ClarolineApp.Model
{
#if WINDOWS_PHONE
    [Index(Name = "i_UID", IsUnique = true, Columns = "_resourceListId,resourceId")] 
#endif
    public class Forum : ResourceModel
    {
        public new const string Label = "CLFRM";

        public Forum()
            : base()
        {
            DiscKey = SupportedModules.CLFRM;
#if WINDOWS_PHONE
            _Topics = new EntitySet<Topic>(attach_Topics, detach_Topics); 
#endif

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
#if WINDPWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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

#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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
#if WINDOWS_PHONE
        [Column(CanBeNull = true)] 
#endif
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

        public new DateTime date
        {
            get
            {
                DateTime d = Topics.SelectMany(t => t.Posts).OrderBy(p => p.date).Select(p => p.date).LastOrDefault();
                if (d != null)
                {
                    return d;
                }
                else
                {
                    return DateTime.Parse("01/01/1753");
                }
            }
            set
            {
                _Date = DateTime.Parse("01/01/1753");
            }
        }

#if WINDOWS_PHONE
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
#else
        private ObservableCollection<Topic> _Topics;

        public ObservableCollection<Topic> Topics
        {
            get
            {
                return _Topics;
            }
            set
            {
                if (value != _Topics)
                {
                    _Topics = value;
                    RaisePropertyChanged("Topics");
                }
            }
        }
#endif
    }
}
