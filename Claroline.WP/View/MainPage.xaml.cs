using ClarolineApp.Languages;
using ClarolineApp.Model;
using ClarolineApp.Settings;
using ClarolineApp.VM;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Phone.Tasks;
using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace ClarolineApp
{
    public partial class MainPage : PhoneApplicationPage
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

        private IMainPageVM _viewModel;

        // Constructeur
        public MainPage()
        {
            InitializeComponent();
            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            Failure_Handler = new PropertyChangedEventHandler(FailureOccured);

            _viewModel = new MainPageVM();
            this.DataContext = _viewModel;
            _viewModel.PropertyChanged += VM_PropertyChanged;

            ClaroClient.Instance.PropertyChanged += Failure_Handler;

            version_text.Text = Helper.GetVersionNumber();

            ApplicationBar.Buttons.Add(new ApplicationBarIconButton()
            {
                Text = AppLanguage.AppBar_Refresh,
                IconUri = new Uri("/icons/appbar.refresh.rest.png", UriKind.Relative)
            });
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Click += Sync_Btn_Click;

            ApplicationBar.MenuItems.Add(new ApplicationBarMenuItem(AppLanguage.AppBar_Settings));
            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Click += SettingsPage_Click;
#if DEBUG
            ApplicationBar.MenuItems.Add(new ApplicationBarMenuItem("[DEV] CLEAR"));
            (ApplicationBar.MenuItems[0] as ApplicationBarMenuItem).Click += DEV_clrDB_Click ;
#endif
        }

        //--------------------------------------------------------------------
        // Event handler
        //--------------------------------------------------------------------


        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            if (AppSettings.Instance.UserNameSetting == "")
            {
                if (MessageBox.Show(AppLanguage.ErrorMessage_NoLogin_Message, AppLanguage.ErrorMessage_NoLogin_Caption, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                {
                    NavigationService.Navigate(new Uri("/Settings/SettingsPage.xaml", UriKind.Relative));
                }
            }

            _viewModel.LoadCollectionsFromDatabase();
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
            ClaroClient.Instance.PropertyChanged -= FailureOccured;
            _viewModel.PropertyChanged -= VM_PropertyChanged;
        }

        private void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsLoading":
                    indicator.IsVisible = _viewModel.IsLoading;
                    break;
                default:
                    break;
            }
        }

        private void CoursList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CoursList.SelectedIndex == -1)
            {
                return;
            }

            Cours _cours = CoursList.SelectedItem as Cours;

            string destination = String.Format("/View/CoursPage.xaml?cours={0}", _cours.sysCode);
            NavigationService.Navigate(new Uri(destination, UriKind.Relative));

            CoursList.SelectedIndex = -1;
        }

        void FailureOccured(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "LastException")
            {
                MessageBox.Show("Exception occured! " + ClaroClient.Instance.LastException.Message);
            }
        }

        private void SettingsPage_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Settings/SettingsPage.xaml", UriKind.Relative));
        }

        private async void Sync_Btn_Click(object sender, EventArgs e)
        {
            indicator.Text = AppLanguage.ProgressBar_Connecting;
            indicator.IsVisible = true;

            bool r = true;
            if (!ClaroClient.Instance.IsValidAccountSync())
            {
                r = await _viewModel.GetUserDataAsync();
            }
            if (r && ((_viewModel as MainPageVM).allCours.Count == 0 || AppSettings.Instance.LastListRequestSetting.CompareTo(DateTime.Now.AddDays(-2)) < 0))
            {
                indicator.Text = String.Format(AppLanguage.ProgressBar_ProcessResult, AppLanguage.MainPage_Cours_PI);
                await _viewModel.GetCoursListAsync();
            }
            else if (r)
            {
                indicator.Text = AppLanguage.ProgressBar_Update;
                await _viewModel.RefreshAsync(true);
            }
            indicator.IsVisible = false;
        }
#if DEBUG
        private void DEV_clrDB_Click(object sender, EventArgs e)
        {
            _viewModel.ResetViewModel();
        }
#endif

        private void IEButton_Click(object sender, RoutedEventArgs e)
        {
            WebBrowserTask homepage = new WebBrowserTask()
            {
                Uri = new Uri(AppSettings.Instance.DomainSetting
                            + AppSettings.Instance.AuthPageSetting
                            + "?login="
                            + AppSettings.Instance.UserNameSetting
                            + "&password="
                            + AppSettings.Instance.PasswordSetting
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
                Body = String.Format("[V:{0} Campus:{1}]\n\n", Helper.GetVersionNumber(true), AppSettings.Instance.PlatformSetting)
            };
            mailToDev.Show();
        }

        private void SendMail_Click(object sender, RoutedEventArgs e)
        {
            Cours _item = (sender as MenuItem).Tag as Cours;
            if (_item != null)
            {
                EmailComposeTask mailToManagers = new EmailComposeTask()
                {
                    To = String.Format("\"{0}\" <{1}>", _item.titular, _item.officialEmail),
                    Subject = String.Format("[{0}][{1}]", AppSettings.Instance.PlatformSetting, _item.title)
                };
                mailToManagers.Show();
            }
        }
    }
}