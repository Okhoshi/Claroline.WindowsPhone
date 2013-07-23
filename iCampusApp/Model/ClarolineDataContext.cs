using System;
using System.ComponentModel;
using System.Data.Linq;
using System.Linq;
using System.Data.Linq.Mapping;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using ClarolineApp.Languages;

namespace ClarolineApp.Model
{
    public class ClarolineDataContext : DataContext
    {
        // Specify the connection string as a static, used in main page and app.xaml.
        public const string DBConnectionString = "Data Source=isostore:/Claroline.sdf";

        // Pass the connection string to the base class.
        public ClarolineDataContext(string connectionString)
            : base(connectionString)
        {
#if(DEBUG)
           // Log = new DebugStreamWriter();
#endif
        }

        public Table<Cours> Cours_Table;
        public Table<ResourceList> ResourceList_Table;
        public Table<ResourceModel> Resources_Table;
        public Table<Notification> Notifications_Table;
        public Table<Topic> Topics_Table;
        public Table<Post> Posts_Table;

        public IQueryable<Annonce> Annonces_Table
        {
            get
            {
                return from Annonce a
                       in Resources_Table
                       where a is Annonce
                       && !(a is Description)
                       && !(a is Event)
                       select a;
            }
        }

        public IQueryable<Document> Documents_Table
        {
            get
            {
                return from Document d
                       in Resources_Table
                       where d is Document
                       select d;
            }
        }

        public IQueryable<Description> Descriptions_Table
        {
            get
            {
                return from Description d
                       in Resources_Table
                       where d is Description
                       select d;
            }
        }

        public IQueryable<Event> Events_Table
        {
            get
            {
                return from Event e
                       in Resources_Table
                       where e is Event
                       select e;
            }
        }

        public IQueryable<Forum> Forum_Table
        {
            get
            {
                return from Forum f
                       in Resources_Table
                       where f is Forum
                       select f;
            }
        }
    }
}