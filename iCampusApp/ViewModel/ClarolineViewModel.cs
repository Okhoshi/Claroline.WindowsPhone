using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

// Directive for the data model.
using ClarolineApp.Model;

namespace ClarolineApp.ViewModel
{
    public class ClarolineViewModel : INotifyPropertyChanged
    {
        // LINQ to SQL data context for the local database.

        private ClarolineDataContext ClarolineDB;

        // Class constructor, create the data context object.

        public ClarolineViewModel(string DBConnectionString)
        {
            ClarolineDB = new ClarolineDataContext(DBConnectionString);
        }

        private ObservableCollection<Cours> _allCours;
        public ObservableCollection<Cours> AllCours
        {
            get { return _allCours; }
            set
            {
                _allCours = value;
                NotifyPropertyChanged("AllCours");
            }
        }
        
        private ObservableCollection<CL_Document> _allFiles;
        public ObservableCollection<CL_Document> AllFiles
        {
            get { return _allFiles; }
            set
            {
                _allFiles = value;
                NotifyPropertyChanged("AllFiles");
            }
        }

        private ObservableCollection<CL_Document> _allFolders;
        public ObservableCollection<CL_Document> AllFolders
        {
            get { return _allFolders; }
            set
            {
                _allFolders = value;
                NotifyPropertyChanged("AllFolders");
            }
        }

        private ObservableCollection<CL_Annonce> _allAnnonces;
        public ObservableCollection<CL_Annonce> AllAnnonces
        {
            get { return _allAnnonces; }
            set
            {
                _allAnnonces = value;
                NotifyPropertyChanged("AllAnnonces");
            }
        }

        private ObservableCollection<CL_Notification> _allNotifications;
        public ObservableCollection<CL_Notification> AllNotifications
        {
            get { return _allNotifications; }
            set
            {
                _allNotifications = value;
                NotifyPropertyChanged("AllNotifications");
            }
        }

        private Dictionary<string, ObservableCollection<CL_Notification>> _notifByCours;
        public Dictionary<string, ObservableCollection<CL_Notification>> NotifByCours
        {
            get { return _notifByCours; }
            set
            {
                _notifByCours = value;
                NotifyPropertyChanged("NotifByCours");
            }
        }
  
        // Write changes in the data context to the database.

        public void SaveChangesToDB()
        {
            ClarolineDB.SubmitChanges();
        }

        // Query database and load the collections and list used by the pivot pages.

        public void LoadCollectionsFromDatabase()
        {

            // Specify the query for all courses in the database.

            var coursInDB = from Cours cours in ClarolineDB.Cours_Table
                            select cours;

            // Query the database and load all to-do items.

            AllCours = new ObservableCollection<Cours>(coursInDB);

            // Specify the query for all section in the database.

            var filesInDb = from CL_Document file in ClarolineDB.Documents_Table
                            where !file.isFolder
                            select file;

            AllFiles = new ObservableCollection<CL_Document>(filesInDb);


            var foldersInDb = from CL_Document fold in ClarolineDB.Documents_Table
                              where fold.isFolder
                              select fold;
            AllFolders = new ObservableCollection<CL_Document>(foldersInDb);

            var annsInDb = from CL_Annonce ann in ClarolineDB.Annonces_Table
                              select ann;
            AllAnnonces = new ObservableCollection<CL_Annonce>(annsInDb);

            var notifsInDb = (from CL_Notification not in ClarolineDB.Notifications_Table
                             orderby not.date descending
                             select not).Take(100);
            AllNotifications = new ObservableCollection<CL_Notification>(notifsInDb);

            // Query the database and load all associated items to their respective collections.
            NotifByCours = new Dictionary<string, ObservableCollection<CL_Notification>>();

            foreach (Cours _cours in coursInDB)
            {
                NotifByCours.Add(_cours.sysCode, new ObservableCollection<CL_Notification>());

                foreach (CL_Notification not in _cours.Notifications)
                    NotifByCours[_cours.sysCode].Add(not);
            }
        }

