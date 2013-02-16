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
using Microsoft.Phone.Net.NetworkInformation;
using System.Windows.Data;
using System.Globalization;
using System.ComponentModel;
using Microsoft.Phone.Shell;
using ClarolineApp.ViewModel;

namespace ClarolineApp.Settings
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        PropertyChangedEventHandler Handler;
        PropertyChangedEventHandler Refresh;
        PropertyChangedEventHandler ResetHandler;

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

        ClarolineViewModel _viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            _viewModel = new ClarolineViewModel();
            this.DataContext = AppSettings.instance;


            Handler = new PropertyChangedEventHandler(Connecting_PropertyChanged);
            Refresh = new PropertyChangedEventHandler(Client_PropertyChanged);
            ResetHandler = new PropertyChangedEventHandler(settings_PropertyChanged);

#if DEBUG
            PivotItem devPivot = new PivotItem()
            {
                Header = "[DEV]",
                Margin = new Thickness(12, 28, 12, 0),
                Content = new StackPanel()
                {
                    VerticalAlignment = System.Windows.VerticalAlignment.Bottom
                }
            };
            
            Button QAccBut = new Button(){ Content = "Charge QAcc"};
            QAccBut.Click += new RoutedEventHandler(QAccBut_Click);
            Button SAccBut = new Button(){ Content = "Charge SAcc"};
            SAccBut.Click += new RoutedEventHandler(SAccBut_Click);
            Button Button = new Button(){ Content = "Test Connect"};
            Button.Click +=new RoutedEventHandler(Button_Click);
            Button Button_1 = new Button(){ Content = "Test WebServ."};
            Button_1.Click += new RoutedEventHandler(Button_Click_1);
            
            ((StackPanel)devPivot.Content).Children.Add(QAccBut);
            ((StackPanel)devPivot.Content).Children.Add(SAccBut);
            ((StackPanel)devPivot.Content).Children.Add(Button);
            ((StackPanel)devPivot.Content).Children.Add(Button_1);
            SettingsPivot.Items.Add(devPivot);
#endif
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            userTextBox.Text = "";
            passwordBox.Password = "";
            _viewModel.ResetViewModel();
            ClaroClient.instance.invalidateClient();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            if (AppSettings.instance.UserSetting.userID > 0)
            {
                updatePanels(false);
                AppSettings.instance.PropertyChanged += ResetHandler;
            }
            else
            {
                updatePanels(true);
            }
            if (!AppSettings.instance.AdvancedSwitchSetting)
            {
                stackPanel2.Height = 0;
            }
            ClaroClient.instance.PropertyChanged += Refresh;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        void Client_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (ClaroClient.instance.isValidAccountWithoutWaiting())
            {
                updatePanels(false);
                ClaroClient.instance.PropertyChanged -= Refresh;
                AppSettings.instance.PropertyChanged -= ResetHandler;
            }
            else
            {
                updatePanels(true);
            }
        }

#if DEBUG
        private void QAccBut_Click(object sender, RoutedEventArgs e)
        {
            userTextBox.Text = "devosq";
            passwordBox.Password = "Elegie24";
        }

        private void SAccBut_Click(object sender, RoutedEventArgs e)
        {
            userTextBox.Text = "sluysmanss";
            passwordBox.Password = "kids4747";
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            ((Button)e.OriginalSource).Content = ClaroClient.instance.isValidAccountWithoutWaiting().ToString();
        }
        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            indicator.IsVisible = true;
            await ClaroClient.instance.makeOperationAsync(SupportedModules.USER, SupportedMethods.getCourseList);
            indicator.IsVisible = false;
        }
#endif

        void Connecting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "isExpired")
            {
                ClaroClient.instance.PropertyChanged -= Handler;
                if (ClaroClient.instance.isValidAccountWithoutWaiting())
                {
                    Connected.Begin();
                	AppSettings.instance.PropertyChanged += ResetHandler;
                	ClaroClient.instance.PropertyChanged += Refresh;
                }
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            indicator.IsVisible = true;
            ClaroClient.instance.PropertyChanged += Handler;
            await _viewModel.GetUserDataAsync();
            indicator.IsVisible = false;
        }

        private void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            string[] _listened = new String[] { AppSettings.UsernameSettingKeyName, AppSettings.PasswordSettingKeyName, AppSettings.WebServiceSettingKeyName, AppSettings.DomainSettingKeyName };

            if (_listened.Contains(e.PropertyName))
            {
                ClaroClient.instance.invalidateClient();
                Disconnected.Begin();
                updatePanels(true);
                AppSettings.instance.PropertyChanged -= ResetHandler;
            }
        }

        private void toggleSwitch_Checked(object sender, RoutedEventArgs e)
        {
            ActivateAdvanced.Begin();
        }

        private void toggleSwitch_Unchecked(object sender, RoutedEventArgs e)
        {
            DeactivateAdvanced.Begin();
        }

        private void updatePanels(Boolean status)
        {
            if (status)
            {
                //Display Connect Page & Address Page
                ConnectPage.Opacity = 100;
                ConnectPage.Visibility = System.Windows.Visibility.Visible;
                userTextBox.IsEnabled = true;
                passwordBox.IsEnabled = true;
                Connect.IsEnabled = true;

                AddressPage.Opacity = 100;
                AddressPage.Visibility = System.Windows.Visibility.Visible;

                UserPage.Opacity = 0;
                UserPage.Visibility = System.Windows.Visibility.Collapsed;

                PlatformPage.Opacity = 0;
                PlatformPage.Visibility = System.Windows.Visibility.Collapsed;
            }
            else
            {
                //Display User Page & Platform Page
                ConnectPage.Opacity = 0;
                ConnectPage.Visibility = System.Windows.Visibility.Collapsed;
                userTextBox.IsEnabled = false;
                passwordBox.IsEnabled = false;
                Connect.IsEnabled = false;

                AddressPage.Opacity = 0;
                AddressPage.Visibility = System.Windows.Visibility.Collapsed;

                UserPage.Opacity = 100;
                UserPage.Visibility = System.Windows.Visibility.Visible;

                PlatformPage.Opacity = 100;
                PlatformPage.Visibility = System.Windows.Visibility.Visible;
                
            }

        }

        private void Deco_Click(object sender, EventArgs e)
        {
            ClaroClient.instance.invalidateClient();
            Disconnected.Begin();
            updatePanels(true);
        }
    }
}