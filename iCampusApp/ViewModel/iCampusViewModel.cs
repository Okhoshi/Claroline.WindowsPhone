using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

// Directive for the data model.
using ClarolineApp.Model;

namespace ClarolineApp.ViewModel
{
    public class iCampusViewModel : INotifyPropertyChanged
    {
        // LINQ to SQL data context for the local database.

        private iCampusDataContext iCampusDB;

        // Class constructor, create the data context object.

        public iCampusViewModel(string DBConnectionString)
        {
            iCampusDB = new iCampusDataContext(DBConnectionString);
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
        
        private ObservableCollection<Documents> _allFiles;
        public ObservableCollection<Documents> AllFiles
        {
            get { return _allFiles; }
            set
            {
                _allFiles = value;
                NotifyPropertyChanged("AllFiles");
            }
        }

        private ObservableCollection<Documents> _allFolders;
        public ObservableCollection<Documents> AllFolders
        {
            get { return _allFolders; }
            set
            {
                _allFolders = value;
                NotifyPropertyChanged("AllFolders");
            }
        }

        private Dictionary<string, ObservableCollection<Documents>> _docByCours;
        public Dictionary<string, ObservableCollection<Documents>> DocByCours
        {
            get { return _docByCours; }
            set
            {
                _docByCours = value;
                NotifyPropertyChanged("DocByCours");
            }
        }

        private ObservableCollection<Annonce> _allAnnonces;
        public ObservableCollection<Annonce> AllAnnonces
        {
            get { return _allAnnonces; }
            set
            {
                _allAnnonces = value;
                NotifyPropertyChanged("AllAnnonces");
            }
        }

        private Dictionary<string, ObservableCollection<Annonce>> _annByCours;
        public Dictionary<string, ObservableCollection<Annonce>> AnnByCours
        {
            get { return _annByCours; }
            set
            {
                _annByCours = value;
                NotifyPropertyChanged("AnnByCours");
            }
        }

        private ObservableCollection<Notification> _allNotifications;
        public ObservableCollection<Notification> AllNotifications
        {
            get { return _allNotifications; }
            set
            {
                _allNotifications = value;
                NotifyPropertyChanged("AllNotifications");
            }
        }

        private Dictionary<string, ObservableCollection<Notification>> _notifByCours;
        public Dictionary<string, ObservableCollection<Notification>> NotifByCours
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
            iCampusDB.SubmitChanges();
        }

        // Query database and load the collections and list used by the pivot pages.

        public void LoadCollectionsFromDatabase()
        {

            // Specify the query for all courses in the database.

            var coursInDB = from Cours cours in iCampusDB.Cours_T
                            select cours;

            // Query the database and load all to-do items.

            AllCours = new ObservableCollection<Cours>(coursInDB);

            // Specify the query for all section in the database.

            var filesInDb = from Documents file in iCampusDB.Documents
                            where !file.IsFolder
                            select file;

            AllFiles = new ObservableCollection<Documents>(filesInDb);


            var foldersInDb = from Documents fold in iCampusDB.Documents
                              where fold.IsFolder
                              select fold;
            AllFolders = new ObservableCollection<Documents>(foldersInDb);

            var annsInDb = from Annonce ann in iCampusDB.Annonces
                              select ann;
            AllAnnonces = new ObservableCollection<Annonce>(annsInDb);

            var notifsInDb = (from Notification not in iCampusDB.Notifications
                             orderby not.date descending
                             select not).Take(1000);
            AllNotifications = new ObservableCollection<Notification>(notifsInDb);

            // Query the database and load all associated items to their respective collections.
            DocByCours = new Dictionary<string, ObservableCollection<Documents>>();
            AnnByCours = new Dictionary<string, ObservableCollection<Annonce>>();
            NotifByCours = new Dictionary<string, ObservableCollection<Notification>>();

            foreach (Cours _cours in coursInDB)
            {
                DocByCours.Add(_cours.sysCode, new ObservableCollection<Documents>());
                AnnByCours.Add(_cours.sysCode, new ObservableCollection<Annonce>());
                NotifByCours.Add(_cours.sysCode, new ObservableCollection<Notification>());

                foreach(Documents doc in _cours.Documents)
                    DocByCours[_cours.sysCode].Add(doc);
                
                foreach(Annonce ann in _cours.Annonces)
                    AnnByCours[_cours.sysCode].Add(ann);

                foreach (Notification not in _cours.Notifications)
                    NotifByCours[_cours.sysCode].Add(not);
            }
        }