        public void ResetDatabase()
        {
            ClarolineDB.Documents_Table.DeleteAllOnSubmit(AllFiles);
            ClarolineDB.Documents_Table.DeleteAllOnSubmit(AllFolders);
            ClarolineDB.Annonces_Table.DeleteAllOnSubmit(AllAnnonces);
            ClarolineDB.Notifications_Table.DeleteAllOnSubmit(AllNotifications);
            ClarolineDB.Cours_Table.DeleteAllOnSubmit(AllCours);
            ClarolineDB.SubmitChanges();
            _allFiles.Clear();
            _allFolders.Clear();
            _allAnnonces.Clear();
            _allNotifications.Clear();
            _allCours.Clear();
            _notifByCours.Clear();
        }

        public void AddDocument(CL_Document newDoc)
        {
            newDoc.updated = true;

            if (newDoc.isFolder)
            {
                if (!AllFolders.Contains(newDoc))
                {
                    // Add a to-do item to the data context.

                    ClarolineDB.Documents_Table.InsertOnSubmit(newDoc);

                    // Save changes to the database.

                    ClarolineDB.SubmitChanges();

                    // Add a to-do item to the "all" observable collection.

                    AllFolders.Add(newDoc);
                }
                else
                    UpdateFolder(newDoc);
            }
            else
            {
                if (!AllFiles.Contains(newDoc))
                {
                    // Add a to-do item to the data context.

                    ClarolineDB.Documents_Table.InsertOnSubmit(newDoc);
                    // Save changes to the database.

                    ClarolineDB.SubmitChanges();

                    // Add a to-do item to the "all" observable collection.

                    AllFiles.Add(newDoc);

                    AddNotification(CL_Notification.CreateNotification(newDoc, false));

                }
                else
                    UpdateFile(newDoc);
            }
        }

        private void UpdateFolder(CL_Document newDoc)
        {
            CL_Document foldInDb = (from CL_Document _fold in ClarolineDB.Documents_Table
                                  where _fold.Equals(AllFolders[AllFolders.IndexOf(newDoc)])
                                  select _fold).First();

            foldInDb.notifiedDate = newDoc.notifiedDate;
            foldInDb.date = newDoc.date;
            foldInDb.updated = newDoc.updated;

            ClarolineDB.SubmitChanges();

            AllFolders[AllFolders.IndexOf(foldInDb)].notifiedDate = newDoc.notifiedDate;
            AllFolders[AllFolders.IndexOf(foldInDb)].date = newDoc.date;
            AllFolders[AllFolders.IndexOf(foldInDb)].updated = newDoc.updated;

        }

        private void UpdateFile(CL_Document newFile)
        {
            CL_Document fileInDb = (from CL_Document _file in ClarolineDB.Documents_Table
                                  where _file.Equals(AllFiles[AllFiles.IndexOf(newFile)])
                             select _file).First();

            fileInDb.notifiedDate = newFile.notifiedDate;
            fileInDb.date = newFile.date;
            fileInDb.updated = newFile.updated;
            fileInDb.date = newFile.date;

            ClarolineDB.SubmitChanges();
            
            AllFiles[AllFiles.IndexOf(fileInDb)].notifiedDate = newFile.notifiedDate;
            AllFiles[AllFiles.IndexOf(fileInDb)].date = newFile.date;
            AllFiles[AllFiles.IndexOf(fileInDb)].updated = newFile.updated;
            AllFiles[AllFiles.IndexOf(fileInDb)].date = newFile.date;

            AddNotification(CL_Notification.CreateNotification(fileInDb, true));
        }

        public void DeleteFolder(CL_Document docForDelete)
        {
            if (docForDelete.isFolder)
            {
                // Remove the section item from the "all" observable collection.

                AllFolders.Remove(docForDelete);

                foreach (CL_Document DocForDelete in docForDelete.getContent())
                {
                    DeleteFolder(DocForDelete);
                }
                // Remove the section item from the data context.

                ClarolineDB.Documents_Table.DeleteOnSubmit(docForDelete);
            }
            else
            {
                // Remove the section item from the "all" observable collection.

                AllFiles.Remove(docForDelete);
                
                // Remove the section item from the data context.

                ClarolineDB.Documents_Table.DeleteOnSubmit(docForDelete);
            }
            // Save changes to the database.

            ClarolineDB.SubmitChanges();
        }

