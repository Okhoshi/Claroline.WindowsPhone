using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ClarolineOnTablet.Models
{
    class ClarolineModel : ViewModelBase
    {

        public event SynchronizedHandler Synchronized;

        public ClarolineModel()
        {
            this.AllAnnonces = new ObservableCollection<Annonce>();
            this.AllCours = new ObservableCollection<Cours>();
            this.AllDocuments = new ObservableCollection<Document>();
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

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        /// <value>
        /// The ID.
        /// </value>
        public string ID
        {
            get { return GetProperty<string>("ID"); }
            set { SetProperty<string>("ID", value); }
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        /// <value>
        /// The name.
        /// </value>
        public string Name
        {
            get { return GetProperty<string>("Name"); }
            set { SetProperty<string>("Name", value); }
        }

        public ObservableCollection<Cours> AllCours
        {
            get { return GetProperty<ObservableCollection<Cours>>("AllCours"); }
            set { SetProperty<ObservableCollection<Cours>>("AllCours",value);}
        }

        public ObservableCollection<Annonce> AllAnnonces
        {
            get { return GetProperty<ObservableCollection<Annonce>>("AllAnnonces"); }
            set { SetProperty<ObservableCollection<Annonce>>("AllAnnonces", value); }
        }

        public ObservableCollection<Document> AllDocuments
        {
            get { return GetProperty<ObservableCollection<Document>>("AllDocuments"); }
            set { SetProperty<ObservableCollection<Document>>("AllDocuments", value); }
        }

    }
}
