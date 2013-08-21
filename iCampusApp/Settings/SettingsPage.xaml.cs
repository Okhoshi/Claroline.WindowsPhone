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
using ClarolineApp.VM;

namespace ClarolineApp.Settings
{
    public partial class SettingsPage : PhoneApplicationPage
    {
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

        ClarolineVM _viewModel;

        public SettingsPage()
        {
            InitializeComponent();

            _viewModel = new ClarolineVM();
            this.DataContext = _viewModel;

            ClarolineVM.Settings.PropertyChanged += (sender, e) =>
            {
                string[] _listened = new String[] { AppSettings.UserNameSettingKeyName, 
                                                    AppSettings.PasswordSettingKeyName, 
                                                    AppSettings.WebServiceSettingKeyName, 
                                                    AppSettings.DomainSettingKeyName };

                if (_viewModel.IsConnected && _listened.Contains(e.PropertyName))
                {
                    _viewModel.ResetViewModel();
                }
            };

            Deco = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            Deco.IsEnabled = _viewModel.IsConnected;

            _viewModel.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsConnected")
                {
                    Deco.IsEnabled = _viewModel.IsConnected;
                }
            };

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

            Button QAccBut = new Button() { Content = "Charge QAcc" };
            QAccBut.Click += new RoutedEventHandler(QAccBut_Click);
            Button SAccBut = new Button() { Content = "Charge SAcc" };
            SAccBut.Click += new RoutedEventHandler(SAccBut_Click);
            Button Button = new Button() { Content = "Test Connect" };
            Button.Click += new RoutedEventHandler(Button_Click);

            ((StackPanel)devPivot.Content).Children.Add(QAccBut);
            ((StackPanel)devPivot.Content).Children.Add(SAccBut);
            ((StackPanel)devPivot.Content).Children.Add(Button);
            SettingsPivot.Items.Add(devPivot);
#endif
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            _viewModel.ResetViewModel();
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
            ((Button)e.OriginalSource).Content = ClaroClient.Instance.IsValidAccountSync().ToString();
        }
#endif

        private async void Connect_Button(object sender, RoutedEventArgs e)
        {
            indicator.IsVisible = true;
            bool r = await _viewModel.GetUserDataAsync();
            if (r)
            {
                await _viewModel.GetCoursListAsync();
            }
            indicator.IsVisible = false;
        }

        private void Deco_Click(object sender, EventArgs e)
        {
            string url = ClarolineVM.Settings.DomainSetting;
            _viewModel.ResetViewModel();
            ClarolineVM.Settings.DomainSetting = url;
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                AppSettings.Instance.PasswordSetting = (sender as PasswordBox).Password;
                Connect_Button(sender, null);
            }
        }
    }
}