        public void ResetDatabase()
        {
            iCampusDB.Documents.DeleteAllOnSubmit(AllFiles);
            iCampusDB.Documents.DeleteAllOnSubmit(AllFolders);
            iCampusDB.Annonces.DeleteAllOnSubmit(AllAnnonces);
            iCampusDB.Notifications.DeleteAllOnSubmit(AllNotifications);
            iCampusDB.Cours_T.DeleteAllOnSubmit(AllCours);
            iCampusDB.SubmitChanges();
            _allFiles.Clear();
            _allFolders.Clear();
            _allAnnonces.Clear();
            _allNotifications.Clear();
            _allCours.Clear();
            _docByCours.Clear();
            _annByCours.Clear();
            _notifByCours.Clear();
        }

        public void AddDocument(Documents newDoc)
        {
            newDoc.Updated = true;
            newDoc.Cours.notified = true;
            UpdateCours(newDoc.Cours);

            if (newDoc.IsFolder)
            {
                if (!AllFolders.Contains(newDoc))
                {
                    // Add a to-do item to the data context.

                    iCampusDB.Documents.InsertOnSubmit(newDoc);

                    // Save changes to the database.

                    iCampusDB.SubmitChanges();

                    // Add a to-do item to the "all" observable collection.

                    AllFolders.Add(newDoc);

                    // Add a to-do item to the appropriate filtered collection.

                    DocByCours[newDoc.Cours.sysCode].Add(newDoc);
                }
                else
                    UpdateFolder(newDoc);
            }
            else
            {
                if (!AllFiles.Contains(newDoc))
                {
                    // Add a to-do item to the data context.

                    iCampusDB.Documents.InsertOnSubmit(newDoc);
                    // Save changes to the database.

                    iCampusDB.SubmitChanges();

                    // Add a to-do item to the "all" observable collection.

                    AllFiles.Add(newDoc);

                    // Add a to-do item to the appropriate filtered collection.

                    DocByCours[newDoc.Cours.sysCode].Add(newDoc);

                    AddNotification(Notification.CreateNotification(newDoc, false));

                }
                else
                    UpdateFile(newDoc);
            }
        }

        private void UpdateFolder(Documents newDoc)
        {
            Documents foldInDb = (from Documents _fold in iCampusDB.Documents
                                  where _fold.Equals(AllFolders[AllFolders.IndexOf(newDoc)])
                                  select _fold).First();

            AllFolders[AllFolders.IndexOf(foldInDb)].notified = newDoc.notified;
            DocByCours[foldInDb.Cours.sysCode][DocByCours[foldInDb.Cours.sysCode].IndexOf(foldInDb)].notified = newDoc.notified;
            foldInDb.notified = newDoc.notified;
            AllFolders[AllFolders.IndexOf(foldInDb)].date = newDoc.date;
            DocByCours[foldInDb.Cours.sysCode][DocByCours[foldInDb.Cours.sysCode].IndexOf(foldInDb)].date = newDoc.date;
            foldInDb.date = newDoc.date;
            AllFolders[AllFolders.IndexOf(foldInDb)].Updated = newDoc.Updated;
            DocByCours[foldInDb.Cours.sysCode][DocByCours[foldInDb.Cours.sysCode].IndexOf(foldInDb)].Updated = newDoc.Updated;
            foldInDb.Updated = newDoc.Updated;
            iCampusDB.SubmitChanges();

        }