        public void AddAnnonce(CL_Annonce newAnn)
        {
            newAnn.updated = true;

            if (!AllAnnonces.Contains(newAnn))
            {
                // Add a to-do item to the data context.

                ClarolineDB.Annonces_Table.InsertOnSubmit(newAnn);

                // Save changes to the database.

                ClarolineDB.SubmitChanges();

                // Add a to-do item to the "all" observable collection.

                AllAnnonces.Add(newAnn);

                AddNotification(CL_Notification.CreateNotification(newAnn, false));
            }
            else
                UpdateAnn(newAnn);
        }

        private void UpdateAnn(CL_Annonce newAnn)
        {
            CL_Annonce annInDb = (from CL_Annonce _ann in ClarolineDB.Annonces_Table
                               where _ann.Equals(AllAnnonces[AllAnnonces.IndexOf(newAnn)])
                               select _ann).First();

            annInDb.notifiedDate = newAnn.notifiedDate;
            AllAnnonces[AllAnnonces.IndexOf(annInDb)].notifiedDate = newAnn.notifiedDate;
            annInDb.updated = newAnn.updated;
            AllAnnonces[AllAnnonces.IndexOf(annInDb)].updated = newAnn.updated;
            if (annInDb.date.CompareTo(newAnn.date) < 0)
            {
                annInDb.date = newAnn.date;
                AllAnnonces[AllAnnonces.IndexOf(annInDb)].date = newAnn.date;
            }
            annInDb.content = newAnn.content;
            AllAnnonces[AllAnnonces.IndexOf(annInDb)].content = newAnn.content;
            annInDb.upToDateContent = newAnn.upToDateContent;
            AllAnnonces[AllAnnonces.IndexOf(annInDb)].upToDateContent = newAnn.upToDateContent;
            ClarolineDB.SubmitChanges();

            AddNotification(CL_Notification.CreateNotification(annInDb, true));
        }

        public void DeleteAnnonce(CL_Annonce annForDelete)
        {

            // Remove the cours item from the "all" observable collection.

            AllAnnonces.Remove(annForDelete);

            AnnByCours[annForDelete.Cours.sysCode].Remove(annForDelete);

            // Remove the cours item from the data context.

            ClarolineDB.Annonces_Table.DeleteOnSubmit(annForDelete);

            // Save changes to the database.

            ClarolineDB.SubmitChanges();
        }

        public void AddNotification(CL_Notification newNot)
        {
            newNot.updated = true;
            newNot.Cours.notifiedDate = true;
            UpdateCours(newNot.Cours);

            if (AllNotifications.Contains(newNot))
            {
                CL_Notification not = (from CL_Notification not_ in AllNotifications
                                    where not_.Equals(newNot)
                                    orderby not_.date descending
                                    select not_).First();
                if (not.date.Date == newNot.date.Date)
                    return;
            }
                // Add a to-do item to the data context.

                ClarolineDB.Notifications_Table.InsertOnSubmit(newNot);

                // Save changes to the database.

                ClarolineDB.SubmitChanges();

                // Add a to-do item to the "all" observable collection.

                AllNotifications.Add(newNot);

                NotifByCours[newNot.Cours.sysCode].Add(newNot);
            /*}
            else
                UpdateNot(newNot);*/
        }

        private void UpdateNot(CL_Notification newNot)
        {
            CL_Notification notInDb = (from CL_Notification _not in ClarolineDB.Notifications_Table
                               where _not.Equals(newNot)
                               select _not).First();

            notInDb.notifiedDate = newNot.notifiedDate;
            NotifByCours[notInDb.Cours.sysCode][NotifByCours[notInDb.Cours.sysCode].IndexOf(notInDb)].notifiedDate = newNot.notifiedDate;
            AllNotifications[AllNotifications.IndexOf(notInDb)].notifiedDate = newNot.notifiedDate;
            notInDb.updated = newNot.updated;
            NotifByCours[notInDb.Cours.sysCode][NotifByCours[notInDb.Cours.sysCode].IndexOf(notInDb)].updated = newNot.updated;
            AllNotifications[AllNotifications.IndexOf(notInDb)].updated = newNot.updated;
            ClarolineDB.SubmitChanges();
        }

        public void DeleteNotification(CL_Notification notForDelete)
        {

            // Remove the cours item from the "all" observable collection.

            AllNotifications.Remove(notForDelete);

            NotifByCours[notForDelete.Cours.sysCode].Remove(notForDelete);

            // Remove the cours item from the data context.

            ClarolineDB.Notifications_Table.DeleteOnSubmit(notForDelete);

            // Save changes to the database.

            ClarolineDB.SubmitChanges();
        }

