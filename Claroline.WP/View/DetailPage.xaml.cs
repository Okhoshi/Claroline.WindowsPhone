using ClarolineApp.Languages;
using ClarolineApp.Model;
using ClarolineApp.Settings;
using ClarolineApp.VM;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Practices.ServiceLocation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Markup;
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
            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            int listid;

            if (parameters.ContainsKey("resource")
             && parameters.ContainsKey("list") && int.TryParse(parameters["list"], out listid))
            {

                _viewModel = new DetailPageVM(ServiceLocator.Current.GetInstance<ISettings>(), parameters["resource"], listid);
                this.DataContext = _viewModel;
                _viewModel.PropertyChanged += VM_PropertyChanged;
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

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            base.OnNavigatedFrom(e);
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

        private string LastUri;

        protected async void WB_Navigating(object sender, NavigatingEventArgs e)
        {
            e.Cancel = true;

            LastUri = e.Uri.AbsoluteUri.Replace("about:", LastUri == null ? _viewModel.Settings.DomainSetting + "/" : LastUri.Substring(0, LastUri.LastIndexOf("/") + 1));
        
            string page = await ViewModelLocator.Client.MakeOperationAsync(SupportedModules.NOMOD, SupportedMethods.GetPage, genMod: LastUri);

            if (page == "" && ViewModelLocator.Client.LastException != null)
            {
                page = String.Format(AppLanguage.ErrorMessage_ServerReturnCode, "000", ViewModelLocator.Client.LastException);
                ViewModelLocator.Client.LastException = null;
            } else {
                //// Replace the matched text in the InputText using the replacement pattern
                page = regex.Replace(page, regexReplace);
                page = page.Replace("</head>", "<meta name=\"viewport\" Content=\"width=" + (sender as WebBrowser).ActualWidth + "\"/>\n</head>");
                page = page.Replace("=\"/", "=\"" + _viewModel.Settings.DomainSetting.Substring(0, _viewModel.Settings.DomainSetting.LastIndexOf("/") + 1));
            }
            (sender as WebBrowser).NavigateToString(page);
        }

        private void ListBoxItem_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            ListBoxItem lbi = sender as ListBoxItem;
            Post p = lbi.DataContext as Post;
            NavigationService.Navigate(new Uri(String.Format("/View/TopicPage.xaml?forum={0}&topic={1}&post={2}", p.Topic.Forum.UniqueIdentifier, p.Topic.resourceId, p.Id), UriKind.Relative));
        }
    }
}