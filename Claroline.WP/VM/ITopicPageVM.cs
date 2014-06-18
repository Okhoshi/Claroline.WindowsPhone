using ClarolineApp.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ClarolineApp.VM
{
    public interface ITopicPageVM : IClarolineVM
    {
        Topic currentTopic
        {
            get;
            set;
        }

        ObservableCollection<Post> posts
        {
            get;
        }
    }
}