        public void AddCours(Cours newCours)
        {
            newCours.updated = true;

            if (!AllCours.Contains(newCours))
            {
                // Add a to-do item to the data context.

                ClarolineDB.Cours_Table.InsertOnSubmit(newCours);

                // Save changes to the database.

                ClarolineDB.SubmitChanges();

                // Add a to-do item to the "all" observable collection.

                AllCours.Add(newCours);

                NotifByCours.Add(newCours.sysCode, new ObservableCollection<CL_Notification>());
            }
            else
                UpdateCours(newCours);
        }

        private void UpdateCours(Cours newCours)
        {
            Cours coursInDb = (from Cours _cours in ClarolineDB.Cours_Table
                               where _cours.Equals(AllCours[AllCours.IndexOf(newCours)])
                               select _cours).First();

            coursInDb.updated = newCours.updated;
            ClarolineDB.SubmitChanges();

            AllCours[AllCours.IndexOf(coursInDb)].updated = newCours.updated;
        }

        public void DeleteCours(Cours coursForDelete)
        {
            // Remove the cours item from the "all" observable collection.
            var queryDoc = from CL_Document _doc in ClarolineDB.Documents_Table
                           where _doc._coursId == coursForDelete.Id
                           select _doc;
            foreach (var Doc in queryDoc)
            {
                //System.Diagnostics.Debug.WriteLine("Deleting document : " + Doc.Name);
                if (Doc.IsFolder)
                    AllFolders.Remove(Doc);
                else
                    AllFiles.Remove(Doc);
                ClarolineDB.Documents_Table.DeleteOnSubmit(Doc);
            }
            DocByCours.Remove(coursForDelete.sysCode);

            var queryAnn = from CL_Annonce _ann in ClarolineDB.Annonces_Table
                        where _ann._coursId == coursForDelete.Id
                        select _ann;
            foreach (var Ann in queryAnn)
            {
                //System.Diagnostics.Debug.WriteLine("Deleting document : " + Ann.title);
                ClarolineDB.Annonces_Table.DeleteOnSubmit(Ann);
                AllAnnonces.Remove(Ann);
            }
            AnnByCours.Remove(coursForDelete.sysCode);

            var queryNot = from CL_Notification _not in ClarolineDB.Notifications_Table
                           where _not._coursId == coursForDelete.Id
                           select _not;
            foreach (var not in queryNot)
            {
                ClarolineDB.Notifications_Table.DeleteOnSubmit(not);
                AllNotifications.Remove(not);
            }
            NotifByCours.Remove(coursForDelete.sysCode);

            // Remove the cours item from the data context.

            ClarolineDB.Cours_Table.DeleteOnSubmit(coursForDelete);

            // Save changes to the database.

            ClarolineDB.SubmitChanges();
        }

        public void ClearCoursList()
        {
            ObservableCollection<Cours> toDel = new ObservableCollection<Cours>();

            foreach (Cours currentCours in _allCours)
            {
                if (!currentCours.updated)
                {
                    DeleteCours(currentCours);
                    toDel.Add(currentCours);
                }
                else
                    currentCours.updated = false;
            }

            foreach (Cours item in toDel)
            {
                _allCours.Remove(item);
            }
        }

        public void ClearDocsOfCours(Cours coursToClear)
        {
            foreach (CL_Document currentDoc in _docByCours[coursToClear.sysCode])
            {
                if (!currentDoc.updated)
                    DeleteFolder(currentDoc);
                else
                    currentDoc.updated = false;
            }
        }

        public void ClearAnnsOfCours(Cours coursToClear)
        {
            foreach (CL_Annonce currentAnn in _annByCours[coursToClear.sysCode])
            {
                if (!currentAnn.updated)
                    DeleteAnnonce(currentAnn);
                else
                    currentAnn.updated = false;
            }
        }

        public void ClearNotifsOfCours(Cours coursToClear)
        {
            foreach (CL_Notification currentNot in _notifByCours[coursToClear.sysCode])
            {
                if (!currentNot.updated)
                    DeleteNotification(currentNot);
                else
                    currentNot.updated = false;
            }
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }

}