        private void UpdateFile(Documents newFile)
        {
            Documents fileInDb = (from Documents _file in iCampusDB.Documents
                                  where _file.Equals(AllFiles[AllFiles.IndexOf(newFile)])
                             select _file).First();

            AllFiles[AllFiles.IndexOf(fileInDb)].notified = newFile.notified;
            DocByCours[fileInDb.Cours.sysCode][DocByCours[fileInDb.Cours.sysCode].IndexOf(fileInDb)].notified = newFile.notified;
            fileInDb.notified = newFile.notified;
            AllFiles[AllFiles.IndexOf(fileInDb)].date = newFile.date;
            DocByCours[fileInDb.Cours.sysCode][DocByCours[fileInDb.Cours.sysCode].IndexOf(fileInDb)].date = newFile.date;
            fileInDb.date = newFile.date;
            AllFiles[AllFiles.IndexOf(fileInDb)].Updated = newFile.Updated;
            DocByCours[fileInDb.Cours.sysCode][DocByCours[fileInDb.Cours.sysCode].IndexOf(fileInDb)].Updated = newFile.Updated;
            fileInDb.Updated = newFile.Updated;
            AllFiles[AllFiles.IndexOf(fileInDb)].date = newFile.date;
            DocByCours[fileInDb.Cours.sysCode][DocByCours[fileInDb.Cours.sysCode].IndexOf(fileInDb)].date = newFile.date;
            fileInDb.date = newFile.date;
            iCampusDB.SubmitChanges();

            AddNotification(Notification.CreateNotification(fileInDb, true));
        }

        public void DeleteFolder(Documents docForDelete)
        {
            if (docForDelete.IsFolder)
            {
                // Remove the section item from the "all" observable collection.

                AllFolders.Remove(docForDelete);

                foreach (Documents DocForDelete in docForDelete.getContent())
                {
                    DeleteFolder(DocForDelete);
                }
                // Remove the section item from the data context.

                iCampusDB.Documents.DeleteOnSubmit(docForDelete);

                // Remove the section item from the appropriate category.   

                DocByCours[docForDelete.Cours.sysCode].Remove(docForDelete);
            }
            else
            {
                // Remove the section item from the "all" observable collection.

                AllFiles.Remove(docForDelete);
                
                // Remove the section item from the data context.

                iCampusDB.Documents.DeleteOnSubmit(docForDelete);

                // Remove the section item from the appropriate category.   

                DocByCours[docForDelete.Cours.sysCode].Remove(docForDelete);
            }
            // Save changes to the database.

            iCampusDB.SubmitChanges();
        }

        public void AddAnnonce(Annonce newAnn)
        {
            newAnn.Updated = true;
            newAnn.Cours.notified = true;
            UpdateCours(newAnn.Cours);

            if (!AllAnnonces.Contains(newAnn))
            {
                // Add a to-do item to the data context.

                iCampusDB.Annonces.InsertOnSubmit(newAnn);

                // Save changes to the database.

                iCampusDB.SubmitChanges();

                // Add a to-do item to the "all" observable collection.

                AllAnnonces.Add(newAnn);

                AnnByCours[newAnn.Cours.sysCode].Add(newAnn);

                AddNotification(Notification.CreateNotification(newAnn, false));
            }
            else
                UpdateAnn(newAnn);
        }

