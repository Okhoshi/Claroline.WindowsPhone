using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarolineApp.RT.Models
{
    public class Annonce : ItemModel
    {
        // Variables globales : propriétés

        public int ressourceId
        {
            get { return GetProperty<int>("ressourceId"); }
            set { SetProperty<int>("ressourceId", value); }
        }

        public String content
        {
            get { return GetProperty<String>("content"); }
            set { SetProperty<String>("content", value); }
        }

        public static async Task<ObservableCollection<Annonce>> getAllAnnouncesAsync(Cours cours)
        {
            String result = await ClarolineClient.Current.makeOperationAsync(SupportedModules.CLANN, SupportedMethods.getResourcesList, cours);
            ObservableCollection<Annonce> ann = JsonConvert.DeserializeObject<ObservableCollection<Annonce>>(result);
            foreach (Annonce annonce in ann)
            {
                annonce.Group = cours.AllResources.First(val => val.ID.Equals("annonce"));
            }
            return ann;
        }

        public static async Task<Annonce> getSingleAnnounceAsync(Cours cours, int resID)
        {
            String result = await ClarolineClient.Current.makeOperationAsync(SupportedModules.CLANN, SupportedMethods.getResourcesList, cours, resID.ToString());
            return JsonConvert.DeserializeObject<Annonce>(result);
        }
    }
}
