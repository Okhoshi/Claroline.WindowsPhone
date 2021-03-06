﻿using ClarolineApp.Model;
using ClarolineApp.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ClarolineApp.VM
{
    public class TopicPageVM : ClarolineVM, ITopicPageVM
    {
        private Topic _currentTopic;
        public Topic currentTopic
        {
            get
            {
                return _currentTopic;
            }
            set
            {
                if (value != _currentTopic)
                {
                    _currentTopic = value;
                    RaisePropertyChanged("currentTopic");
                    RaisePropertyChanged("posts");
                }
            }
        }

        public ObservableCollection<Post> posts
        {
            get
            {
                return new ObservableCollection<Post>(currentTopic.Posts.OrderBy(p => p.date));
            }
        }

        public TopicPageVM()
            : this(null, 0, 0)
        {
            if (DesignerProperties.IsInDesignTool)
            {
                currentTopic = new Topic()
                {
                    title = "Design Time Topic Title",
                    PosterFirstname = "John",
                    PosterLastname = "Doe",
                    Views = 14
                };

                currentTopic.Posts.Add(new Post()
                {
                    PosterFirstname = "Jane",
                    PosterLastname = "Doe",
                    Text = "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,"+
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,"+
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,"+
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1"
                });
                currentTopic.Posts.Add(new Post()
                {
                    PosterFirstname = "Jane",
                    PosterLastname = "Doe",
                    Text = "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1," +
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1," +
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1," +
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1"
                });
                currentTopic.Posts.Add(new Post()
                {
                    PosterFirstname = "Jane",
                    PosterLastname = "Doe",
                    Text = "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1," +
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1," +
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1," +
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1"
                });
                currentTopic.Posts.Add(new Post()
                {
                    PosterFirstname = "Jane",
                    PosterLastname = "Doe",
                    Text = "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1," +
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1," +
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1," +
                    "Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1,Design Text 1"
                });
            }
        }

        public TopicPageVM(ISettings settings, int topicId, int forumUId)
            : base(settings)
        {
            if (!DesignerProperties.IsInDesignTool)
            {
                using (ClarolineDataContext cdc = new ClarolineDataContext(ClarolineDataContext.DBConnectionString))
                {
                    currentTopic = (from Topic t in cdc.Topics_Table
                                    where t.resourceId == topicId && t.Forum.UniqueIdentifier == forumUId
                                    select t).Single();
                    var p = posts;
                }
            }
        }
    }
}
