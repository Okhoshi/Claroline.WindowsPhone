using ClarolineApp.RT.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.ApplicationModel.Activation;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// Pour en savoir plus sur le modèle d'élément Contrat de recherche, consultez la page http://go.microsoft.com/fwlink/?LinkId=234240

namespace ClarolineApp.RT
{
    /// <summary>
    /// Cette page affiche les résultats d'une recherche globale effectuée dans cette application.
    /// </summary>
    public sealed partial class SearchResultsPage1 : ClarolineApp.RT.Common.LayoutAwarePage
    {
        List<ItemModel> results;

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchResultsPage" /> class.
        /// </summary>
        public SearchResultsPage1()
        {
            this.InitializeComponent();

            ApplicationModel.Current.Synchronized += (sender, e) =>
            {
                FilterSelection(this.DefaultViewModel["QueryPattern"] as string);
            };
        }

        private void FilterSelection(string queryText)
        {
            //
            // create filter
            //
            var filterList = new List<Filter>();
            filterList.Add(new Filter("All", "All", 0, true));
            filterList.Add(new Filter("Annonces", "annonce", 0));
            filterList.Add(new Filter("Documents", "document", 0));
            //
            // adapt view model
            //
            this.DefaultViewModel["QueryPattern"] = queryText;
            this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';
            this.DefaultViewModel["Filters"] = filterList;
            this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
        }


        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="navigationParameter">The parameter value passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested.
        /// </param>
        /// <param name="pageState">A dictionary of state preserved by this page during an earlier
        /// session.  This will be null the first time a page is visited.</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            //
            // save
            //
            this.DefaultViewModel["QueryPattern"] = Convert.ToString(navigationParameter);
            //
            // set
            //
            this.DefaultViewModel["Application"] = ApplicationModel.Current;
            //
            // just filter
            //
            FilterSelection(this.DefaultViewModel["QueryPattern"] as string);

            this.DefaultViewModel["Results"] = results;
        }

        void RefreshFilter(Filter filter = null)
        {
            foreach (Filter f in this.DefaultViewModel["Filters"] as List<Filter>)
            {
                switch (f.Name)
                {
                    case "All":
                        f.Count = results.Count();
                        if(f == filter)
                            this.DefaultViewModel["Results"] = results;
                        break;
                    default:
                        f.Count = results.Count(val => val.Group.ID.Equals(f.Id));
                        if(f == filter)
                            this.DefaultViewModel["Results"] = results.Where(val => val.Group.ID.Equals(filter.Id));
                        break;
                }
            }

        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
        /// <param name="e">Event data describing how the selected filter was changed.</param>
        void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ApplicationModel.Current.IsSynchronizing)
            {
                VisualStateManager.GoToState(this, "Synchronizing", true);
                return;
            }
            else
            {

                // Determine what filter was selected
                var selectedFilter = e.AddedItems.FirstOrDefault() as Filter;
                if (selectedFilter != null)
                {
                    // Mirror the results into the corresponding Filter object to allow the
                    // RadioButton representation used when not snapped to reflect the change
                    selectedFilter.Active = true;

                    // TODO: Respond to the change in active filter by setting this.DefaultViewModel["Results"]
                    //       to a collection of items with bindable Image, Title, Subtitle, and Description properties
                    string pattern = Convert.ToString(this.DefaultViewModel["QueryPattern"]);
                    results = ApplicationModel.Current.Search(pattern);
                    RefreshFilter(selectedFilter);

                    // Ensure results are found
                    if (results.Count > 0)
                    {
                        VisualStateManager.GoToState(this, "ResultsFound", true);
                        return;
                    }
                }
            }

            // Display informational text when there are no search results.
            VisualStateManager.GoToState(this, "NoResultsFound", true);
        }

        /// <summary>
        /// Invoked when a filter is selected using a RadioButton when not snapped.
        /// </summary>
        /// <param name="sender">The selected RadioButton instance.</param>
        /// <param name="e">Event data describing how the RadioButton was selected.</param>
        void Filter_Checked(object sender, RoutedEventArgs e)
        {
            Filter filter = (sender as FrameworkElement).DataContext as Filter;
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                filtersViewSource.View.MoveCurrentTo(filter);
            }
            RefreshFilter(filter);
        }

        /// <summary>
        /// Invoked when an item within a group is clicked.
        /// </summary>
        /// <param name="sender">The GridView (or ListView when the application is snapped)
        /// displaying the item clicked.</param>
        /// <param name="e">Event data that describes the item clicked.</param>
        void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            //
            // Navigate to the appropriate destination page, configuring the new page
            // by passing required information as a navigation parameter
            //
            this.Frame.Navigate(typeof(CoursPage), ((ResourceModel<Cours>)e.ClickedItem).ID);
        } //TODO il etait mis ItemModel a la place de ResourceModel<Cours>

        /// <summary>
        /// View model describing one of the filters available for viewing search results.
        /// </summary>
        private sealed class Filter : ClarolineApp.RT.Common.BindableBase
        {
            private String _name;
            private int _count;
            private bool _active;
            private String _id;

            /// <summary>
            /// Initializes a new instance of the <see cref="Filter" /> class.
            /// </summary>
            /// <param name="name">The name.</param>
            /// <param name="count">The count.</param>
            /// <param name="active">if set to <c>true</c> [active].</param>
            public Filter(String name, String id, int count, bool active = false)
            {
                this.Name = name;
                this.Count = count;
                this.Active = active;
                this.Id = id;
            }

            /// <summary>
            /// To the string.
            /// </summary>
            /// <returns></returns>
            public override String ToString()
            {
                return Description;
            }

            /// <summary>
            /// Gets or sets the name.
            /// </summary>
            /// <value>
            /// The name.
            /// </value>
            public String Name
            {
                get { return _name; }
                set { if (this.SetProperty(ref _name, value)) this.OnPropertyChanged("Description"); }
            }

            /// <summary>
            /// Gets or sets the id.
            /// </summary>
            /// <value>
            /// The id.
            /// </value>
            public String Id
            {
                get { return _id; }
                set { this.SetProperty(ref _id, value); }
            }

            /// <summary>
            /// Gets or sets the count.
            /// </summary>
            /// <value>
            /// The count.
            /// </value>
            public int Count
            {
                get { return _count; }
                set { if (this.SetProperty(ref _count, value)) this.OnPropertyChanged("Description"); }
            }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="Filter" /> is active.
            /// </summary>
            /// <value>
            ///   <c>true</c> if active; otherwise, <c>false</c>.
            /// </value>
            public bool Active
            {
                get { return _active; }
                set { this.SetProperty(ref _active, value); }
            }

            /// <summary>
            /// Gets the description.
            /// </summary>
            /// <value>
            /// The description.
            /// </value>
            public String Description
            {
                get { return String.Format("{0} ({1})", _name, _count); }
            }
        }
    }

}
