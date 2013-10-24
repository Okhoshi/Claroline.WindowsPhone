using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarolineApp.RT.Models
{
    public class ItemModel : ViewModelBase
    {
        public Cours Cours
        {
            get { return GetProperty<Cours>("Cours"); }
            set { SetProperty<Cours>("Cours", value); }
        }

        public bool Updated
        {
            get { return GetProperty<bool>("Updated"); }
            set { SetProperty<bool>("Updated", value); }
        }

        public bool visibility
        {
            get { return GetProperty<bool>("visibility"); }
            set { SetProperty<bool>("visibility", value); }
        }

        public DateTime date
        {
            get { return GetProperty<DateTime>("date"); }
            set { SetProperty<DateTime>("date", value); }
        }

        public DateTime loaded
        {
            get { return GetProperty<DateTime>("loaded"); }
            set { SetProperty<DateTime>("loaded", value); }
        }

        public DateTime notified
        {
            get { return GetProperty<DateTime>("notified"); }
            set { SetProperty<DateTime>("notified", value); }
        }

        public bool isNotified
        {
            get { return notified.CompareTo(DateTime.Now) <= 0; }
        }

        public String title
        {
            get { return GetProperty<String>("title"); }
            set { SetProperty<String>("title", value); }
        }

        public ResourceModel<ItemModel> Group
        {
            get { return GetProperty<ResourceModel<ItemModel>>("Group"); }
            set { SetProperty<ResourceModel<ItemModel>>("Group", value); }
        }

    }
}
