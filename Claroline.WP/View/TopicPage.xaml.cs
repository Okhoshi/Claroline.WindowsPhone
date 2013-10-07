using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using ClarolineApp.VM;
using System.Windows.Markup;
using System.Threading;
using System.ComponentModel;

namespace ClarolineApp.View
{
    public partial class TopicPage : PhoneApplicationPage
    {
        ITopicPageVM _viewModel;

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

        public TopicPage()
        {
            InitializeComponent();
            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;
            int topicid, forumid;

            if (parameters.ContainsKey("topic") && int.TryParse(parameters["topic"], out topicid)
             && parameters.ContainsKey("forum") && int.TryParse(parameters["forum"], out forumid))
            {

                _viewModel = new TopicPageVM(topicid, forumid);
                this.DataContext = _viewModel;
                _viewModel.PropertyChanged += VM_PropertyChanged;

                int postid;
                if (parameters.ContainsKey("post") && int.TryParse(parameters["post"], out postid))
                {
                    var i = _viewModel.posts.FirstOrDefault(p => p.Id == postid);
                    Dispatcher.BeginInvoke(() =>
                    {
                        Posts.SelectedItem = i;
                        Posts.ScrollIntoView(i);
                    });
                }

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
    }
}