using ClarolineApp.Languages;
using ClarolineApp.VM;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace ClarolineApp
{
    public partial class CoursPage : PhoneApplicationPage
    {

        private ICoursPageVM _viewModel;

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

        public CoursPage()
        {
            InitializeComponent();
            this.Language = XmlLanguage.GetLanguage(Thread.CurrentThread.CurrentCulture.Name);
            rootButton = ApplicationBar.Buttons[0] as ApplicationBarIconButton;
            ClaroClient.instance.PropertyChanged += ClaroClient_PropertyChanged;
        }

        private void ClaroClient_PropertyChanged(object sender, PropertyChangedEventArgs e)
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

        //--------------------------------------------------------------------
        // Event handler
        //--------------------------------------------------------------------

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;

            if (parameters.ContainsKey("cours"))
            {
                _viewModel = new CoursPageVM(parameters["cours"]);
                this.DataContext = _viewModel;
                _viewModel.loadedInView = true;
                _viewModel.PropertyChanged += _viewModel_PropertyChanged;

                base.OnNavigatedTo(e);
            }
            else
            {
                NavigationService.GoBack();
            }
        }

        void _viewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "root":
                    rootButton.IsEnabled = !_viewModel.IsOnRoot();
                    //Update the path somewhere
                    break;
                case "navigationTarget":
                    NavigationService.Navigate(_viewModel.GetNavigationTarget());
                    break;
                default:
                    break;
            }
        }

        private void rootButton_Click(object sender, EventArgs e)
        {
            if (_viewModel.IsDocumentPivotSelected(SectionsPivot.SelectedItem))
            {
                _viewModel.GoUp();
            }
        }

        private async void refrButton_Click(object sender, EventArgs e)
        {
            _indicator.Text = AppLanguage.ProgressBar_Update;
            _indicator.IsVisible = true;
            await _viewModel.RefreshAsync();
            _indicator.IsVisible = false;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (rootButton != null)
            {
                if (_viewModel.IsDocumentPivotSelected(SectionsPivot.SelectedItem)
                    && !_viewModel.IsOnRoot())
                {
                    rootButton.IsEnabled = true;
                }
                else
                {
                    rootButton.IsEnabled = false;
                }
            }
        }
    }
}