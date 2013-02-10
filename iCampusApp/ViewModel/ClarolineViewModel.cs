using ClarolineApp.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Data.Linq;
using System.Windows;
using System.Collections.ObjectModel;

namespace ClarolineApp.ViewModel
{
    abstract class ClarolineViewModel : IClarolineViewModel
    {

        private ClarolineDataContext ClarolineDB;

        public ClarolineViewModel(string DBConnectionString)
        {
            ClarolineDB = new ClarolineDataContext(DBConnectionString);
        }

        public void ResetDatabase()
        {
            List<CL_Document> AllDocuments = (from CL_Document d
                                                in ClarolineDB.Documents_Table
                                              select d).ToList();
            List<CL_Annonce> AllAnnonces = (from CL_Annonce a
                                            in ClarolineDB.Annonces_Table
                                            select a).ToList();
            List<CL_Notification> AllNotifications = (from CL_Notification n
                                                      in ClarolineDB.Notifications_Table
                                                      select n).ToList();
            List<Cours> AllCours = (from Cours c
                                    in ClarolineDB.Cours_Table
                                    select c).ToList();

            ClarolineDB.Documents_Table.DeleteAllOnSubmit(AllDocuments);
            ClarolineDB.Annonces_Table.DeleteAllOnSubmit(AllAnnonces);
            ClarolineDB.Notifications_Table.DeleteAllOnSubmit(AllNotifications);
            ClarolineDB.Cours_Table.DeleteAllOnSubmit(AllCours);
            SaveChangesToDB();
        }

        public void SaveChangesToDB()
        {
            ClarolineDB.SubmitChanges();
        }

        public void AddCours(Cours newCours)
        {
            bool alreadyInDB = ClarolineDB.Cours_Table.Contains(newCours);

            if (!alreadyInDB)
            {
                newCours.updated = true;
                newCours.loaded = DateTime.Now;
                ClarolineDB.Cours_Table.InsertOnSubmit(newCours);
                SaveChangesToDB();
            }
            else
            {
                Cours coursInDb = (from Cours _cours in ClarolineDB.Cours_Table
                                   where _cours.Equals(newCours)
                                   select _cours).First();

                coursInDb.updated = true;
                coursInDb.loaded = DateTime.Now;
                SaveChangesToDB();
            }

            foreach (ResourceList list in newCours.Resources)
            {
                AddResourceList(list);
            }
        }

        public void AddResourceList(ResourceList newList)
        {
            bool alreadyInDB = ClarolineDB.ResourceList_Table.Contains(newList);

            if (!alreadyInDB)
            {
                newList.loaded = DateTime.Now;
                newList.updated = true;
                ClarolineDB.ResourceList_Table.InsertOnSubmit(newList);
                SaveChangesToDB();
            }
            else
            {
                ResourceList listFromDB = (from ResourceList rl
                                           in ClarolineDB.ResourceList_Table
                                           where rl.Equals(newList)
                                           select rl).First();
                listFromDB.updated = true;
                listFromDB.loaded = DateTime.Now;
                listFromDB.name = newList.name;
                listFromDB.visibility = newList.visibility;
                SaveChangesToDB();
            }
        }

        public void AddAnnonce(CL_Annonce newAnn)
        {
            bool alreadyInDB = ClarolineDB.Annonces_Table.Contains(newAnn);

            if (!alreadyInDB)
            {
                newAnn.updated = true;
                newAnn.loaded = DateTime.Now;
                ClarolineDB.Annonces_Table.InsertOnSubmit(newAnn);
                SaveChangesToDB();
            }
            else
            {
                CL_Annonce annFromDB = (from CL_Annonce a
                                        in ClarolineDB.Annonces_Table
                                        where a.Equals(newAnn)
                                        select a).First();

                annFromDB.notifiedDate = newAnn.notifiedDate;
                annFromDB.updated = true;
                annFromDB.loaded = DateTime.Now;
                if (annFromDB.date.CompareTo(newAnn.date) < 0)
                {
                    annFromDB.date = newAnn.date;
                }
                annFromDB.content = newAnn.content;
                SaveChangesToDB();
            }

            AddNotification(CL_Notification.CreateNotification(newAnn, alreadyInDB));
        }

        public void AddDocument(CL_Document newDoc)
        {
            bool alreadyInDB = ClarolineDB.Documents_Table.Contains(newDoc);

            if (!alreadyInDB)
            {
                newDoc.updated = true;
                newDoc.loaded = DateTime.Now;
                ClarolineDB.Documents_Table.InsertOnSubmit(newDoc);
                SaveChangesToDB();
            }
            else
            {
                CL_Document docFromDB = (from CL_Document d
                                         in ClarolineDB.Documents_Table
                                         where d.Equals(newDoc)
                                         select d).First();

                docFromDB.notifiedDate = newDoc.notifiedDate;
                docFromDB.date = newDoc.date;
                docFromDB.updated = true;
                docFromDB.loaded = DateTime.Now;

                SaveChangesToDB();
            }

            if (!newDoc.isFolder)
            {
                AddNotification(CL_Notification.CreateNotification(newDoc, alreadyInDB));
            }
        }