        private void UpdateAnn(Annonce newAnn)
        {
            Annonce annInDb = (from Annonce _ann in iCampusDB.Annonces
                               where _ann.Equals(AllAnnonces[AllAnnonces.IndexOf(newAnn)])
                               select _ann).First();

            annInDb.notified = newAnn.notified;
            AnnByCours[annInDb.Cours.sysCode][AnnByCours[annInDb.Cours.sysCode].IndexOf(annInDb)].notified = newAnn.notified;
            AllAnnonces[AllAnnonces.IndexOf(annInDb)].notified = newAnn.notified;
            annInDb.Updated = newAnn.Updated;
            AnnByCours[annInDb.Cours.sysCode][AnnByCours[annInDb.Cours.sysCode].IndexOf(annInDb)].Updated = newAnn.Updated;
            AllAnnonces[AllAnnonces.IndexOf(annInDb)].Updated = newAnn.Updated;
            if (annInDb.date.CompareTo(newAnn.date) < 0)
            {
                annInDb.date = newAnn.date;
                AnnByCours[annInDb.Cours.sysCode][AnnByCours[annInDb.Cours.sysCode].IndexOf(annInDb)].date = newAnn.date;
                AllAnnonces[AllAnnonces.IndexOf(annInDb)].date = newAnn.date;
            }
            annInDb.content = newAnn.content;
            AnnByCours[annInDb.Cours.sysCode][AnnByCours[annInDb.Cours.sysCode].IndexOf(annInDb)].content = newAnn.content;
            AllAnnonces[AllAnnonces.IndexOf(annInDb)].content = newAnn.content;
            iCampusDB.SubmitChanges();

            AddNotification(Notification.CreateNotification(annInDb, true));
        }

        public void DeleteAnnonce(Annonce annForDelete)
        {

            // Remove the cours item from the "all" observable collection.

            AllAnnonces.Remove(annForDelete);

            AnnByCours[annForDelete.Cours.sysCode].Remove(annForDelete);

            // Remove the cours item from the data context.

            iCampusDB.Annonces.DeleteOnSubmit(annForDelete);

            // Save changes to the database.

            iCampusDB.SubmitChanges();
        }

        public void AddNotification(Notification newNot)
        {
            newNot.Updated = true;
            newNot.Cours.notified = true;
            UpdateCours(newNot.Cours);

            if (AllNotifications.Contains(newNot))
            {
                Notification not = (from Notification not_ in AllNotifications
                                    where not_.Equals(newNot)
                                    orderby not_.date descending
                                    select not_).First();
                if (not.date == newNot.date)
                    return;
            }
                // Add a to-do item to the data context.

                iCampusDB.Notifications.InsertOnSubmit(newNot);

                // Save changes to the database.

                iCampusDB.SubmitChanges();

                // Add a to-do item to the "all" observable collection.

                AllNotifications.Add(newNot);

                NotifByCours[newNot.Cours.sysCode].Add(newNot);
            /*}
            else
                UpdateNot(newNot);*/
        }

        private void UpdateNot(Notification newNot)
        {
            Notification notInDb = (from Notification _not in iCampusDB.Notifications
                               where _not.Equals(newNot)
                               select _not).First();

            notInDb.notified = newNot.notified;
            NotifByCours[notInDb.Cours.sysCode][NotifByCours[notInDb.Cours.sysCode].IndexOf(notInDb)].notified = newNot.notified;
            AllNotifications[AllNotifications.IndexOf(notInDb)].notified = newNot.notified;
            notInDb.Updated = newNot.Updated;
            NotifByCours[notInDb.Cours.sysCode][NotifByCours[notInDb.Cours.sysCode].IndexOf(notInDb)].Updated = newNot.Updated;
            AllNotifications[AllNotifications.IndexOf(notInDb)].Updated = newNot.Updated;
            iCampusDB.SubmitChanges();
        }

        public void DeleteNotification(Notification notForDelete)
        {

            // Remove the cours item from the "all" observable collection.

            AllNotifications.Remove(notForDelete);

            NotifByCours[notForDelete.Cours.sysCode].Remove(notForDelete);

            // Remove the cours item from the data context.

            iCampusDB.Notifications.DeleteOnSubmit(notForDelete);

            // Save changes to the database.

            iCampusDB.SubmitChanges();
        }

