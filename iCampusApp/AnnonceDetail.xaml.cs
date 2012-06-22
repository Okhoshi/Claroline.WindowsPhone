using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;
using ClarolineApp.Model;
using Microsoft.Phone.Shell;

namespace ClarolineApp
{
    public partial class AnnonceDetail : PhoneApplicationPage
    {
        private Annonce _annonce;

        public AnnonceDetail()
        {
            InitializeComponent();

            this.DataContext = App.selecteditem;
            _annonce = App.selecteditem as Annonce;

            SystemTray.SetProgressIndicator(this, App.currentProgressInd);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (_annonce.notified)
            {
                App.currentProgressInd.Text = "";
                App.currentProgressInd.IsVisible = true;

                _annonce.notified = false;
                App.ViewModel.AddAnnonce(_annonce);
                _annonce.Cours.checkNotified();
                App.ViewModel.AllNotifications.Where(not => not.notified && not.ressourceId == _annonce.ressourceId && not.ressourceType == ValidTypes.Annonce).ToList().ForEach(not =>
                {
                    not.notified = false;
                    App.ViewModel.AddNotification(not);
                });

                App.Client.makeOperation(AllowedOperations.getSingleAnnounce, _annonce.Cours, _annonce.ressourceId);
            }
        }
    }
}