using ClarolineApp.Model;
using ClarolineApp.Settings;
using GalaSoft.MvvmLight;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace ClarolineApp.VM
{
    public class ClarolineVM : ViewModelBase, IClarolineVM
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

        private ClarolineDataContext _db;
        protected ClarolineDataContext ClarolineDB
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

        private bool _isLoading;
        public bool IsLoading
        {
            get
            {
                return _isLoading;
            }
            private set
            {
                if (value != _isLoading)
                {
                    _isLoading = value;
                    RaisePropertyChanged("IsLoading");
                }
            }
        }

        public ClarolineVM()
        {
            LoadCollectionsFromDatabase();

            if (!DesignerProperties.IsInDesignTool)
            {
                Settings.PropertyChanged += (sender, e) =>
                {
                    RaisePropertyChanged("IsConnected");

                    if (e.PropertyName == AppSettings.PlatformSettingKeyName)
                    {
                        RaisePropertyChanged("ApplicationName");
                    }
                };
            }
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
                if (value == null || !value.Equals(_navigationTarget))
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

            ClarolineDB.Notifications_Table.DeleteAllOnSubmit(AllNotifications);
            ClarolineDB.Resources_Table.DeleteAllOnSubmit(AllResources);
            ClarolineDB.ResourceList_Table.DeleteAllOnSubmit(AllLists);
            ClarolineDB.Cours_Table.DeleteAllOnSubmit(AllCours);
            SaveChangesToDB();

            Client.InvalidateClient();

            Settings.Reset();
        }

        public void SaveChangesToDB()
        {
            try
            {
                ClarolineDB.SubmitChanges(ConflictMode.ContinueOnConflict);
            }
            catch (ChangeConflictException)
            {
                foreach (ObjectChangeConflict c in ClarolineDB.ChangeConflicts)
                {
                    c.Resolve(RefreshMode.KeepCurrentValues, true);
                }
                SaveChangesToDB();
            }
        }

        public virtual void AddCours(Cours newCours)
        {
            ClarolineDataContext cdc = ClarolineDB;
            //using (ClarolineDataContext cdc = new ClarolineDataContext(ClarolineDataContext.DBConnectionString))
            //{
            var q = cdc.Cours_Table.Where(c => c.sysCode == newCours.sysCode);

            newCours.updated = true;
            newCours.loaded = DateTime.Now;

            if (!q.Any())
            {
                cdc.Cours_Table.InsertOnSubmit(newCours);
                cdc.SubmitChanges();
                cdc.Refresh(RefreshMode.OverwriteCurrentValues, newCours);
            }
            else
            {
                q.First().UpdateFrom(newCours);
                newCours.Id = q.First().Id;
                cdc.SubmitChanges();
            }

            //}
        }

        public virtual void AddResourceList(ResourceList newList, int coursId)
        {
            ClarolineDataContext cdc = ClarolineDB;
            //using (ClarolineDataContext cdc = new ClarolineDataContext(ClarolineDataContext.DBConnectionString))
            //{
            var q = cdc.ResourceList_Table.Where(rl => rl._coursId == coursId && rl.label == newList.label);

            newList.updated = true;
            newList.loaded = DateTime.Now;

            if (!q.Any())
            {
                newList.Cours = cdc.Cours_Table.Single(c => c.Id == coursId);

                cdc.ResourceList_Table.InsertOnSubmit(newList);
                cdc.SubmitChanges();
                cdc.Refresh(RefreshMode.OverwriteCurrentValues, newList);
            }
            else
            {
                q.First().UpdateFrom(newList);
                cdc.SubmitChanges();
                newList.Id = q.First().Id;
            }
            //}
        }

        public virtual void AddNotification(Notification newNot, ResourceModel attachedResource)
        {
            using (ClarolineDataContext cdc = new ClarolineDataContext(ClarolineDataContext.DBConnectionString))
            {

                Notification lastNotificationFromDB = cdc.Notifications_Table.OrderByDescending(n => n.date)
                                                                                     .FirstOrDefault(n => n.resource == newNot.resource);

                if (lastNotificationFromDB != null)
                {
                    if (lastNotificationFromDB.date.CompareTo(newNot.date) >= 0)
                    {
                        return;
                    }
                }

                newNot.resource = cdc.Resources_Table.Single(r => r.Id == attachedResource.Id);
                cdc.Notifications_Table.InsertOnSubmit(newNot);
                cdc.SubmitChanges();
            }
        }

        public virtual void AddResource(ResourceModel newRes, int containerId)
        {
            ClarolineDataContext cdc = ClarolineDB;
            //using (ClarolineDataContext cdc = new ClarolineDataContext(ClarolineDataContext.DBConnectionString))
            //{
                var q = cdc.Resources_Table.Where(r => r.DiscKey == newRes.DiscKey
                                                    && r.ResourceList.Id == containerId
                                                    && r.resourceId == newRes.resourceId
                                                 );

                newRes.updated = true;
                newRes.loaded = DateTime.Now;
                bool alreadyInDb = false;

                if (!q.Any())
                {
                    cdc.ResourceList_Table.Single(r => r.Id == containerId).Resources.Add(newRes);

                    cdc.Resources_Table.InsertOnSubmit(newRes);
                    cdc.SubmitChanges();
                    cdc.Refresh(RefreshMode.OverwriteCurrentValues, newRes);
                }
                else
                {
                    q.Single().UpdateFrom(newRes);
                    cdc.SubmitChanges();
                    newRes.Id = q.Single().Id;

                    alreadyInDb = true;
                }

                if (!(newRes is Document) || !(newRes as Document).isFolder)
                {
                    AddNotification(Notification.CreateNotification(alreadyInDb), newRes);
                }
            //}
        }

        public virtual void DeleteCours(Cours coursForDelete)
        {
            ClarolineDB.Cours_Table.DeleteOnSubmit(coursForDelete);

            SaveChangesToDB();
        }

        public virtual void DeleteResourceList(ResourceList listForDelete)
        {
            ClarolineDB.ResourceList_Table.DeleteOnSubmit(listForDelete);
            SaveChangesToDB();
        }

        public virtual void DeleteResource(ResourceModel resForDelete)
        {
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
            using (ClarolineDataContext cdc = new ClarolineDataContext(ClarolineDataContext.DBConnectionString))
            {
                (from Cours c
                 in cdc.Cours_Table
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
                cdc.SubmitChanges();
            }
        }

        public void ClearResOfCours(Cours coursToClear)
        {
            using (ClarolineDataContext cdc = new ClarolineDataContext(ClarolineDataContext.DBConnectionString))
            {
                (from ResourceModel rm
                     in cdc.Resources_Table
                 where rm.ResourceList.Cours.Equals(coursToClear)
                 select rm).ToList()
                     .ForEach(rm =>
                     {
                         if (rm.updated)
                         {
                             rm.updated = false;
                         }
                         else
                         {
                             DeleteResource(rm);
                         }
                     });
                cdc.SubmitChanges();
            }
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

        public virtual void LoadCollectionsFromDatabase() { }

        protected DateTime _lastClientCall = DateTime.MinValue;

        //Delay between two updates in hours
        protected const double UpdateDelay = 1.0;

        public virtual async Task RefreshAsync(bool force = false)
        {
            if (force || _lastClientCall.AddHours(UpdateDelay).CompareTo(DateTime.Now) < 0)
            {
                IsLoading = true;
                String updates = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.USER, SupportedMethods.GetUpdates);
                if (updates != "")
                {
                    _lastClientCall = DateTime.Now;
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
                                                upRes.notifiedDate = DateTime.Parse(resource.Value["date"]);
                                                SaveChangesToDB();

                                                AddNotification(Notification.CreateNotification(true), upRes);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                IsLoading = false;
            } //Else
        }

        public async Task<Cours> PrepareCoursForOpeningAsync(Cours coursToPrepare)
        {
            bool r = await GetResourcesListForThisCoursAsync(coursToPrepare);

            if (r)
            {
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

                using (ClarolineDataContext cdc = new ClarolineDataContext(ClarolineDataContext.DBConnectionString))
                {
                    coursToPrepare = (from Cours c in cdc.Cours_Table where c.Equals(coursToPrepare) select c).Single();
                    coursToPrepare.loaded = DateTime.Now;
                    int count = coursToPrepare.Resources.Count;
                    cdc.SubmitChanges();
                }
            }
            return coursToPrepare;
        }

        public async Task GetResourcesForThisListAsync(ResourceList container)
        {
            IsLoading = true;
            _lastClientCall = DateTime.Now;
            String strContent = await ClaroClient.Instance.MakeOperationAsync(container.GetSupportedModule(),
                                                                              SupportedMethods.GetResourcesList,
                                                                              reqCours: container.Cours,
                                                                              genMod: container.label);

            if (strContent != "")
            {
                IList resources = (IList)JsonConvert.DeserializeObject(strContent, container.ressourceListType);

                foreach (ResourceModel item in resources)
                {
                    AddResource(item, container.Id);
                }
                container.loaded = DateTime.Now;
            }
            IsLoading = false;
        }

        public async Task GetSingleResourceAsync(ResourceList container, string resourceString = null)
        {
            IsLoading = true;
            _lastClientCall = DateTime.Now;
            String strContent = await ClaroClient.Instance.MakeOperationAsync(container.GetSupportedModule(),
                                                                              SupportedMethods.GetSingleResource,
                                                                              container.Cours,
                                                                              resourceString);
            if (strContent != "")
            {

                ResourceModel resource = (ResourceModel)JsonConvert.DeserializeObject(strContent, container.ressourceType);
                AddResource(resource, container.Id);
            }
            IsLoading = false;
        }

        public async Task<bool> GetUserDataAsync()
        {
            IsLoading = true;
            String strContent = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.USER,
                                                                              SupportedMethods.GetUserData);

            if (strContent != "")
            {
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

                IsLoading = false;
                return true;
            }
            IsLoading = false;
            return false;
        }

        public async Task GetCoursListAsync()
        {
            IsLoading = true;
            _lastClientCall = DateTime.Now;
            String strContent = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.USER,
                                                                              SupportedMethods.GetCourseList);
            if (strContent != "")
            {
                List<Cours> Courses = JsonConvert.DeserializeObject<List<Cours>>(strContent);

                foreach (Cours cours in Courses)
                {
                    AddCours(cours);
                }

                ClearCoursList();
            }
            IsLoading = false;
        }

        public async Task<bool> GetResourcesListForThisCoursAsync(Cours cours)
        {
            IsLoading = false;
            _lastClientCall = DateTime.Now;
            String strContent = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.USER,
                                                                              SupportedMethods.GetToolList,
                                                                              cours);
            if (strContent != "")
            {
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
                        case Forum.Label:
                            item.ressourceType = typeof(Forum);
                            break;
                        default:
                            item.ressourceType = typeof(ResourceModel);
                            break;
                    }

                    AddResourceList(item, cours.Id);
                }
                IsLoading = false;
                return true;
            }
            IsLoading = false;
            return false;
        }

        public async Task<bool> CheckHostValidity(string url)
        {
            string page = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.NOMOD, SupportedMethods.GetPage, genMod: url, forAuth: true);
            return page.Contains("<!-- - - - - - - - - - - Claroline Body - - - - - - - - - -->");
        }

        public async Task<string> CheckModuleValidity()
        {
            Regex regex = new Regex(@"<!-- PLATFORM SETTINGS (\P{Cn}*?) PLATFORM SETTINGS -->", RegexOptions.CultureInvariant);
            string page = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.NOMOD, SupportedMethods.GetPage, genMod: AppSettings.Instance.DomainSetting);
            if (regex.IsMatch(page))
            {
                return regex.Match(page).Groups[1].Value;
            }
            return "{}";
        }
    }
}
