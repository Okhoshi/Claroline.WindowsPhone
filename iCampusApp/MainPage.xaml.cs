using System;
using System.Collections.Generic;
using System.Collections.Specialized;
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
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data.Linq;
using ClarolineApp.Model;
using ClarolineApp.Settings;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System.Diagnostics;
using ClarolineApp.Languages;

namespace ClarolineApp
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        PropertyChangedEventHandler MainPage_Handler;
        PropertyChangedEventHandler Failure_Handler;
        AppSettings set;

        // Constructeur
        public MainPage()
        {
            InitializeComponent();
            this.DataContext = App.ViewModel;
            set = new AppSettings();
            SystemTray.SetProgressIndicator(this, App.currentProgressInd);
            MainPage_Handler = new PropertyChangedEventHandler(MainPage_PropertyChanged);
            Failure_Handler = new PropertyChangedEventHandler(FailureOccured);
        }
        //--------------------------------------------------------------------
        // Event handler
        //--------------------------------------------------------------------


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Panorama.Title = (new AppSettings()).PlatformSetting;
        }

        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        // Used to notify Silverlight that a property has changed.

        private void NotifyPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        private void CoursList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoursList.SelectedIndex == -1)
            {
                return;
            }

            App.selecteditem = CoursList.SelectedItem;

            if ((CoursList.SelectedItem as Cours).isLoaded)
            {
                if ((CoursList.SelectedItem as Cours).notified)
                {
                    ((Cours)CoursList.SelectedItem).notified = false;
                    App.ViewModel.AddCours((Cours)CoursList.SelectedItem);
                }

                NavigationService.Navigate(new Uri("/CoursPage.xaml", UriKind.Relative));
            }
            else
            {
                ((Cours)CoursList.SelectedItem).PropertyChanged += MainPage_Handler;
                App.Client.PropertyChanged += Failure_Handler;

                PerformanceProgressBar progress = Helper.FindFirstElementInVisualTree<PerformanceProgressBar>(this.CoursList.ItemContainerGenerator.ContainerFromIndex(CoursList.SelectedIndex) as ListBoxItem);
                progress.Visibility = System.Windows.Visibility.Visible;

                App.Client.makeOperation(AllowedOperations.updateCompleteCourse, CoursList.SelectedItem as Cours);
                App.selecteditem = CoursList.SelectedItem;
            }
            CoursList.SelectedIndex = -1;
        }

        void FailureOccured(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "Failure" && App.selecteditem != null)
            {
                Cours _cours = App.selecteditem as Cours;
                App.selecteditem = null;
                _cours.PropertyChanged -= MainPage_Handler;
                App.Client.PropertyChanged -= Failure_Handler;

                PerformanceProgressBar progress = Helper.FindFirstElementInVisualTree<PerformanceProgressBar>(this.CoursList.ItemContainerGenerator.ContainerFromIndex(CoursList.Items.IndexOf(_cours)) as ListBoxItem);
                progress.Visibility = System.Windows.Visibility.Collapsed;
            }
        }

        void MainPage_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "isLoaded":
                    ((Cours)sender).PropertyChanged -= MainPage_Handler;

                    PerformanceProgressBar progress = Helper.FindFirstElementInVisualTree<PerformanceProgressBar>(this.CoursList.ItemContainerGenerator.ContainerFromIndex(CoursList.Items.IndexOf(sender)) as ListBoxItem);
                    progress.Visibility = System.Windows.Visibility.Collapsed;
                    App.selecteditem = sender;

                    if ((sender as Cours).notified)
                    {
                        ((Cours)sender).notified = false;
                        App.ViewModel.AddCours((Cours)sender);
                    }
                    NavigationService.Navigate(new Uri("/CoursPage.xaml", UriKind.Relative));
                    break;
                default:
                    break;
            }
        }

        private void SettingsPage_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings/SettingsPage.xaml", UriKind.Relative));
        }

        private void Connect_Click(object sender, EventArgs e)
        {
            App.Client.makeOperation(AllowedOperations.getCourseList);
        }

        private void Sync_Btn_Click(object sender, EventArgs e)
        {
            App.Client.makeOperation(AllowedOperations.getUpdates);
        }

        private void DEV_clrDB_Click(object sender, EventArgs e)
        {
            App.ViewModel.ResetDatabase();
        }

        private void IEButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask homepage = new WebBrowserTask()
            {
                Uri = new Uri(set.DomainSetting + set.AuthPageSetting + "?login=" + set.UsernameSetting + "&password=" + set.PasswordSetting, UriKind.Absolute)
            };
            homepage.Show();
        }

        private void MailButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask mailToDev = new EmailComposeTask()
            {
                To = String.Format("\"{0} Developpment Team\" <support.develop@live.be>", App.ApplicationName),
                Subject = String.Format("[{0}] {1}", App.ApplicationName, AppLanguage.MainPage_About_DevMailButton),
            };
            mailToDev.Show();
        }

        private void sendMail_Click(object sender, RoutedEventArgs e)
        {
            Cours _item = (sender as MenuItem).Tag as Cours;
            if (_item != null)
            {
                EmailComposeTask mailToManagers = new EmailComposeTask()
                {
                    To = String.Format("\"{0}\" <{1}>", _item.titular, _item.officialEmail),
                    Subject = String.Format("[{0}][{1}]", set.PlatformSetting, _item.sysCode)
                };
                mailToManagers.Show();
            }
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Panorama.SelectedItem.Equals(Cours_PI))
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
            }
            else
            {
                ApplicationBar.Mode = ApplicationBarMode.Default;
            }
        }
    }
}