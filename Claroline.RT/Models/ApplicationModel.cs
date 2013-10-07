using System.Linq;
using System.Collections.ObjectModel;
using ClarolineApp.RT.Common;
using System.Collections.Generic;
using Windows.Storage;
using System;
using Windows.Networking.Connectivity;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Activation;

namespace ClarolineApp.RT.Models
{
    public delegate void SynchronizedHandler(object sender, EventArgs e);

    public class ApplicationModel : ViewModelBase
    {
        private static ApplicationModel instance;

        public static ApplicationModel Current
        {
            get
            {
                if (instance == null)
                    instance = new ApplicationModel();
                return instance;
            }
        }

        public ApplicationModel()
        {
#if DEBUG
            this.Domain = @"http://mesconsult.be/clarodev";
            this.WebServicePath = "/module/MOBILE/";
            this.Login = "qdevos";
            this.Password = "elegie24";
#else
            this.Domain;
            this.WebServicePath = "/module/MOBILE/";
            this.Login;
            this.Password;
#endif
            //
            // set settings
            //
            this.IsInitialized = false;
            this.Title = App.Current.Resources["AppName"] as string;
            this.LogoUri = "ms-appx:///Assets/TitleLogo.png";
            //this.Support = App.Current.Resources["Support"] as string;
            //this.Copyright = App.Current.Resources["Copyright"] as string;
            //this.Privacy = App.Current.Resources["PrivacyUri"] as string;
            this.LogoFullWidth = 600;
            this.LogoSnappedWidth = 250;
            this.MaxItemsToShow = 6;
            this.AllCours = new ObservableCollection<Cours>();
            this.IsConnected = NetworkInformation.GetInternetConnectionProfile() != null;
            //
            // set commands
            //
            this.SynchronizeCommand = new SynchronizeCommand();
            this.SynchronizeCommand.Notify += (snd, sccmd) =>
            {
                //
                // synchronize
                //
                Synchronize();
            };
            //
            // is initialized
            //
            this.IsInitialized = true;
        }

        public event SynchronizedHandler Synchronized;

        public bool IsConnected
        {
            get { return GetProperty<bool>("IsConnected"); }
            set { SetProperty<bool>("IsConnected", value); }
        }

        public string Title
        {
            get { return GetProperty<string>("Title"); }
            set { SetProperty<string>("Title", value); }
        }

        public string LogoUri
        {
            get { return GetProperty<string>("LogoUri"); }
            set { SetProperty<string>("LogoUri", value); }
        }

        public string Login
        {
            get { return GetProperty<string>("Login"); }
            set { SetProperty<string>("Login", value); }
        }

        public string Password
        {
            get { return GetProperty<string>("Password"); }
            set { SetProperty<string>("Password", value); }
        }

        public string Domain
        {
            get { return GetProperty<string>("Domain"); }
            set { SetProperty<string>("Domain", value); }
        }

        public string WebServicePath
        {
            get { return GetProperty<string>("WebServicePath"); }
            set { SetProperty<string>("WebServicePath", value); }
        }

        public int LogoFullWidth
        {
            get { return GetProperty<int>("LogoFullWidth"); }
            set { SetProperty<int>("LogoFullWidth", value); }
        }

        public int LogoSnappedWidth
        {
            get { return GetProperty<int>("LogoSnappedWidth"); }
            set { SetProperty<int>("LogoSnappedWidth", value); }
        }

        public string Support
        {
            get { return GetProperty<string>("Support"); }
            set { SetProperty<string>("Support", value); }
        }

        public string Copyright
        {
            get { return GetProperty<string>("Copyright"); }
            set { SetProperty<string>("Copyright", value); }
        }

        public int MaxItemsToShow
        {
            get { return GetProperty<int>("MaxItemsToShow"); }
            set { SetProperty<int>("MaxItemsToShow", value); }
        }

        public ObservableCollection<Cours> AllCours
        {
            get { return GetProperty<ObservableCollection<Cours>>("AllCours"); }
            set { SetProperty<ObservableCollection<Cours>>("AllCours", value); }
        }

        public string Privacy
        {
            get { return GetProperty<string>("Privacy"); }
            set { SetProperty<string>("Privacy", value); }
        }

        public bool IsInitialized
        {
            get { return GetProperty<bool>("IsInitialized"); }
            set { SetProperty<bool>("IsInitialized", value); }
        }

        public bool IsSynchronizing
        {
            get { return GetProperty<bool>("IsSynchronizing"); }
            set { SetProperty<bool>("IsSynchronizing", value); }
        }

        /// <summary>
        /// Gets or sets the horizontal scroll offet.
        /// </summary>
        /// <value>
        /// The horizontal scroll offet.
        /// </value>
        public double HorizontalScrollOffet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the vertical scroll offet.
        /// </summary>
        /// <value>
        /// The vertical scroll offet.
        /// </value>
        public double VerticalScrollOffet
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the synchronize command.
        /// </summary>
        /// <value>
        /// The synchronize command.
        /// </value>
        public SynchronizeCommand SynchronizeCommand
        {
            get;
            set;
        }


