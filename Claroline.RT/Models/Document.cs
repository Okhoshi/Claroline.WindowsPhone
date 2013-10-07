using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClarolineApp.RT.Models
{
    public class Document : ItemModel
    {
        // Variables globales : propriétés

        public bool IsFolder
        {
            get { return GetProperty<bool>("IsFolder"); }
            set { SetProperty<bool>("IsFolder", value); }
        }

        public String Description
        {
            get { return GetProperty<String>("Description"); }
            set { SetProperty<String>("Description", value); }
        }

        public String Extension
        {
            get { return GetProperty<String>("Extension"); }
            set { SetProperty<String>("Extension", value); }
        }

        public String path
        {
            get { return GetProperty<String>("path"); }
            set { SetProperty<String>("path", value); }
        }

        public String url
        {
            get { return GetProperty<String>("url"); }
            set { SetProperty<String>("url", value); }
        }

        public double size
        {
            get { return GetProperty<double>("size"); }
            set { SetProperty<double>("size", value); }
        }

        public static async Task<ObservableCollection<Document>> getAllDocumentsAsync(Cours cours)
        {
           String result = await ClarolineClient.Current.makeOperationAsync(SupportedModules.CLDOC, SupportedMethods.getResourcesList, cours);
           ObservableCollection<Document> docs =  JsonConvert.DeserializeObject<ObservableCollection<Document>>(result);
           foreach (Document doc in docs)
           {
               doc.Group = cours.AllResources.First(val => val.ID.Equals("document"));
           }
            return docs;
        }
    }
}
