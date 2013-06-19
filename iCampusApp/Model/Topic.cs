using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Data.Linq.Mapping;
using System.Linq;
using System.Text;

namespace ClarolineApp.Model
{
    public class Topic : ResourceModel
    {
        public new const string Label = "CLFRM_TOPIC";

        public Topic()
            : base()
        {
            DiscKey = SupportedModules.CLFRM_TOPIC;

            _Posts = new EntitySet<Post>(attach_Posts, detach_Posts);
        }

        
        private int _Views;

        [JsonProperty("topic_views")]
        [Column(CanBeNull = true)]
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
        [Column(CanBeNull = true)]
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
        [Column(CanBeNull = true)]
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
        
        #region Entity Side for Forum2Topic

        [Column(CanBeNull = true)]
        protected int _ForumId;

        protected EntityRef<Forum> _Forum;

        // Association, to describe the relationship between this key and that "storage" table

        [Association(Name = "Forum2Topic", Storage = "_Forum", ThisKey = "_ForumId", OtherKey = "Id", IsForeignKey = false)]
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
                            this._Forum.Entity = null;
                            previousValue.Topics.Remove(this);
                        }
                        this._Forum.Entity = value;

                        value.Topics.Add(this);
                        this._ForumId = value.Id;
                    }
                }

                RaisePropertyChanged("Forum");
            }
        }

        #endregion
        
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
        
    }
}
