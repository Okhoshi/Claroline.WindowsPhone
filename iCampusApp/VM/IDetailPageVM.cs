using ClarolineApp.Model;

namespace ClarolineApp.VM
{
    interface IDetailPageVM : IClarolineVM
    {
        ResourceModel currentResource
        {
            get;
            set;
        }
    }
}
