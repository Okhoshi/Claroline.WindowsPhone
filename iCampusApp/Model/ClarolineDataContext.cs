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
        }

        public Table<Cours> Cours_Table;
        public Table<ResourceList> ResourceList_Table;
        public Table<ResourceModel> Resources_Table;
        public Table<CL_Notification> Notifications_Table;

        public IQueryable<CL_Annonce> Annonces_Table
        {
            get{
                return from CL_Annonce a
                       in Resources_Table
                       where a is CL_Annonce
                       select a;
                   }
        }

        public IQueryable<CL_Document> Documents_Table
        {
            get
            {
                return from CL_Document d
                       in Resources_Table
                       where d is CL_Document
                       select d;
            }
        }
    }
}