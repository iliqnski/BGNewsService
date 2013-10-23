using BGNewsService.Common;
using BGNewsService.DataModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Windows.ApplicationModel.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Search Contract item template is documented at http://go.microsoft.com/fwlink/?LinkId=234240

namespace BGNewsService.Views
{
    /// <summary>
    /// This page displays search results when a global search is directed to this application.
    /// </summary>
    public sealed partial class SearchResultsPage : LayoutAwarePage
    {
        SearchPane searchPane;

        public SearchResultsPage()
        {
            this.InitializeComponent();
            searchPane = SearchPane.GetForCurrentView();
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
            var queryText = navigationParameter as String;

            var filterList = new List<Filter>();
            filterList.Add(new Filter("All", 0, true));

            if (!String.IsNullOrEmpty(queryText))
            {
                var formattedQueryText = queryText[0].ToString().ToUpper() +
                    queryText.Substring(1);

                var filteredGroups = AllNewsGroupsViewModel.GetGroups("AllGroups")
                    .Where(g => g.Title == formattedQueryText || g.Title.Contains(formattedQueryText));

                this.DefaultViewModel["Results"] = filteredGroups.ToList(); 
                this.DefaultViewModel["QueryText"] = '\u201c' + queryText + '\u201d';
                this.DefaultViewModel["Filters"] = filterList;
                this.DefaultViewModel["ShowFilters"] = filterList.Count > 1;
            }
        }

        /// <summary>
        /// Invoked when a filter is selected using the ComboBox in snapped view state.
        /// </summary>
        /// <param name="sender">The ComboBox instance.</param>
        /// <param name="e">Event data describing how the selected filter was changed.</param>
        void Filter_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // Determine what filter was selected
            var selectedFilter = e.AddedItems.FirstOrDefault() as Filter;
            if (selectedFilter != null)
            {
                // Mirror the results into the corresponding Filter object to allow the
                // RadioButton representation used when not snapped to reflect the change
                selectedFilter.Active = true;

                // Ensure results are found
                object results;
                ICollection resultsCollection;
                if (this.DefaultViewModel.TryGetValue("Results", out results) &&
                    (resultsCollection = results as ICollection) != null &&
                    resultsCollection.Count != 0)
                {
                    VisualStateManager.GoToState(this, "ResultsFound", true);
                    return;
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
            // Mirror the change into the CollectionViewSource used by the corresponding ComboBox
            // to ensure that the change is reflected when snapped
            if (filtersViewSource.View != null)
            {
                var filter = (sender as FrameworkElement).DataContext;
                filtersViewSource.View.MoveCurrentTo(filter);
            }
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            searchPane.SuggestionsRequested += searchPane_SuggestionsRequested;
        }

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            base.OnNavigatingFrom(e);
            searchPane.SuggestionsRequested -= searchPane_SuggestionsRequested;
        }

        void searchPane_SuggestionsRequested(SearchPane sender, SearchPaneSuggestionsRequestedEventArgs args)
        {
            var groups = AllNewsGroupsViewModel.GetGroups("AllGroups");

            args.Request.SearchSuggestionCollection
                .AppendQuerySuggestions((from gr in groups
                                        .Where(g => g.Title == args.QueryText.ToLower() || g.Title.Contains(args.QueryText.ToLower()))
                                         orderby gr.Title ascending
                                         select gr.Title).Take(5));
        }

        private void ResultsGridViewItemClick(object sender, ItemClickEventArgs e)
        {
            NewsGroupViewModel clickedModel = e.ClickedItem as NewsGroupViewModel;
            this.Frame.Navigate(typeof(GroupDetailPage), clickedModel.UniqueId);
        }

        /// <summary>
        /// View model describing one of the filters available for viewing search results.
        /// </summary>
        private sealed class Filter : BindableBase
        {
            private String name;
            private int count;
            private bool active;

            public Filter(String name, int count, bool active = false)
            {
                this.Name = name;
                this.Count = count;
                this.Active = active;
            }

            public override String ToString()
            {
                return Description;
            }

            public String Name
            {
                get { return name; }
                set { if (this.SetProperty(ref name, value)) this.OnPropertyChanged("Description"); }
            }

            public int Count
            {
                get { return count; }
                set { if (this.SetProperty(ref count, value)) this.OnPropertyChanged("Description"); }
            }

            public bool Active
            {
                get { return active; }
                set { this.SetProperty(ref active, value); }
            }

            public String Description
            {
                get { return String.Format("{0} ({1})", name, count); }
            }
        }
    }
}