        public void AddCours(Cours newCours)
        {
            newCours.Updated = true;

            if (!AllCours.Contains(newCours))
            {
                // Add a to-do item to the data context.

                iCampusDB.Cours_T.InsertOnSubmit(newCours);

                // Save changes to the database.

                iCampusDB.SubmitChanges();

                // Add a to-do item to the "all" observable collection.

                AllCours.Add(newCours);

                DocByCours.Add(newCours.sysCode, new ObservableCollection<Documents>());
                AnnByCours.Add(newCours.sysCode, new ObservableCollection<Annonce>());
                NotifByCours.Add(newCours.sysCode, new ObservableCollection<Notification>());
            }
            else
                UpdateCours(newCours);
        }

        private void UpdateCours(Cours newCours)
        {
            Cours coursInDb = (from Cours _cours in iCampusDB.Cours_T
                               where _cours.Equals(AllCours[AllCours.IndexOf(newCours)])
                               select _cours).First();

            coursInDb.notified = newCours.notified;
            coursInDb.Updated = newCours.Updated;
            iCampusDB.SubmitChanges();
            AllCours[AllCours.IndexOf(coursInDb)].notified = newCours.notified;
            AllCours[AllCours.IndexOf(coursInDb)].Updated = newCours.Updated;
        }

        public void DeleteCours(Cours coursForDelete)
        {
            // Remove the cours item from the "all" observable collection.
            var queryDoc = from Documents _doc in iCampusDB.Documents
                           where _doc._coursId == coursForDelete.Id
                           select _doc;
            foreach (var Doc in queryDoc)
            {
                //System.Diagnostics.Debug.WriteLine("Deleting document : " + Doc.Name);
                if (Doc.IsFolder)
                    AllFolders.Remove(Doc);
                else
                    AllFiles.Remove(Doc);
                iCampusDB.Documents.DeleteOnSubmit(Doc);
            }
            DocByCours.Remove(coursForDelete.sysCode);

            var queryAnn = from Annonce _ann in iCampusDB.Annonces
                        where _ann._coursId == coursForDelete.Id
                        select _ann;
            foreach (var Ann in queryAnn)
            {
                //System.Diagnostics.Debug.WriteLine("Deleting document : " + Ann.title);
                iCampusDB.Annonces.DeleteOnSubmit(Ann);
                AllAnnonces.Remove(Ann);
            }
            AnnByCours.Remove(coursForDelete.sysCode);

            var queryNot = from Notification _not in iCampusDB.Notifications
                           where _not._coursId == coursForDelete.Id
                           select _not;
            foreach (var not in queryNot)
            {
                iCampusDB.Notifications.DeleteOnSubmit(not);
                AllNotifications.Remove(not);
            }
            NotifByCours.Remove(coursForDelete.sysCode);

            // Remove the cours item from the data context.

            iCampusDB.Cours_T.DeleteOnSubmit(coursForDelete);

            // Save changes to the database.

            iCampusDB.SubmitChanges();
        }

        public void ClearCoursList()
        {
            ObservableCollection<Cours> toDel = new ObservableCollection<Cours>();

            foreach (Cours currentCours in _allCours)
            {
                if (!currentCours.Updated)
                {
                    DeleteCours(currentCours);
                    toDel.Add(currentCours);
                }
                else
                    currentCours.Updated = false;
            }

            foreach (Cours item in toDel)
            {
                _allCours.Remove(item);
            }
        }

        public void ClearDocsOfCours(Cours coursToClear)
        {
            foreach (Documents currentDoc in _docByCours[coursToClear.sysCode])
            {
                if (!currentDoc.Updated)
                    DeleteFolder(currentDoc);
                else
                    currentDoc.Updated = false;
            }
        }

        public void ClearAnnsOfCours(Cours coursToClear)
        {
            foreach (Annonce currentAnn in _annByCours[coursToClear.sysCode])
            {
                if (!currentAnn.Updated)
                    DeleteAnnonce(currentAnn);
                else
                    currentAnn.Updated = false;
            }
        }

        public void ClearNotifsOfCours(Cours coursToClear)
        {
            foreach (Notification currentNot in _notifByCours[coursToClear.sysCode])
            {
                if (!currentNot.Updated)
                    DeleteNotification(currentNot);
                else
                    currentNot.Updated = false;
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