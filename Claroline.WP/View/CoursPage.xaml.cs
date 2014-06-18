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
using System.Diagnostics;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Navigation;

namespace ClarolineApp
{
    public partial class CoursPage : PhoneApplicationPage
    {

        private ICoursPageVM _viewModel;
        private ApplicationBarIconButton rootButton;

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
            ApplicationBar.Buttons.Add(new ApplicationBarIconButton()
            {
                IconUri = new Uri("/icons/appbar.lines.horizontal.4.png", UriKind.Relative),
                Text = AppLanguage.AppBar_Menu
            });
            (ApplicationBar.Buttons[0] as ApplicationBarIconButton).Click += menuButton_Click;
            
            ApplicationBar.Buttons.Add(new ApplicationBarIconButton()
            {
                IconUri = new Uri("/icons/appbar.arrow.up.png", UriKind.Relative),
                Text = AppLanguage.AppBar_GoUp
            });
            rootButton = ApplicationBar.Buttons[1] as ApplicationBarIconButton;
            rootButton.Click += rootButton_Click;

            ApplicationBar.Buttons.Add(new ApplicationBarIconButton()
            {
                IconUri = new Uri("/icons/appbar.refresh.rest.png", UriKind.Relative),
                Text = AppLanguage.AppBar_Refresh
            });
            (ApplicationBar.Buttons[2] as ApplicationBarIconButton).Click += refrButton_Click;
        }

        //--------------------------------------------------------------------
        // Event handler
        //--------------------------------------------------------------------

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            IDictionary<string, string> parameters = this.NavigationContext.QueryString;

            if (parameters.ContainsKey("cours"))
            {
                if (_viewModel == null)
                {
                    _viewModel = new CoursPageVM(ServiceLocator.Current.GetInstance<ISettings>(), parameters["cours"]);
                    this.DataContext = _viewModel;
                }

                _viewModel.PropertyChanged += VM_PropertyChanged;
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
            _viewModel.NavigationTarget = null;
            //_viewModel.PropertyChanged -= VM_PropertyChanged;
        }

        void VM_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "root":
                    rootButton.IsEnabled = !_viewModel.IsOnRoot();
                    //Update the path somewhere
                    break;
                case "NavigationTarget":
                    if (_viewModel.NavigationTarget != null)
                    {
                        NavigationService.Navigate(_viewModel.NavigationTarget);
                    }
                    break;
                case "IsLoading":
                    indicator.IsVisible = _viewModel.IsLoading;
                    break;
                default:
                    break;
            }
        }

        private void rootButton_Click(object sender, EventArgs e)
        {
            if (_viewModel.IsPivotSelectedOfType(SectionsPivot.SelectedItem, typeof(Document)))
            {
                _viewModel.GoUp();
            }
        }

        private async void refrButton_Click(object sender, EventArgs e)
        {
            indicator.Text = AppLanguage.ProgressBar_Update;
            indicator.IsVisible = true;
            await _viewModel.RefreshAsync(true);
            indicator.IsVisible = false;
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //Enabling root button routine
            if (rootButton != null)
            {
                if (_viewModel.IsPivotSelectedOfType(SectionsPivot.SelectedItem, typeof(Document))
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

        private void CLDOCList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox list = sender as ListBox;

            if (list.SelectedIndex == -1)
            {
                return;
            }

            if (_viewModel.IsPivotSelectedOfType(SectionsPivot.SelectedItem, typeof(Document)))
            {
                _viewModel.OnDocumentItemSelected(list.SelectedItem as Document);
            }

            list.SelectedIndex = -1;
        }

        private void LongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            LongListSelector list = sender as LongListSelector;

            if (list == null || list.SelectedItem == null)
            {
                return;
            }

            _viewModel.OnItemWithDetailsSelected(list.SelectedItem as ResourceModel);

            list.SelectedItem = null;
        }

        private void Menu_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = sender as ListBox;

            if (l.SelectedIndex == -1)
            {
                return;
            }

            SectionsPivot.SelectedItem = l.SelectedItem;

            l.SelectedIndex = -1;
        }

        private void menuButton_Click(object sender, EventArgs e)
        {
            SectionsPivot.SelectedIndex = 0;
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox l = sender as ListBox;

            if (l.SelectedIndex == -1)
            {
                return;
            }

            _viewModel.OnItemWithDetailsSelected(l.SelectedItem as ResourceModel);

            l.SelectedIndex = -1;
        }
    }
}