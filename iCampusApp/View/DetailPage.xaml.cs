using ClarolineApp.Model;
using ClarolineApp.Settings;
using ClarolineApp.VM;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
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

        public static Regex regex = new Regex("(?:<div id=\"claroPage\">\\P{Cn}*?<div id=\"courseRightContent\">" +
                                              "(?<Content>\\P{Cn}+?)</div>\\s*?<!-- rightContent --" +
                                              ">\\P{Cn}*?<!-- end of claroPage -->\\P{Cn}*?</div>)",
                                              RegexOptions.CultureInvariant | RegexOptions.Compiled);

        // This is the replacement string
        public static string regexReplace = "<div id=\"claroPage\">${Content}<!-- end of claroPage --></div>";

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

                if (_viewModel.currentResource is Document)
                {
                    (_viewModel.currentResource as Document).OpenDocumentAsync();
                    NavigationService.GoBack();
                }

                _viewModel.RefreshAsync();

                base.OnNavigatedTo(e);
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        protected async void WB_Navigating(object sender, NavigatingEventArgs e)
        {
            e.Cancel = true;

            string page = await ClaroClient.Instance.MakeOperationAsync(SupportedModules.NOMOD, SupportedMethods.GetPage, genMod: e.Uri.AbsoluteUri.Replace("about:", AppSettings.Instance.DomainSetting));
            
            //// Replace the matched text in the InputText using the replacement pattern
            page = regex.Replace(page, regexReplace);
            page = page.Replace("</head>", "<meta name=\"viewport\" content=\"width=" + (sender as WebBrowser).ActualWidth + "\"/>\n</head>");
            page = page.Replace("=\"/", "=\"" + AppSettings.Instance.DomainSetting + "/");
            (sender as WebBrowser).NavigateToString(page);
        }
    }
}