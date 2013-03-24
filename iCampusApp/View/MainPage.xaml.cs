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
using System.Windows.Data;
using ClarolineApp.ViewModel;

namespace ClarolineApp
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        PropertyChangedEventHandler Failure_Handler;

        ProgressIndicator _indicator;

        ProgressIndicator indicator
        {
            get
            {
                if (_indicator == null)
                {
                    _indicator = new ProgressIndicator()
                    {
                        IsIndeterminate = true,
                        IsVisible = false
                    };
                    SystemTray.SetProgressIndicator(this, _indicator);
                }
                return _indicator;
            }
        }

        private IMainPageViewModel _viewModel;

        // Constructeur
        public MainPage()
        {
            InitializeComponent();
            Failure_Handler = new PropertyChangedEventHandler(FailureOccured);

            _viewModel = new MainPageViewModel();
            this.DataContext = _viewModel;

            version_text.Text = Helper.GetVersionNumber();
        }
        //--------------------------------------------------------------------
        // Event handler
        //--------------------------------------------------------------------


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            this.Panorama.Title = AppSettings.instance.PlatformSetting;

            if (AppSettings.instance.UsernameSetting == "")
            {
                if (MessageBox.Show(AppLanguage.ErrorMessage_NoLogin_Message, AppLanguage.ErrorMessage_NoLogin_Caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("/Settings/SettingsPage.xaml", UriKind.Relative));
                }
            }
        }
        
        private async void CoursList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoursList.SelectedIndex == -1)
            {
                return;
            }

            Cours _cours = CoursList.SelectedItem as Cours;

            if (_cours.loadedToday() && _cours.Resources.Count != 0)
            {
                string destination = String.Format("/View/CoursPage.xaml?cours={0}", _cours.sysCode);
                NavigationService.Navigate(new Uri(destination, UriKind.Relative));
            }
            else
            {
                ClaroClient.instance.PropertyChanged += Failure_Handler;

                PerformanceProgressBar progress = Helper.FindFirstElementInVisualTree<PerformanceProgressBar>(this.CoursList.ItemContainerGenerator.ContainerFromIndex(CoursList.SelectedIndex) as ListBoxItem);
                progress.Visibility = System.Windows.Visibility.Visible;

                await _viewModel.PrepareCoursForOpeningAsync(CoursList.SelectedItem as Cours);

                progress.Visibility = System.Windows.Visibility.Collapsed;
            }

            CoursList.SelectedIndex = -1;
        }

        void FailureOccured(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "lastException")
            {
                ClaroClient.instance.PropertyChanged -= Failure_Handler;

                MessageBox.Show("Exception occured! " + ClaroClient.instance.lastException.Message);
            }
        }

        private void SettingsPage_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings/SettingsPage.xaml", UriKind.Relative));
        }

        private async void Connect_Click(object sender, EventArgs e)
        {
            indicator.Text = AppLanguage.ProgressBar_Connecting;
            indicator.IsVisible = true;
            if (!ClaroClient.instance.isValidAccountWithoutWaiting())
            {
                await _viewModel.GetUserDataAsync();
            }
            else
            {
                indicator.Text = String.Format(AppLanguage.ProgressBar_ProcessResult, AppLanguage.MainPage_Cours_PI);
                await _viewModel.GetCoursListAsync();
            }
            indicator.IsVisible = false;
        }

        private async void Sync_Btn_Click(object sender, EventArgs e)
        {
            indicator.Text = AppLanguage.ProgressBar_Update;
            indicator.IsVisible = true;
            await _viewModel.RefreshAsync();
            indicator.IsVisible = false;
        }

        private void DEV_clrDB_Click(object sender, EventArgs e)
        {
            _viewModel.ResetViewModel();
        }

        private void IEButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask homepage = new WebBrowserTask()
            {
                Uri = new Uri(AppSettings.instance.DomainSetting 
                            + AppSettings.instance.AuthPageSetting 
                            + "?login=" 
                            + AppSettings.instance.UsernameSetting 
                            + "&password=" 
                            + AppSettings.instance.PasswordSetting
                           , UriKind.Absolute)
            };
            homepage.Show();
        }

        private void MailButton_Click(object sender, RoutedEventArgs e)
        {
            EmailComposeTask mailToDev = new EmailComposeTask()
            {
                To = String.Format("\"{0} Development Team\" <support.develop@live.be>", App.ApplicationName),
                Subject = String.Format("[{0}] {1}", App.ApplicationName, AppLanguage.MainPage_About_DevMailButton),
				Body = String.Format("[V:{0} Campus:{1}]\n\n", Helper.GetVersionNumber(true), AppSettings.instance.PlatformSetting)
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
                    Subject = String.Format("[{0}][{1}]", AppSettings.instance.PlatformSetting, _item.title)
                };
                mailToManagers.Show();
            }
        }

        private void Panorama_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Panorama.SelectedItem.Equals(About_PI))
            {
                ApplicationBar.Mode = ApplicationBarMode.Minimized;
            }
            else
            {
                ApplicationBar.Mode = ApplicationBarMode.Default;
            }
        }

        private void NotifList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (NotifList.SelectedIndex == -1)
            {
                return;
            }

            CL_Notification selectedNotification = (CL_Notification)sender;
            
            MessageBox.Show("Selected :" + selectedNotification.ToString());

            NotifList.SelectedIndex = -1;
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
    }
}