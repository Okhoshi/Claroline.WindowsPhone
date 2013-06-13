using ClarolineApp.Model;
using ClarolineApp.Settings;
using System;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ClarolineApp.VM
{
    interface IClarolineVM : INotifyPropertyChanged
    {
        void LoadCollectionsFromDatabase();
        void ResetViewModel();
        void SaveChangesToDB();

        Task RefreshAsync(bool force = false);

        Task PrepareCoursForOpeningAsync(Cours coursToPrepare);
        Task GetResourcesForThisListAsync(ResourceList container);
        Task GetSingleResourceAsync(ResourceList container, string resourceId);

        Task<bool> GetUserDataAsync();
        Task GetCoursListAsync();
        Task<bool> GetResourcesListForThisCoursAsync(Cours cours);

        void AddCours(Cours newCours);
        void AddResourceList(ResourceList newList, int coursId);
        void AddResource(ResourceModel newRes, int listId);
        void AddNotification(Notification newNot);

        void DeleteCours(Cours coursForDelete);
        void DeleteResourceList(ResourceList listForDelete);
        void DeleteResource(ResourceModel resForDelete);
        void DeleteNotification(Notification notForDelete);

        void ClearCoursList();
        void ClearResOfCours(Cours coursToClear);
        void ClearNotifsOfCours(Cours coursToClear, int keeped);

        Uri GetNavigationTarget();
    }
}
