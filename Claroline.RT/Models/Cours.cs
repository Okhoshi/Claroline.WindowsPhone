using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Json;



namespace ClarolineApp.RT.Models
{
    public class Cours : ViewModelBase
    {
        // Variables globales : propriétés

        public DateTime isLoaded;

        public bool notified
        {
            get { return GetProperty<bool>("notified"); }
            set { SetProperty<bool>("notified", value); }
        }

        public bool Updated
        {
            get { return GetProperty<bool>("Updated"); }
            set { SetProperty<bool>("Updated", value); }
        }

        public String officialEmail
        {
            get { return GetProperty<string>("officialEmail"); }
            set { SetProperty<string>("officialEmail", value); }
        }

        public String sysCode
        {
            get { return GetProperty<string>("sysCode"); }
            set { SetProperty<string>("sysCode", value); }
        }
        public String title
        {
            get { return GetProperty<string>("title"); }
            set { SetProperty<string>("title", value); }
        }

        public String titular
        {
            get { return GetProperty<string>("titular"); }
            set { SetProperty<string>("titular", value); }
        }

        public String officialCode
        {
            get { return GetProperty<string>("officialCode"); }
            set { SetProperty<string>("officialCode", value); }
        }
        public ObservableCollection<ResourceModel<ItemModel>> AllResources
        {
            get { return GetProperty<ObservableCollection<ResourceModel<ItemModel>>>("AllResources"); }
            set { SetProperty<ObservableCollection<ResourceModel<ItemModel>>>("AllResources", value); }
        }

        public Cours()
        {
            AllResources = new ObservableCollection<ResourceModel<ItemModel>>();
            AllResources.Add(new ResourceModel<ItemModel>(){ ID = "annonce", Name = "Annonces"});
            AllResources.Add(new ResourceModel<ItemModel>() { ID = "document", Name = "Documents et Liens"});
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is loading.
        /// </summary>
        /// <value>
        /// <c>true</c> if this instance is loading; otherwise, <c>false</c>.
        /// </value>
        public bool IsSynchronizing
        {
            get { return GetProperty<bool>("IsSynchronizing"); }
            set { SetProperty<bool>("IsSynchronizing", value); }
        }

        public static async Task<ObservableCollection<Cours>> getAllCoursesAsync()
        {
            String result = await ClarolineClient.Current.makeOperationAsync(SupportedModules.USER, SupportedMethods.getCourseList);
            return new ObservableCollection<Cours>(JsonConvert.DeserializeObject<List<Cours>>(result));
        }

        public static async Task<Cours> getCompleteCourseAsync(Cours cours)
        {
            String result = await ClarolineClient.Current.makeOperationAsync(SupportedModules.USER, SupportedMethods.getCourseToolList, cours);

            try
            {
                JsonTextReader reader = new JsonTextReader(new StringReader(result));
                while (reader.Read())
                {
                    if (reader.Value != null)
                    {
                        switch (reader.Value.ToString())
                        {
                            case "isAnn":
                                reader.Read();
                                cours.AllResources[0].isRes = (bool)reader.Value;
                                break;
                            case "annNotif":
                                reader.Read();
                                cours.AllResources[0].resNotif = (bool)reader.Value;
                                break;
                            case "isDnL":
                                reader.Read();
                                cours.AllResources[1].isRes = (bool)reader.Value;
                                break;
                            case "dnlNotif":
                                reader.Read();
                                cours.AllResources[1].resNotif = (bool)reader.Value;
                                break;

                        }
                    }
                }

                cours.AllResources[0].Items.Clear();
                foreach (ItemModel item in (await Annonce.getAllAnnouncesAsync(cours)))
                {
                    cours.AllResources[0].Items.Add(item);
                }
                cours.AllResources[1].Items.Clear();
                foreach (ItemModel item in await Document.getAllDocumentsAsync(cours))
                {
                    cours.AllResources[1].Items.Add(item);
                }
            }
            catch (JsonException ex)
            {
                Debug.WriteLine(ex.Message);
            }

            return cours;
        }

        /*public ObservableCollection<ItemModel> groupedResources { 
            get {
                ObservableCollection<ItemModel> res = new ObservableCollection<ItemModel>();
                foreach (ResourceModel<ItemModel> ress in AllResources)
                {
                    foreach (ItemModel item in ress.Item)
                    {
                        item.groupBy = ress.Name;
                        res.Add(item);
                    }
                }
                res
            }
        }*/
    }
}