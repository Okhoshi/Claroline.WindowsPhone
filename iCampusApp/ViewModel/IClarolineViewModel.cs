using ClarolineApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace ClarolineApp.ViewModel
{
    interface IClarolineViewModel : INotifyPropertyChanged
    {
         void LoadCollectionsFromDatabase();
         void ResetDatabase();
         void SaveChangesToDB();
         void RefreshAsync();

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
