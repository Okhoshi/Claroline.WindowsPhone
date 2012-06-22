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
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClarolineApp.Model;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
using ClarolineApp.Settings;
using ClarolineApp.Languages;

namespace ClarolineApp
{
    public partial class CoursPage : PhoneApplicationPage
    {

        private Documents rootDoc;
        private Cours _cours;

        public CoursPage()
        {
            InitializeComponent();

            _cours = App.selecteditem as Cours;
            SystemTray.SetProgressIndicator(this, App.currentProgressInd);

            this.DataContext = _cours;
            rootDoc = new Documents() { Cours = _cours, name = null, IsFolder = true, path = "" };
            DocContent.ItemsSource = rootDoc.getContent();
            AnnList.ItemsSource = App.ViewModel.AnnByCours[_cours.sysCode];

            //Supprime les PivotItems non inclus dans le cours

            if (!_cours.isDnL || SectionsPivot.Items.Count(p => ((PivotItem)p).Name == "DnL") == 0)
            {
                SectionsPivot.Items.Remove(SectionsPivot.Items.Single(p => ((PivotItem)p).Name == "DnL"));
                ApplicationBar.IsVisible = false;
            }
            else
                if (rootDoc.name == null)
                    (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;

            if (!_cours.isAnn || (SectionsPivot.Items.Count(p => ((PivotItem)p).Name == "Ann") == 0))
            {
                SectionsPivot.Items.Remove(SectionsPivot.Items.Single(p => ((PivotItem)p).Name == "Ann"));
            }
        }

        //--------------------------------------------------------------------
        // Event handler
        //--------------------------------------------------------------------

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
        }

        private void DocContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DocContent.SelectedIndex == -1)
            {
                return;
            }
            if ((DocContent.SelectedItem as Documents).notified)
            {
                (DocContent.SelectedItem as Documents).checkNotified();
            }
            if ((DocContent.SelectedItem as Documents).IsFolder)
            {
                rootDoc = DocContent.SelectedItem as Documents;
                PivotItem PI = (PivotItem)SectionsPivot.Items.Single(p => ((PivotItem)p).Name == "DnL");
                PI.Header = rootDoc.name.ToLower();
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;

                DocContent.ItemsSource = rootDoc.getContent();
                DocContent.SelectedIndex = -1;
            }
            else
            {
                rootDoc.checkNotified();
                AppSettings appSet = new AppSettings();
                byte[] toEncodeAsBytes = System.Text.Encoding.GetEncoding("iso-8859-1").GetBytes(appSet.DomainSetting + (DocContent.SelectedItem as Documents).url.Replace("&amp;", "&"));
                string EncodedURL = System.Convert.ToBase64String(toEncodeAsBytes);
                string loginString = "login=" + appSet.UsernameSetting + "&password=" + appSet.PasswordSetting;

                WebBrowserTask open = new WebBrowserTask()
                {
                    Uri = new Uri(Uri.EscapeUriString(appSet.DomainSetting + appSet.AuthPageSetting + "?" + loginString + "&sourceUrl=" + EncodedURL), UriKind.Absolute)
                };
                DocContent.SelectedIndex = -1;
                open.Show();
            }
        }

        private void rootButton_Click(object sender, EventArgs e)
        {
            PivotItem PI = (PivotItem)SectionsPivot.Items.Single(p => ((PivotItem)p).Name == "DnL");
            Documents _root = rootDoc.getRoot();
            DocContent.ItemsSource = _root.getContent();

            if (_root.name == null)
            {
                _cours.checkNotified();
                PI.Header = AppLanguage.CoursPage_Doc_PI;
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            }
            else
            {
                _root.checkNotified();
                PI.Header = _root.name.ToLower();
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            }
            rootDoc = _root;
        }

        private void refrButton_Click(object sender, EventArgs e)
        {
            App.Client.makeOperation(AllowedOperations.getUpdates, _cours);
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SectionsPivot.SelectedItem.Equals(DnL) && rootDoc.name != null)
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = true;
            }
            else
            {
                (ApplicationBar.Buttons[0] as ApplicationBarIconButton).IsEnabled = false;
            }
        }

        private void AnnList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnnList.SelectedIndex == -1)
            {
                return;
            }

            App.selecteditem = AnnList.SelectedItem;
            AnnList.SelectedIndex = -1;
            NavigationService.Navigate(new Uri("/AnnonceDetail.xaml", UriKind.Relative));
        }
    }
}