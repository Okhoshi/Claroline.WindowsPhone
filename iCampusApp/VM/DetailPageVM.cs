using ClarolineApp.Model;
using ClarolineApp.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarolineApp.VM
{
    public class DetailPageVM : ClarolineVM, IDetailPageVM
    {
        private ResourceModel _currentResource;

        public ResourceModel currentResource
        {
            get
            {
                return _currentResource;
            }
            set
            {
                if (_currentResource != value)
                {
                    _currentResource = value;
                    RaisePropertyChanged("currentResource");
                }
            }
        }

        private bool IsNotified;

        public DetailPageVM()
        {
            if (IsInDesignMode)
            {
                currentResource = new Forum() { title = "Design Forum" };
                Topic t1 = new Topic() { Forum = currentResource as Forum, PosterFirstname = "John", PosterLastname = "Doe", title = "Design Topic 1", Views = 15 };
                Topic t2 = new Topic() { Forum = currentResource as Forum, PosterFirstname = "Jane", PosterLastname = "Doe", title = "Design Topic 2", Views = 15 };

                (currentResource as Forum).Topics.AddRange(new Topic[] { t1, t2 });
                
                t1.Posts.Add(new Post()
                {
                    PosterFirstname = "John",
                    PosterLastname = "Doe",
                    Topic = t1,
                    Text = "Design Text 1 Design Text 1 Design Text 1 "
                         + "Design Text 1 Design Text 1 Design Text 1 "
                         + "Design Text 1 Design Text 1 Design Text 1 "
                         + "Design Text 1 Design Text 1 Design Text 1 "
                         + "Design Text 1 Design Text 1 Design Text 1 "
                });
                t1.Posts.Add(new Post()
                {
                    PosterFirstname = "John",
                    PosterLastname = "Doe",
                    Topic = t1,
                    Text = "Design Text 2 Design Text 2 Design Text 2 "
                         + "Design Text 2 Design Text 2 Design Text 2 "
                         + "Design Text 2 Design Text 2 Design Text 2 "
                         + "Design Text 2 Design Text 2 Design Text 2 "
                         + "Design Text 2 Design Text 2 Design Text 2 "
                });

                t1.Posts.Add(new Post()
                {
                    PosterFirstname = "Jane",
                    PosterLastname = "Doe",
                    Topic = t1,
                    Text = "Design Text 3 Design Text 3 Design Text 3 "
                         + "Design Text 3 Design Text 3 Design Text 3 "
                         + "Design Text 3 Design Text 3 Design Text 3 "
                         + "Design Text 3 Design Text 3 Design Text 3 "
                         + "Design Text 3 Design Text 3 Design Text 3 "
                });

                t2.Posts.Add(new Post()
                {
                    PosterFirstname = "Jane",
                    PosterLastname = "Doe",
                    Topic = t1,
                    Text = "Design Text 1 Design Text 1 Design Text 1 "
                         + "Design Text 1 Design Text 1 Design Text 1 "
                         + "Design Text 1 Design Text 1 Design Text 1 "
                         + "Design Text 1 Design Text 1 Design Text 1 "
                         + "Design Text 1 Design Text 1 Design Text 1 "
                });
            }
        }

        public DetailPageVM(string resid, int listid)
        {
            if(!IsInDesignMode)
            {
                ClarolineDB.Dispose();
                ClarolineDB = null;

                currentResource = (from r
                                  in ClarolineDB.Resources_Table
                                   where r.resourceId == resid
                                   && r.ResourceList.Id == listid
                                   select r).FirstOrDefault();

                IsNotified = currentResource.isNotified;

                currentResource.seenDate = DateTime.Now;
                SaveChangesToDB(); 
            }
        }

        public override async Task RefreshAsync(bool force = false)
        {
            if (IsNotified)
            {
                await GetSingleResourceAsync(currentResource.ResourceList, currentResource.GetResourceString());

                currentResource = (from r
                                   in ClarolineDB.Resources_Table
                                   where r.Id == currentResource.Id
                                   select r).FirstOrDefault();
                currentResource.seenDate = DateTime.Now;
                SaveChangesToDB();

                IsNotified = false;
            }
            else
            {
                await base.RefreshAsync(force);
            }
        }
    }
}
