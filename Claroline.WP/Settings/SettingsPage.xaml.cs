﻿using System;
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
using System.IO;
using Newtonsoft.Json;
using ClarolineApp.Languages;
using Microsoft.Practices.ServiceLocation;

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

        public ISettings Settings
        {
            get
            {
                return ServiceLocator.Current.GetInstance<ISettings>();
            }
        }

        public SettingsPage()
        {
            InitializeComponent();

            _viewModel = this.DataContext as ClarolineVM;

            ViewModelLocator.Client.Settings.PropertyChanged += (sender, e) =>
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

            ViewModelLocator.Client.Settings.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "IsInSync")
                {
                    indicator.IsVisible = ViewModelLocator.Client.IsInSync;
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

            if (Settings.IsValidHostSetting)
            {
                VisualStateManager.GoToState(this, "Valid", true);
            }
        }

        private void Reset_Click(object sender, EventArgs e)
        {
            _viewModel.ResetViewModel();
        }

        private async void Connect_Button(object sender, RoutedEventArgs e)
        {
            bool r = ClarolineClient.IsNetworkAvailable();
            if (r)
            {
                r = await ViewModelLocator.Client.IsValidAccountAsync();
                if (r)
                {
                    string response = await _viewModel.CheckModuleValidity();
                    if (response != "{}")
                    {
                        JsonTextReader reader = new JsonTextReader(new StringReader(response));
                        while (reader.Read())
                        {
                            if (reader.Value != null && reader.TokenType == JsonToken.PropertyName)
                            {
                                switch (reader.Value.ToString())
                                {
                                    case "version":
                                        if (reader.Read() && reader.TokenType == JsonToken.Integer)
                                        {
                                            int version;
                                            if (!int.TryParse(reader.Value.ToString(), out version) || App.ModuleVersionRequired > version)
                                            {
                                                ViewModelLocator.Client.InvalidateClient();
                                                ShowMessage(AppLanguage.ErrorMessage_OutdatedModule); //Not readable version or outdated one
                                            }
                                        }
                                        else
                                        {
                                            ViewModelLocator.Client.InvalidateClient();
                                            ShowMessage(AppLanguage.ErrorMessage_UnreadableJson); // Unreadable
                                        }
                                        break;
                                    case "path":
                                        if (reader.Read() && reader.TokenType == JsonToken.String)
                                        {
                                            Settings.WebServiceSetting = reader.Value.ToString();
                                        }
                                        else
                                        {
                                            ViewModelLocator.Client.InvalidateClient();
                                            ShowMessage(AppLanguage.ErrorMessage_UnreadableJson); // Unreadable
                                        }
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }

                        if (ViewModelLocator.Client.IsValidAccountSync())
                        {
                            _viewModel.GetUserDataAsync();
                            _viewModel.GetCoursListAsync();
                        }
                    }
                    else
                    {
                        ViewModelLocator.Client.InvalidateClient();
                        ShowMessage(AppLanguage.ErrorMessage_MissingModule); // Missing Module
                    }
                }
                else
                {
                    ShowMessage(AppLanguage.ErrorMessage_AuthFailed, true); // Bad password
                }
            }
            else
            {
                ShowMessage(AppLanguage.ErrorMessage_NetworkUnavailable); //Missing Network
            }
        }

        private void ShowMessage(string message, bool concernPassword = false)
        {
            if (message != null)
            {
                VisualStateManager.GoToState(this, concernPassword ? "PasswordError" : "Error", true);
                userError.Text = message;
            }
            else
            {
                VisualStateManager.GoToState(this, "UserDefault", true);
            }
        }

        private void Deco_Click(object sender, EventArgs e)
        {
            string url = ViewModelLocator.Client.Settings.DomainSetting;
            _viewModel.ResetViewModel();
            ViewModelLocator.Client.Settings.DomainSetting = url;
        }

        private void passwordBox_KeyDown(object sender, KeyEventArgs e)
        {
            ShowMessage(null);
            if (e.Key == Key.Enter && Settings.IsValidHostSetting)
            {
                Settings.PasswordSetting = (sender as PasswordBox).Password;
                Connect_Button(sender, null);
            }
        }

        private async void Validate_Click(object sender, RoutedEventArgs e)
        {
            string url = SiteTextBox.Text;
            errorDetail.Text = AppLanguage.ErrorMessage_NotAClarolinePlatform;

            if (Settings.UseSSLSetting)
            {
                url = url.Replace("http://", "https://");
            }
            bool isHostValid = await _viewModel.CheckHostValidity(url);
            if (Settings.UseSSLSetting && !isHostValid)
            {
                if (Settings.TryHTTPSetting)
                {
                    url = url.Replace("https://", "http://");
                    isHostValid = await _viewModel.CheckHostValidity(url);
                }
                else
                {
                    errorDetail.Text = AppLanguage.ErrorMessage_NoSSLAvailable;
                }
            }

            Settings.DomainSetting = url;
            Settings.IsValidHostSetting = isHostValid;
            VisualStateManager.GoToState(this, isHostValid ? "Valid" : "Invalid", true);
        }

        private void SiteTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (!Settings.DomainSetting.Equals(SiteTextBox.Text))
            {
                VisualStateManager.GoToState(this, "Default", true);
                Settings.IsValidHostSetting = false;
            }
        }

        private void SiteTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter && !Settings.IsValidHostSetting)
            {
                Settings.DomainSetting = (sender as TextBox).Text;
                Validate_Click(sender, null);
            }
        }
    }
}