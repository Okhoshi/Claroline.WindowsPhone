using ClarolineApp.Model;

namespace ClarolineApp.VM
{
    public interface IDetailPageVM : IClarolineVM
    {
        ResourceModel currentResource
        {
            get;
            set;
        }
    }
}