        /// <summary>
        /// Loads the state.
        /// </summary>
        public void LoadState()
        {

        }

        protected virtual void OnSynchronized(object sender, EventArgs e)
        {
            if (Synchronized != null)
            {
                Synchronized(sender, e);
            }
        }

        /// <summary>
        /// Synchronizes this instance.
        /// </summary>
        public async void Synchronize()
        {
            if (IsInitialized)
            {
                IsSynchronizing = true;

                AllCours.Clear();
                foreach (var item in await Cours.getAllCoursesAsync())
                {
                    AllCours.Add(item);
                }

                if (AllCours != null)
                {
                    //
                    // loop over feeds
                    //
                    foreach (Cours cours in AllCours)
                    {
                        //
                        // sync
                        //
                        await Cours.getCompleteCourseAsync(cours);
                        NotifyCoursSynchronized(cours);
                    }
                }
            }
        }

        internal void NotifyCoursSynchronized(Cours cours)
        {
            if (cours != null)
            {
                //
                // loop over all feeds
                //
                foreach (var f in cours.AllResources)
                {
                    if (f.IsSynchronizing == true)
                        return;
                }
                //
                // done
                //
                IsSynchronizing = false;
                //
                // done
                //
                OnSynchronized(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Finds the feed.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <returns></returns>
        public Cours FindCours(string syscode)
        {
            return (from cours in AllCours
                    where cours.sysCode.Equals(syscode)
                    select cours).FirstOrDefault();
        }

        /// <summary>
        /// Finds the item.
        /// </summary>
        /// <param name="resID">The id.</param>
        /// <returns></returns>
        public ItemModel Find(string resID)
        {
            return (from cours in AllCours
                    from res in cours.AllResources
                    from item in res.Items
                    where (res.ID.Equals(1) && (item as Annonce).ressourceId.Equals(resID)) || (res.ID.Equals(2) && (item as Document).path.Equals(resID))
                    select item).FirstOrDefault();
        }

        public List<ItemModel> Search(string pattern)
        {
            return (from cours in AllCours
                    from res in cours.AllResources
                    from item in res.Items
                    where (res.ID.Equals("annonce") && (item as Annonce).content != null && (item as Annonce).content.Contains(pattern))
                    || (res.ID.Equals("document") && (item as Document).Description != null && (item as Document).Description.Contains(pattern))
                    || item.title.Contains(pattern)
                    orderby item.date descending
                    select item).ToList();
        }

        protected override void OnPropertyChanged(string name)
        {
            //
            // call base
            //
            base.OnPropertyChanged(name);
            //
            // check if initialized
            //
            if (IsInitialized)
            {
                //
                // see what property has changed
                //
                switch (name)
                {
                    case "MaxItemsToShow":
                        //
                        // save
                        //
                        SavePersistentSettings();
                        //
                        // sync
                        //
                        ApplicationModel.Current.Synchronize();
                        break;
                    case "Login":
                    case "Password":
                    case "Domain":
                    case "WebServicePath":
                        //
                        // save
                        //
                        SavePersistentSettings();
                        break;
                }
            }
        }

        public void SavePersistentSettings()
        {
            //
            // save
            //
            ApplicationData.Current.LocalSettings.Values["MaxItemsToShow"] = Convert.ToString(MaxItemsToShow);
            ApplicationData.Current.LocalSettings.Values["Login"] = Login;
            ApplicationData.Current.LocalSettings.Values["Password"] = Password;
            ApplicationData.Current.LocalSettings.Values["Domain"] = Domain;
            ApplicationData.Current.LocalSettings.Values["WebServicePath"] = Domain;
        }

        public void LoadPersistentSettings()
        {
            //
            // check if there
            //
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("MaxItemsToShow"))
            {
                //
                // load
                //
                MaxItemsToShow = Convert.ToInt32(ApplicationData.Current.LocalSettings.Values["MaxItemsToShow"]);
            }
            //
            // check if there
            //
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Login"))
            {
                //
                // load
                //
                Login = Convert.ToString(ApplicationData.Current.LocalSettings.Values["Login"]);
            }
            //
            // check if there
            //
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Password"))
            {
                //
                // load
                //
                Password = Convert.ToString(ApplicationData.Current.LocalSettings.Values["Password"]);
            }
            //
            // check if there
            //
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("Domain"))
            {
                //
                // load
                //
                Domain = Convert.ToString(ApplicationData.Current.LocalSettings.Values["Domain"]);
            }
            //
            // check if there
            //
            if (ApplicationData.Current.LocalSettings.Values.ContainsKey("WebServicePath"))
            {
                //
                // load
                //
                Domain = Convert.ToString(ApplicationData.Current.LocalSettings.Values["WebServicePath"]);
            }
        }
    }
}
