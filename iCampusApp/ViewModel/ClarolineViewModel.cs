using ClarolineApp.Model;
using ClarolineApp.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClarolineApp.ViewModel
{
    class ClarolineViewModel : IClarolineViewModel
    {

        protected ClarolineDataContext ClarolineDB;

        public ClarolineViewModel(string DBConnectionString = ClarolineDataContext.DBConnectionString)
        {
            ClarolineDB = new ClarolineDataContext(DBConnectionString);
        }

        public virtual void ResetViewModel()
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

        public virtual void AddCours(Cours newCours)
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

        public virtual void AddResourceList(ResourceList newList)
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

        public virtual void AddAnnonce(CL_Annonce newAnn)
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

        public virtual void AddDocument(CL_Document newDoc)
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

        public virtual void AddNotification(CL_Notification newNot)
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

        public virtual void AddResource(ResourceModel newRes)
        {
            if (newRes.GetType().Equals(typeof(CL_Annonce)))
            {
                AddAnnonce((CL_Annonce) newRes);
            }
            else if (newRes.GetType().Equals(typeof(CL_Document)))
            {
                AddDocument((CL_Document)newRes);
            }
        }

        public virtual void DeleteCours(Cours coursForDelete)
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

        public virtual void DeleteResourceList(ResourceList listForDelete)
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

        public virtual void DeleteAnnonce(CL_Annonce annForDelete)
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

        public virtual void DeleteDocument(CL_Document docForDelete)
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

        public virtual void DeleteNotification(CL_Notification notForDelete)
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
             .ForEach(n => DeleteNotification(n));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.

        protected void NotifyPropertyChanged(string propertyName)
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

        public virtual void LoadCollectionsFromDatabase() { }

        protected DateTime _lastClientCall = DateTime.MinValue;

        //Delay between two updates in hours
        protected const double UPDATE_DELAY = 1.0;

        public virtual async Task RefreshAsync()
        {
            if (_lastClientCall.AddHours(UPDATE_DELAY).CompareTo(DateTime.Now) < 0)
            {
                String updates = await ClaroClient.instance.makeOperationAsync(SupportedModules.USER, SupportedMethods.getUpdates);
                if (!updates.Equals("[]"))
                {
                    Dictionary<String, Dictionary<String, Dictionary<String, Dictionary<String, String>>>> Updates;
                    Updates = JsonConvert.DeserializeObject<Dictionary<String, Dictionary<String, Dictionary<String, Dictionary<String, String>>>>>(updates);
                    foreach (KeyValuePair<String, Dictionary<String, Dictionary<String, Dictionary<String, String>>>> course in Updates)
                    {
                        Cours upCours = (from Cours c
                                        in ClarolineDB.Cours_Table
                                         where c.sysCode.Equals(course.Key)
                                         select c).FirstOrDefault();

                        if (upCours == null)
                        {
                            await GetCoursListAsync();

                            upCours = (from Cours c
                                        in ClarolineDB.Cours_Table
                                       where c.sysCode.Equals(course.Key)
                                       select c).FirstOrDefault();
                            if (upCours != null)
                            {
                                await PrepareCoursForOpeningAsync(upCours);
                            }
                            continue;
                        }
                        else
                        {
                            foreach (KeyValuePair<String, Dictionary<String, Dictionary<String, String>>> tool in course.Value)
                            {
                                if (!upCours.Resources.Any(rl => rl.label.Equals(tool.Key)))
                                {
                                    await GetResourcesListForThisCoursAsync(upCours);

                                    if (upCours.Resources.Any(rl => rl.label.Equals(tool.Key)))
                                    {
                                        await GetResourcesForThisListAsync(upCours.Resources.First(rl => rl.label.Equals(tool.Key)));
                                    }
                                    continue;
                                }
                                else
                                {
                                    foreach (KeyValuePair<String, Dictionary<String, String>> ressource in tool.Value)
                                    {
                                    ResourceList rlist = upCours.Resources.First(rl => rl.label.Equals(tool.Key));

                                        switch (tool.Key)
                                        {
                                            case "CLANN":
                                                CL_Annonce upAnn = (from CL_Annonce a
                                                                    in ClarolineDB.Annonces_Table
                                                                    where a.resourceId == int.Parse(ressource.Key)
                                                                    select a).FirstOrDefault();
                                                if (upAnn == null)
                                                {
                                                    await GetSingleResourceAsync(rlist, ressource.Key);
                                                }
                                                else
                                                {
                                                    upAnn.date = DateTime.Parse(ressource.Value["date"]);
                                                    upAnn.notifiedDate = DateTime.Now;
                                                    AddAnnonce(upAnn);
                                                }
                                                break;
                                            case "CLDOC":
                                                CL_Document upDoc = (from CL_Document d
                                                                     in ClarolineDB.Documents_Table
                                                                     where d.path.Equals(ressource.Key)
                                                                     select d).FirstOrDefault();

                                                if (upDoc == null)
                                                {
                                                    await GetSingleResourceAsync(rlist, ressource.Key);
                                                }
                                                else
                                                {
                                                    upDoc.date = DateTime.Parse(ressource.Value["date"]);
                                                    upDoc.notifiedDate = DateTime.Now;
                                                    AddDocument(upDoc);
                                                }
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            //Else
            return;
        }

        public async Task PrepareCoursForOpeningAsync(Cours coursToPrepare)
        {
            await GetResourcesListForThisCoursAsync(coursToPrepare);

            foreach (ResourceList rl in coursToPrepare.Resources)
            {
                await GetResourcesForThisListAsync(rl);
            }
        }

        public async Task GetResourcesForThisListAsync(ResourceList container)
        {
            String strContent = await ClaroClient.instance.makeOperationAsync(container.GetSupportedModule(),
                                                                              SupportedMethods.getResourcesList,
                                                                              container.Cours);

            List<ResourceModel> resources = (List<ResourceModel>)JsonConvert.DeserializeObject(strContent, container.ressourceListType);
            foreach (ResourceModel item in resources)
            {
                AddResource(item);
            }
        }

        public async Task GetSingleResourceAsync(ResourceList container, string resourceString = null)
        {
            String strContent = await ClaroClient.instance.makeOperationAsync(container.GetSupportedModule(),
                                                                              SupportedMethods.getSingleResource,
                                                                              container.Cours,
                                                                              resourceString);

            ResourceModel resource = (ResourceModel)JsonConvert.DeserializeObject(strContent, container.ressourceType);
            AddResource(resource);
        }

        public async Task GetUserDataAsync()
        {
            String strContent = await ClaroClient.instance.makeOperationAsync(SupportedModules.USER,
                                                                              SupportedMethods.getUserData);

            User connectedUser = JsonConvert.DeserializeObject<User>(strContent);
            StringReader str = new StringReader(strContent);
            String institution = "";
            String platform = "";

            JsonTextReader reader = new JsonTextReader(str);
            while (reader.Read())
            {
                if (reader.Value != null)
                {
                    switch (reader.Value.ToString())
                    {
                        case "institutionName":
                            institution = reader.ReadAsString();
                            break;
                        case "platformName":
                            platform = reader.ReadAsString();
                            break;
                        default:
                            continue;
                    }
                }
            }
            reader.Close();
            str.Close();

            AppSettings.instance.UserSetting.setUser(connectedUser);
            AppSettings.instance.IntituteSetting = institution;
            AppSettings.instance.PlatformSetting = platform;
        }

        public async Task GetCoursListAsync()
        {
            String strContent = await ClaroClient.instance.makeOperationAsync(SupportedModules.USER,
                                                                              SupportedMethods.getCourseList);

            List<Cours> Courses = JsonConvert.DeserializeObject<List<Cours>>(strContent);

            foreach (Cours cours in Courses)
            {
                AddCours(cours);
            }

            ClearCoursList();
        }

        public async Task GetResourcesListForThisCoursAsync(Cours cours)
        {
            String strContent = await ClaroClient.instance.makeOperationAsync(SupportedModules.USER,
                                                                              SupportedMethods.getToolList,
                                                                              cours);

            List<ResourceList> rl = JsonConvert.DeserializeObject<List<ResourceList>>(strContent);
            foreach (ResourceList item in rl)
            {
                switch (item.label)
                {
                    case CL_Annonce.LABEL:
                        item.ressourceType = typeof(CL_Annonce);
                        break;
                    case CL_Document.LABEL:
                        item.ressourceType = typeof(CL_Document);
                        break;
                }

                item.Cours = cours;

                AddResourceList(item);
            }
        }
    }
}
