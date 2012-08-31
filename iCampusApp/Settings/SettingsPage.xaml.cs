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

namespace ClarolineApp.Settings
{
    public partial class SettingsPage : PhoneApplicationPage
    {
        PropertyChangedEventHandler Handler;
        PropertyChangedEventHandler Refresh;
        PropertyChangedEventHandler ResetHandler;

        AppSettings settings;

        public SettingsPage()
        {
            InitializeComponent();

            settings = new AppSettings();
            this.DataContext = settings;

            Handler = new PropertyChangedEventHandler(Connecting_PropertyChanged);
            Refresh = new PropertyChangedEventHandler(Client_PropertyChanged);
            ResetHandler = new PropertyChangedEventHandler(settings_PropertyChanged);

#if !DEBUG
            SettingsPivot.Items.Remove(DevPivot);
#endif
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            userTextBox.Text = "";
            passwordBox.Password = "";
            App.ViewModel.ResetDatabase();
            App.Client.invalidateClient();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            SystemTray.SetProgressIndicator(this, App.currentProgressInd);
            if (settings.UserSetting.userID > 0)
            {
                updatePanels(false);
                settings.PropertyChanged += ResetHandler;
            }
            else
            {
                updatePanels(true);
            }
            if (!settings.AdvancedSwitchSetting)
            {
                stackPanel2.Height = 0;
            }
            App.Client.PropertyChanged += Refresh;
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
        }

        void Client_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (App.Client.isValidAccount())
            {
                updatePanels(false);
                App.Client.PropertyChanged -= Refresh;
                settings.PropertyChanged -= ResetHandler;
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
            ((Button)e.OriginalSource).Content = App.Client.isValidAccount().ToString();
        }
#endif
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            App.Client.makeOperation(AllowedOperations.getCourseList);
        }

        void Connecting_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "isExpired")
            {
                if (App.Client.isValidAccount())
                {
                    Connected.Begin();
                	settings.PropertyChanged += ResetHandler;
                	App.Client.PropertyChanged += Refresh;
                }
                App.Client.PropertyChanged -= Handler;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            App.Client.PropertyChanged += Handler;
            App.Client.makeOperation(AllowedOperations.getUserData);
        }

        private void settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "AdvancedSwitchSetting")
            {
                return;
            }
            App.Client.invalidateClient();
			Disconnected.Begin();
            updatePanels(true);
            settings.PropertyChanged -= ResetHandler;
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
            App.Client.invalidateClient();
            Disconnected.Begin();
            updatePanels(true);
        }
    }
}