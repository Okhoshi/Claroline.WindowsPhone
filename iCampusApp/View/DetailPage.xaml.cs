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
using ClarolineApp.Settings;
using Microsoft.Phone.Shell;
using ClarolineApp.VM;
using System.ComponentModel;
using System.Windows.Navigation;

namespace ClarolineApp
{
    public partial class DetailPage : PhoneApplicationPage
    {
        IDetailPageVM _viewModel;

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

        public DetailPage()
        {
            InitializeComponent();
            ClaroClient.Instance.PropertyChanged += ClaroClient_propertyChanged;
        }

        private void ClaroClient_propertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsInSync":
                    indicator.IsVisible = (sender as ClaroClient).IsInSync;
                    break;
                default:
                    break;
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            int resid, listid;

            if (parameters.ContainsKey("resource") && int.TryParse(parameters["resource"], out resid)
             && parameters.ContainsKey("list") && int.TryParse(parameters["list"], out listid))
            {
                
                _viewModel = new DetailPageVM(resid, listid);
                this.DataContext = _viewModel;

                _viewModel.RefreshAsync();

                base.OnNavigatedTo(e);
            }
            else
            {
                NavigationService.GoBack();
            }
        }
    }
}