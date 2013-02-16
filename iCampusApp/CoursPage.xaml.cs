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
using System.Collections.ObjectModel;
using System.ComponentModel;
using ClarolineApp.Model;
using Microsoft.Phone.Tasks;
using Microsoft.Phone.Shell;
using ClarolineApp.Settings;
using ClarolineApp.Languages;
using System.Windows.Navigation;

namespace ClarolineApp
{
    public partial class CoursPage : PhoneApplicationPage
    {

        private CL_Document rootDoc;
        private Cours _cours;

        private ICoursPageViewModel _viewModel;

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

            //Supprime les PivotItems non inclus dans le cours

            if (!_cours.Resources.Any(rl => rl.ressourceType.Equals(typeof(CL_Document))) || SectionsPivot.Items.Any(p => ((PivotItem)p).Name == "CLDOC"))
            {
                SectionsPivot.Items.Remove(SectionsPivot.Items.Single(p => ((PivotItem)p).Name == "CLDOC"));
                rootButton.IsEnabled = false;
            }
            else
            {
                if (rootDoc.title == null)
                {
                    rootButton.IsEnabled = false;
                }
            }

            if (!_cours.Resources.Any(rl => rl.ressourceType.Equals(typeof(CL_Annonce))) || (SectionsPivot.Items.Any(p => ((PivotItem)p).Name == "Ann")))
            {
                SectionsPivot.Items.Remove(SectionsPivot.Items.Single(p => ((PivotItem)p).Name == "Ann"));
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
                _viewModel = new CoursPageViewModel(parameters["cours"]);
                this.DataContext = _viewModel;
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
            
        }

        private void DocContent_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DocContent.SelectedIndex == -1)
            {
                return;
            }

            (DocContent.SelectedItem as CL_Document).seenDate = DateTime.Now;
            
            if ((DocContent.SelectedItem as CL_Document).isFolder)
            {
                rootDoc = DocContent.SelectedItem as CL_Document;
                CLDOC.Header = rootDoc.title.ToLower();
                rootButton.IsEnabled = true;

                DocContent.ItemsSource = rootDoc.getContent();
                DocContent.SelectedIndex = -1;
            }
            else
            {
                //Dwl Doc
            }
        }

        private void rootButton_Click(object sender, EventArgs e)
        {
            CL_Document _root = rootDoc.getRoot();
            DocContent.ItemsSource = _root.getContent();

            if (_root.title == null)
            {
                CLDOC.Header = AppLanguage.CoursPage_Doc_PI;
                rootButton.IsEnabled = false;
            }
            else
            {
                CLDOC.Header = _root.title.ToLower();
                rootButton.IsEnabled = true;
            }
            rootDoc = _root;
        }

        private async void refrButton_Click(object sender, EventArgs e)
        {
            _indicator.Text = AppLanguage.ProgressBar_Update;
            _indicator.IsVisible = true;
            await _viewModel.RefreshAsync();
            _indicator.IsVisible = false;
        }

        private void AnnList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnnList.SelectedIndex == -1)
            {
                return;
            }

            (AnnList.SelectedItem as CL_Annonce).seenDate = DateTime.Now;
            string destination = String.Format("/AnnonceDetail.xaml?resource={0}", (AnnList.SelectedItem as CL_Annonce).resourceId);
            AnnList.SelectedIndex = -1;

            NavigationService.Navigate(new Uri(destination, UriKind.Relative));
        }

        private void Pivot_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (SectionsPivot.SelectedItem.Equals(CLDOC) && rootDoc.title != null)
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