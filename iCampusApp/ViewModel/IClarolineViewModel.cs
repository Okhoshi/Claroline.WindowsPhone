using ClarolineApp.Model;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ClarolineApp.ViewModel
{
    interface IClarolineViewModel : INotifyPropertyChanged
    {
        void LoadCollectionsFromDatabase();
        void ResetViewModel();
        void SaveChangesToDB();

        Task RefreshAsync();

        Task PrepareCoursForOpeningAsync(Cours coursToPrepare);
        Task GetResourcesForThisListAsync(ResourceList container);
        Task GetSingleResourceAsync(ResourceList container, string resourceId);

        Task GetUserDataAsync();
        Task GetCoursListAsync();
        Task GetResourcesListForThisCoursAsync(Cours cours);


        void AddCours(Cours newCours);
        void AddResourceList(ResourceList newList);
        void AddAnnonce(CL_Annonce newAnn);
        void AddDocument(CL_Document newDoc);
        void AddNotification(CL_Notification newNot);

        void DeleteCours(Cours coursForDelete);
        void DeleteResourceList(ResourceList listForDelete);
        void DeleteAnnonce(CL_Annonce annForDelete);
        void DeleteDocument(CL_Document docForDelete);
        void DeleteNotification(CL_Notification notForDelete);

        void ClearCoursList();
        void ClearDocsOfCours(Cours coursToClear);
        void ClearAnnsOfCours(Cours coursToClear);
        void ClearNotifsOfCours(Cours coursToClear, int keeped);
    }
}
