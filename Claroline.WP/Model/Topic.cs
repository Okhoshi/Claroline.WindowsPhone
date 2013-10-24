using ClarolineApp.Common;
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
#endif

namespace ClarolineApp.Model
{
#if WINDOWS_PHONE
    [Table] 
#endif
    public class Topic : ModelBase
    {

        public Topic()
            : base()
        {
#if WINDOWS_PHONE
            _Posts = new EntitySet<Post>(attach_Posts, detach_Posts); 
#else
            _Posts = new ObservableCollection<Post>();
#endif
        }

        protected int _Id;

        // Define ID: internal Notifications, public property and database column.

#if WINDOWS_PHONE
        [Column(IsPrimaryKey = true, IsDbGenerated = true, DbType = "INT NOT NULL Identity", CanBeNull = false, AutoSync = AutoSync.OnInsert)] 
#endif
        public virtual int Id
        {
            get
            {
                return _Id;
            }
            set
            {
                if (_Id != value)
                {
                    RaisePropertyChanging("Id");
                    _Id = value;
                    RaisePropertyChanged("Id");
                }
            }
        }

        protected string _Title;

#if WINDOWS_PHONE
        [Column] 
#endif
        public string title
        {
            get
            {
                return _Title;
            }
            set
            {
                if (value != _Title)
                {
                    RaisePropertyChanging("title");
                    _Title = value;
                    RaisePropertyChanged("title");
                }
            }
        }

        protected int _resourceId;

#if WINDOWS_PHONE
        [Column] 
#endif
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
                    RaisePropertyChanging("resourceId");
                    _resourceId = value;
                    RaisePropertyChanged("resourceId");
                }
            }
        }

        public bool isNotified
        {
            get
            {
                return true;
            }
        }

        private int _Views;

        [JsonProperty("topic_views")]
#if WINDOWS_PHONE
        [Column] 
#endif
        public int Views
        {
            get
            {
                return _Views;
            }
            set
            {
                if (_Views != value)
                {
                    RaisePropertyChanging("Views");
                    _Views = value;
                    RaisePropertyChanged("Views");
                }
            }
        }
        
        private string _PosterLastname;

        [JsonProperty("poster_lastname")]
#if WINDOWS_PHONE
        [Column] 
#endif
        public string PosterLastname
        {
            get
            {
                return _PosterLastname;
            }
            set
            {
                if (_PosterLastname != value)
                {
                    RaisePropertyChanging("PosterLastname");
                    _PosterLastname = value;
                    RaisePropertyChanged("PosterLastname");
                }
            }
        }
        
        private string _PosterFirstname;

        [JsonProperty("poster_firstname")]
#if WINDOWS_PHONE
        [Column] 
#endif
        public string PosterFirstname
        {
            get
            {
                return _PosterFirstname;
            }
            set
            {
                if (_PosterFirstname != value)
                {
                    RaisePropertyChanging("PosterFirstname");
                    _PosterFirstname = value;
                    RaisePropertyChanged("PosterFirstname");
                }
            }
        }

#if WINDOWS_PHONE
        #region Entity Side for Forum2Topic

        [Column]
        protected int _ForumUId;

        protected EntityRef<Forum> _Forum;

        // Association, to describe the relationship between this key and that "storage" table

        [Association(Name = "Forum2Topic", Storage = "_Forum", ThisKey = "_ForumUId", OtherKey = "UniqueIdentifier", IsForeignKey = false)]
        public Forum Forum
        {
            get { return _Forum.Entity; }
            set
            {
                RaisePropertyChanging("Forum");

                if (value != null)
                {
                    Forum previousValue = this._Forum.Entity;
                    if (((previousValue != value) || (this._Forum.HasLoadedOrAssignedValue == false)))
                    {
                        if ((previousValue != null))
                        {
                            this._Forum.Entity.PropertyChanged -= Forum_PropertyChanged;
                            this._Forum.Entity = null;
                            previousValue.Topics.Remove(this);
                        }
                        this._Forum.Entity = value;
                        this._Forum.Entity.PropertyChanged += Forum_PropertyChanged;

                        value.Topics.Add(this);
                        this._ForumUId = value.UniqueIdentifier;
                    }
                }

                RaisePropertyChanged("Forum");
            }
        }

        #endregion
#else
        protected int _ForumUId;

        protected Forum _Forum;

        public Forum Forum
        {
            get { return _Forum; }
            set
            {
                RaisePropertyChanging("Forum");

                if (value != null)
                {
                    Forum previousValue = this._Forum;
                    if (previousValue != value)
                    {
                        if ((previousValue != null))
                        {
                            this._Forum.PropertyChanged -= Forum_PropertyChanged;
                            this._Forum = null;
                            previousValue.Topics.Remove(this);
                        }
                        this._Forum = value;
                        this._Forum.PropertyChanged += Forum_PropertyChanged;

                        value.Topics.Add(this);
                        this._ForumUId = value.UniqueIdentifier;
                    }
                }

                RaisePropertyChanged("Forum");
            }
        }
#endif   
        
        private void Forum_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "UniqueIdentifier":
                    this._ForumUId = Forum.UniqueIdentifier;
                    break;
                default:
                    break;
            }
        }

#if WINDOWS_PHONE
        #region Association Topic2Posts Many Side

        // Define the entity set for the collection side of the relationship.

        private EntitySet<Post> _Posts;

        [Association(Name = "Topic2Posts", Storage = "_Posts", OtherKey = "_TopicId", ThisKey = "Id", DeleteRule = "Cascade")]
        public EntitySet<Post> Posts
        {
            get { return this._Posts; }
            set { this._Posts.Assign(value); }
        }

        // Called during an add operation

        private void attach_Posts(Post _Posts)
        {
            RaisePropertyChanging("Posts");
            _Posts.Topic = this;
            RaisePropertyChanged("Posts");
        }

        // Called during a remove operation

        private void detach_Posts(Post _Posts)
        {
            RaisePropertyChanging("Posts");
            _Posts.Topic = null;
            RaisePropertyChanged("Posts");
        }

        #endregion 
#else
        private ObservableCollection<Post> _Posts;

        public ObservableCollection<Post> Posts
        {
            get
            {
                return _Posts;
            }
            set
            {
                if (value != _Posts)
                {
                    _Posts = value;
                    RaisePropertyChanged("Resources");
                }
            }
        }
#endif
    }
}
