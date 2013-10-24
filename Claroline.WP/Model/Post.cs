using ClarolineApp.Common;
using Newtonsoft.Json;
using System;
#if WINDOWS_PHONE
using System.Data.Linq;
using System.Data.Linq.Mapping; 
#endif

namespace ClarolineApp.Model
{

#if WINDOWS_PHONE
    [Table] 
#endif
    public class Post : ModelBase
    {

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
        
        private string _PosterLastname;

        [JsonProperty("lastname")]
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

        [JsonProperty("firstname")]
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
        
        private string _Text;

        [JsonProperty("post_text")]
#if WINDOWS_PHONE
        [Column] 
#endif
        public string Text
        {
            get
            {
                return _Text;
            }
            set
            {
                if (_Text != value)
                {
                    RaisePropertyChanging("Text");
                    _Text = value;
                    RaisePropertyChanged("Text");
                }
            }
        }

        protected DateTime _NotifiedDate = DateTime.Parse("01/01/1753");

#if WINDOWS_PHONE
        [Column] 
#endif
        public DateTime notifiedDate
        {
            get
            {
                return _NotifiedDate;
            }
            set
            {
                if (_NotifiedDate != value)
                {
                    RaisePropertyChanging("notifiedDate");
                    _NotifiedDate = value;
                    RaisePropertyChanged("notifiedDate");
                    RaisePropertyChanged("isNotified");
                }
            }
        }

        public bool isNotified
        {
            get
            {
                return seenDate.CompareTo(notifiedDate) < 0;
            }
        }

        protected DateTime _SeenDate = DateTime.Parse("01/01/1753");

#if WINDOWS_PHONE
        [Column] 
#endif
        public DateTime seenDate
        {
            get
            {
                return _SeenDate;
            }
            set
            {
                if (_SeenDate != value)
                {
                    RaisePropertyChanging("seenDate");
                    _SeenDate = value;
                    RaisePropertyChanged("seenDate");
                    RaisePropertyChanged("isNotified");
                }
            }
        }

        protected DateTime _Date = DateTime.Parse("01/01/1753");

#if WINDOWS_PHONE
        [Column] 
#endif
        public DateTime date
        {
            get
            {
                return _Date;
            }
            set
            {
                if (_Date != value)
                {
                    RaisePropertyChanging("date");
                    _Date = value;
                    RaisePropertyChanged("date");
                }
            }
        }

#if WINDOWS_PHONE
		        #region Entity Side for Topic2Posts
  

        [Column]
        protected int _TopicId;

        protected EntityRef<Topic> _Topic;

        // Association, to describe the relationship between this key and that "storage" table

        [Association(Name = "Topic2Posts", Storage = "_Topic", ThisKey = "_TopicId", OtherKey = "Id", IsForeignKey = true)]
        public Topic Topic
        {
            get { return _Topic.Entity; }
            set
            {
                RaisePropertyChanging("Topic");

                if (value != null)
                {
                    Topic previousValue = this._Topic.Entity;
                    if (((previousValue != value) || (this._Topic.HasLoadedOrAssignedValue == false)))
                    {
                        if ((previousValue != null))
                        {
                            this._Topic.Entity = null;
                            previousValue.Posts.Remove(this);
                        }
                        this._Topic.Entity = value;

                        value.Posts.Add(this);
                        this._TopicId = value.Id;
                    }
                }

                RaisePropertyChanged("Topic");
            }
        }

        #endregion
#else
        protected int _TopicId;

        protected Topic _Topic;

        public Topic Topic
        {
            get { return _Topic; }
            set
            {
                RaisePropertyChanging("Topic");

                if (value != null)
                {
                    Topic previousValue = this._Topic;
                    if (previousValue != value)
                    {
                        if (previousValue != null)
                        {
                            this._Topic = null;
                            previousValue.Posts.Remove(this);
                        }
                        this._Topic = value;

                        value.Posts.Add(this);
                        this._TopicId = value.Id;
                    }
                }

                RaisePropertyChanged("Topic");
            }
        }
#endif

    }
}
