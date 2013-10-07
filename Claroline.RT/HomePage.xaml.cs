using Callisto.Controls;
using ClarolineApp.RT.Controls;
using ClarolineApp.RT.Data;
using ClarolineApp.RT.Models;
using ClarolineApp.RT.Settings;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.ApplicationSettings;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Page Détail du groupe, consultez la page http://go.microsoft.com/fwlink/?LinkId=234229

namespace ClarolineApp.RT
{
    /// <summary>
    /// Page affichant une vue d'ensemble d'un groupe, ainsi qu'un aperçu des éléments
    /// qu'il contient.
    /// </summary>
    public sealed partial class HomePage : ClarolineApp.RT.Common.LayoutAwarePage
    {
        public HomePage()
        {
            this.InitializeComponent();

            Current = this;
        }

        public static HomePage Current;

        /// <summary>
        /// Remplit la page à l'aide du contenu passé lors de la navigation. Tout état enregistré est également
        /// fourni lorsqu'une page est recréée à partir d'une session antérieure.
        /// </summary>
        /// <param name="navigationParameter">Valeur de paramètre passée à
        /// <see cref="Frame.Navigate(Type, Object)"/> lors de la requête initiale de cette page.
        /// </param>
        /// <param name="pageState">Dictionnaire d'état conservé par cette page durant une session
        /// antérieure. Null lors de la première visite de la page.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            //this.DefaultViewModel["Group"] = ;
            this.DefaultViewModel["Cours"] = ApplicationModel.Current.AllCours;

            Synchronize();
        }

        private async void Synchronize()
        {
            ObservableCollection<Cours> cours = await Cours.getAllCoursesAsync();
            //this.DefaultViewModel["Cours"] = cours;
        }

        /// <summary>
        /// Invoqué lorsqu'un utilisateur clique sur un élément.
        /// </summary>
        /// <param name="sender">GridView (ou ListView lorsque l'état d'affichage de l'application est Snapped)
        /// affichant l'élément sur lequel l'utilisateur a cliqué.</param>
        /// <param name="e">Données d'événement décrivant l'élément sur lequel l'utilisateur a cliqué.</param>
        public void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // Accédez à la page de destination souhaitée, puis configurez la nouvelle page
            // en transmettant les informations requises en tant que paramètre de navigation.
            var item = (Cours)e.ClickedItem;
            this.Frame.Navigate(typeof(CoursPage), item);
        }

        //TODO Header_Click aussi
    }
}

