using ClarolineApp.Model;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ClarolineApp.ViewModel
{
    interface IClarolineViewModel : INotifyPropertyChanged
    {
        void LoadCollectionsFromDatabase();
        void ResetViewModel();
        void SaveChangesToDB();

        Task RefreshAsync(bool force = false);

        Task PrepareCoursForOpeningAsync(Cours coursToPrepare);
        Task GetResourcesForThisListAsync(ResourceList container);
        Task GetSingleResourceAsync(ResourceList container, string resourceId);

        Task GetUserDataAsync();
        Task GetCoursListAsync();
        Task GetResourcesListForThisCoursAsync(Cours cours);

        void AddCours(Cours newCours);
        void AddResourceList(ResourceList newList);
        void AddNotification(CL_Notification newNot);

        void DeleteCours(Cours coursForDelete);
        void DeleteResourceList(ResourceList listForDelete);
        void DeleteResource(ResourceModel resForDelete);
        void DeleteNotification(CL_Notification notForDelete);

        void ClearCoursList();
        void ClearResOfCours(Cours coursToClear);
        void ClearNotifsOfCours(Cours coursToClear, int keeped);

        Uri GetNavigationTarget();
    }
}