        public void AddNotification(CL_Notification newNot)
        {
            bool alreadyInDB = ClarolineDB.Notifications_Table.Contains(newNot);

            if (alreadyInDB)
            {
                CL_Notification lastNotificationFromDB = (from CL_Notification n
                                                          in ClarolineDB.Notifications_Table
                                                          where n.Equals(newNot)
                                                          orderby n.date descending
                                                          select n).First();
                if (lastNotificationFromDB.date.Date.Equals(newNot.date.Date))
                {
                    return;
                }
            }

            ClarolineDB.Notifications_Table.InsertOnSubmit(newNot);
            SaveChangesToDB();
        }

        public void DeleteCours(Cours coursForDelete)
        {
            // Remove the cours item from the "all" observable collection.
            var queryList = from ResourceList rl
                            in ClarolineDB.ResourceList_Table
                            where rl._coursId == coursForDelete.Id
                            select rl;
            foreach (var rl in queryList)
            {
                DeleteResourceList(rl);
            }

            var queryNot = from CL_Notification n in ClarolineDB.Notifications_Table
                           where n._coursId == coursForDelete.Id
                           select n;
            foreach (var not in queryNot)
            {
                DeleteNotification(not);
            }

            ClarolineDB.Cours_Table.DeleteOnSubmit(coursForDelete);

            SaveChangesToDB();
        }

        public void DeleteResourceList(ResourceList listForDelete)
        {
            Type resourceType = listForDelete.Resources.First().GetType();

            foreach (ResourceModel item in listForDelete.Resources)
            {
                if (resourceType.Equals(typeof(CL_Annonce)))
                {
                    DeleteAnnonce(item as CL_Annonce);
                }
                else if (resourceType.Equals(typeof(CL_Document)))
                {
                    DeleteDocument(item as CL_Document);
                }
            }

            ClarolineDB.ResourceList_Table.DeleteOnSubmit(listForDelete);
            SaveChangesToDB();
        }

        public void DeleteAnnonce(CL_Annonce annForDelete)
        {

            var queryNot = from CL_Notification n in ClarolineDB.Notifications_Table
                           where n.resource.Equals(annForDelete)
                           select n;
            foreach (var not in queryNot)
            {
                DeleteNotification(not);
            }

            ClarolineDB.Annonces_Table.DeleteOnSubmit(annForDelete);
            SaveChangesToDB();
        }

        public void DeleteDocument(CL_Document docForDelete)
        {
            var queryNot = from CL_Notification n in ClarolineDB.Notifications_Table
                           where n.resource.Equals(docForDelete)
                           select n;
            foreach (var not in queryNot)
            {
                DeleteNotification(not);
            }

            foreach (CL_Document inner in docForDelete.getContent())
            {
                DeleteDocument(inner);
            }

            ClarolineDB.Documents_Table.DeleteOnSubmit(docForDelete);
            SaveChangesToDB();
        }

        public void DeleteNotification(CL_Notification notForDelete)
        {
            ClarolineDB.Notifications_Table.DeleteOnSubmit(notForDelete);
            SaveChangesToDB();
        }

        public void ClearCoursList()
        {
            (from Cours c
             in ClarolineDB.Cours_Table
             select c).ToList()
             .ForEach(c =>
             {
                 if (c.updated)
                 {
                     c.updated = false;
                 }
                 else
                 {
                     DeleteCours(c);
                 }
             });
        }

        public void ClearDocsOfCours(Cours coursToClear)
        {
            (from CL_Document d
             in ClarolineDB.Documents_Table
             where d.resourceList.Cours.Equals(coursToClear)
             select d).ToList()
             .ForEach(d =>
             {
                 if (d.updated)
                 {
                     d.updated = false;
                 }
                 else
                 {
                     DeleteDocument(d);
                 }
             });
        }

        public void ClearAnnsOfCours(Cours coursToClear)
        {
            (from CL_Annonce a
             in ClarolineDB.Annonces_Table
             where a.resourceList.Cours.Equals(coursToClear)
             select a).ToList()
             .ForEach(a =>
             {
                 if (a.updated)
                 {
                     a.updated = false;
                 }
                 else
                 {
                     DeleteAnnonce(a);
                 }
             });
        }

        public void ClearNotifsOfCours(Cours coursToClear, int keeped)
        {
            (from CL_Notification n
             in ClarolineDB.Notifications_Table
             where n.Cours.Equals(coursToClear)
             orderby n.date descending
             select n).Skip(keeped).ToList()
             .ForEach(n => DeleteNotification(n) );
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                Deployment.Current.Dispatcher.BeginInvoke(() =>
                {
                    PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
                });
            }
        }
        #endregion

        public abstract void LoadCollectionsFromDatabase();

        public abstract void RefreshAsync();
    }
}
