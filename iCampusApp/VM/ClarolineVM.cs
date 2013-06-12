using ClarolineApp.Model;
using ClarolineApp.Settings;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ClarolineApp.VM
{
    public class ClarolineVM : IClarolineVM
    {
        public string ApplicationName
        {
            get
            {
                if (DesignerProperties.IsInDesignTool)
                {
                    return "Claromobile";
                }
                else
                {
                    return AppSettings.Instance.PlatformSetting;
                }
            }
        }

        private static ClarolineDataContext _db;
        protected static ClarolineDataContext ClarolineDB
        {
            get
            {
                if (_db == null)
                {
                    _db = new ClarolineDataContext(ClarolineDataContext.DBConnectionString);
                }
                return _db;
            }
            set
            {
                if (value != _db)
                {
                    _db = value;
                }
            }
        }

        public static AppSettings Settings
        {
            get
            {
                return AppSettings.Instance;
            }
        }

        public static ClaroClient Client
        {
            get
            {
                return ClaroClient.Instance;
            }
        }

        public bool IsConnected
        {
            get
            {
                return Settings.UserSetting != null && Settings.UserSetting.firstName != "";
            }
        }

        public ClarolineVM()
        {
            LoadCollectionsFromDatabase();

            Settings.PropertyChanged += (sender, e) =>
            {
                RaisePropertyChanged("IsConnected");

                if (e.PropertyName == AppSettings.PlatformSettingKeyName)
                {
                    RaisePropertyChanged("ApplicationName");
                }
            };
        }

        private Uri _navigationTarget;

        public Uri NavigationTarget
        {
            get
            {
                return _navigationTarget;
            }
            set
            {
                if (!value.Equals(_navigationTarget))
                {
                    _navigationTarget = value;
                    if (_navigationTarget != null)
                    {
                        RaisePropertyChanged("NavigationTarget");
                    }
                }
            }
        }

        public Uri GetNavigationTarget()
        {
            return NavigationTarget;
        }

        public virtual void ResetViewModel()
        {
            List<ResourceModel> AllResources = (from ResourceModel d
                                                in ClarolineDB.Resources_Table
                                                select d).ToList();

            List<Notification> AllNotifications = (from Notification n
                                                      in ClarolineDB.Notifications_Table
                                                      select n).ToList();

            List<ResourceList> AllLists = (from ResourceList l
                                           in ClarolineDB.ResourceList_Table
                                           select l).ToList();

            List<Cours> AllCours = (from Cours c
                                    in ClarolineDB.Cours_Table
                                    select c).ToList();

            ClarolineDB.Resources_Table.DeleteAllOnSubmit(AllResources);
            ClarolineDB.ResourceList_Table.DeleteAllOnSubmit(AllLists);
            ClarolineDB.Notifications_Table.DeleteAllOnSubmit(AllNotifications);
            ClarolineDB.Cours_Table.DeleteAllOnSubmit(AllCours);
            SaveChangesToDB();

            Client.InvalidateClient();

            Settings.Reset();
        }

        public void SaveChangesToDB()
        {
            ClarolineDB.SubmitChanges();
        }

        public virtual void AddCours(Cours newCours)
        {
            Cours coursInDb = ClarolineDB.Cours_Table.FirstOrDefault(c => c.sysCode == newCours.sysCode);

            if (coursInDb == null)
            {
                newCours.updated = true;
                newCours.loaded = DateTime.Now;
                ClarolineDB.Cours_Table.InsertOnSubmit(newCours);
                SaveChangesToDB();

                newCours.Id = ClarolineDB.Cours_Table.FirstOrDefault(c => c.sysCode == newCours.sysCode).Id;
            }
            else
            {
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
            bool alreadyInDB = ClarolineDB.ResourceList_Table.Any(l => l._coursId == newList._coursId
                                                                    && l.label == newList.label);

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
                                           where rl.Id == newList.Id
                                           select rl).First();
                listFromDB.updated = true;
                listFromDB.loaded = DateTime.Now;
                listFromDB.name = newList.name;
                listFromDB.visibility = newList.visibility;
                SaveChangesToDB();
            }
        }

        public virtual void AddNotification(Notification newNot)
        {
            bool alreadyInDB = ClarolineDB.Notifications_Table.Any(n => n._coursId == newNot._coursId
                                                                     && n._resourceId == newNot._resourceId);

            if (alreadyInDB)
            {
                Notification lastNotificationFromDB = (from Notification n
                                                          in ClarolineDB.Notifications_Table
                                                          where n._resourceId == newNot._resourceId
                                                          orderby n.date descending
                                                          select n).First();
                if (lastNotificationFromDB.date.CompareTo(newNot.date) >= 0)
                {
                    return;
                }
            }

            ClarolineDB.Notifications_Table.InsertOnSubmit(newNot);
            SaveChangesToDB();
        }

        public virtual void AddResource(ResourceModel newRes)
        {
            bool alreadyInDB = ClarolineDB.Resources_Table.Any(r => r.DiscKey == newRes.DiscKey
                                                                 && r.resourceId == newRes.resourceId
                                                                 && r.ResourceList.Id == newRes.ResourceList.Id);

            if (!alreadyInDB)
            {
                newRes.updated = true;
                newRes.loaded = DateTime.Now;
                ClarolineDB.Resources_Table.InsertOnSubmit(newRes);
                SaveChangesToDB();
            }
            else
            {
                newRes.updated = true;
                newRes.loaded = DateTime.Now;

                SaveChangesToDB();
            }

            if (!(newRes is Document) || !(newRes as Document).isFolder)
            {
                AddNotification(Notification.CreateNotification(newRes, alreadyInDB));
            }
        }

        public virtual void DeleteCours(Cours coursForDelete)
        {
            // Remove the cours item from the "all" observable collection.
            var queryList = from ResourceList rl
                            in ClarolineDB.ResourceList_Table
                            where rl.Cours.Equals(coursForDelete)
                            select rl;
            foreach (var rl in queryList)
            {
                DeleteResourceList(rl);
            }

            var queryNot = from Notification n in ClarolineDB.Notifications_Table
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
            var list = listForDelete.Resources.ToList();

            foreach (ResourceModel item in list)
            {
                DeleteResource(item);
            }

            ClarolineDB.ResourceList_Table.DeleteOnSubmit(listForDelete);
            SaveChangesToDB();
        }

        public virtual void DeleteResource(ResourceModel resForDelete)
        {

            var queryNot = from Notification n in ClarolineDB.Notifications_Table
                           where n.resource.Equals(resForDelete)
                           select n;
            foreach (var not in queryNot)
            {
                DeleteNotification(not);
            }

            foreach (var subres in resForDelete.GetSubRes())
            {
                DeleteResource(subres);
            }

            ClarolineDB.Resources_Table.DeleteOnSubmit(resForDelete);
            SaveChangesToDB();
        }

        public virtual void DeleteNotification(Notification notForDelete)
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

        public void ClearResOfCours(Cours coursToClear)
        {
            (from ResourceModel rm
             in ClarolineDB.Resources_Table
             where rm.ResourceList.Cours.Equals(coursToClear)
             select rm).ToList()
             .ForEach(rm =>
             {
                 if (rm.GetSubRes().Count == 0)
                 {
                     if (rm.updated)
                     {
                         rm.updated = false;
                     }
                     else
                     {
                         DeleteResource(rm);
                     }
                 }
             });
            SaveChangesToDB();
        }

        public void ClearNotifsOfCours(Cours coursToClear, int keeped)
        {
            (from Notification n
             in ClarolineDB.Notifications_Table
             where n.Cours.Equals(coursToClear)
             orderby n.date descending
             select n).Skip(keeped).ToList()
             .ForEach(n => DeleteNotification(n));
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.

        protected void RaisePropertyChanged(string propertyName)
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
        protected const double UpdateDelay = 1.0;

        public virtual async Task RefreshAsync(bool force = false)
        {
            if (force || _lastClientCall.AddHours(UpdateDelay).CompareTo(DateTime.Now) < 0)
            {
                String updates = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.USER, SupportedMethods.GetUpdates);
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
                                    foreach (KeyValuePair<String, Dictionary<String, String>> resource in tool.Value)
                                    {
                                        ResourceList rlist = upCours.Resources.First(rl => rl.label.Equals(tool.Key));
                                        ResourceModel upRes = rlist.GetResourceByResId(resource.Key);

                                        if (upRes == null)
                                        {
                                            await GetSingleResourceAsync(rlist, resource.Key);
                                        }
                                        else
                                        {
                                            upRes.date = DateTime.Parse(resource.Value["date"]);
                                            upRes.notifiedDate = DateTime.Now;
                                            SaveChangesToDB();
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

            foreach (ResourceList rl in (from ResourceList l
                                         in ClarolineDB.ResourceList_Table
                                         where l.Cours.Equals(coursToPrepare)
                                         select l))
            {
                if (Enum.IsDefined(typeof(SupportedModules), rl.GetSupportedModule()))
                {
                    await GetResourcesForThisListAsync(rl);
                }
            }
            ClearResOfCours(coursToPrepare);

            coursToPrepare.Resources.AddRange(from ResourceList l in ClarolineDB.ResourceList_Table
                                              where l.Cours.Equals(coursToPrepare)
                                              select l);
        }

        public async Task GetResourcesForThisListAsync(ResourceList container)
        {
            String strContent = await ClaroClient.Instance.MakeOperationAsync(container.GetSupportedModule(),
                                                                              SupportedMethods.GetResourcesList,
                                                                              reqCours:container.Cours,
                                                                              genMod:container.label);

            IList resources = (IList)JsonConvert.DeserializeObject(strContent, container.ressourceListType);

            foreach (ResourceModel item in resources)
            {
                item.ResourceList = container;
                AddResource(item);
            }
        }

        public async Task GetSingleResourceAsync(ResourceList container, string resourceString = null)
        {
            String strContent = await ClaroClient.Instance.MakeOperationAsync(container.GetSupportedModule(),
                                                                              SupportedMethods.GetSingleResource,
                                                                              container.Cours,
                                                                              resourceString);

            ResourceModel resource = (ResourceModel)JsonConvert.DeserializeObject(strContent, container.ressourceType);
            AddResource(resource);
        }

        public async Task GetUserDataAsync()
        {
            String strContent = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.USER,
                                                                              SupportedMethods.GetUserData);

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

            AppSettings.Instance.UserSetting.setUser(connectedUser);
            AppSettings.Instance.InstituteSetting = institution;
            AppSettings.Instance.PlatformSetting = platform;
        }

        public async Task GetCoursListAsync()
        {
            String strContent = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.USER,
                                                                              SupportedMethods.GetCourseList);

            List<Cours> Courses = JsonConvert.DeserializeObject<List<Cours>>(strContent);

            foreach (Cours cours in Courses)
            {
                AddCours(cours);
            }

            ClearCoursList();
        }

        public async Task GetResourcesListForThisCoursAsync(Cours cours)
        {
            String strContent = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.USER,
                                                                              SupportedMethods.GetToolList,
                                                                              cours);

            List<ResourceList> rl = JsonConvert.DeserializeObject<List<ResourceList>>(strContent);
            foreach (ResourceList item in rl)
            {
                switch (item.label)
                {
                    case Annonce.Label:
                        item.ressourceType = typeof(Annonce);
                        break;
                    case Document.Label:
                        item.ressourceType = typeof(Document);
                        break;
                    case Description.Label:
                        item.ressourceType = typeof(Description);
                        break;
                    case Event.Label:
                        item.ressourceType = typeof(Event);
                        break;
                    default:
                        item.ressourceType = typeof(ResourceModel);
                        break;
                }

                item.Cours = cours;

                AddResourceList(item);
            }
        }
    }
}
