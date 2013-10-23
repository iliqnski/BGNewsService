using BGNewsService.Common;
using BGNewsService.DataPersisterModel;
using RSS;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

namespace BGNewsService.DataModel
{
    /// <summary>
    /// Generic model for all news groups.
    /// </summary>
    public sealed class AllNewsGroupsViewModel
    {
        private static AllNewsGroupsViewModel dataSource = new AllNewsGroupsViewModel();

        RSSClient rssClient = new RSSClient();

        #region RSS feeds urls
        //Business Group
        private string financialNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=15#sthash.tuMyH3kE.dpuf";
        private string energyNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=16#sthash.bimevWXA.dpuf";
        private string tourismNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=19#sthash.bimevWXA.dpuf";

        //Politics Group
        private string diplomacyNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=20#sthash.bimevWXA.dpuf";
        private string bulgariaInEuNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=22#sthash.bimevWXA.dpuf";
        private string domesticNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=23#sthash.1R5Kjvhg.dpuf";

        //Society Group
        private string environmentNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=24#sthash.bimevWXA.dpuf";
        private string educationNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=25#sthash.bimevWXA.dpuf";
        private string cultureNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=26#sthash.5HIeA5vS.dpuf";
        private string archaeologyNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=34#sthash.5HIeA5vS.dpuf";
        private string healthNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=62#sthash.5HIeA5vS.dpuf";
        private string obituariesNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=28#sthash.5HIeA5vS.dpuf";

        //Sports Group
        private string sportsNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=4#sthash.5HIeA5vS.dpuf";

        //Crime Group
        private string crimeNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=5#sthash.5HIeA5vS.dpuf";

        //Lifestyle Group
        private string lifestyleNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=6#sthash.5HIeA5vS.dpuf";

        //Views Group
        private string viewsNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=10#sthash.5HIeA5vS.dpuf";

        //World Group
        private string worldNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=30#sthash.5HIeA5vS.dpuf";
        #endregion

        #region News groups
        NewsGroupViewModel financeGroup = new NewsGroupViewModel("Business - 1",
                "Finance",
                DateTime.Now,
                "Assets/finance.jpg",
                "Latest news from Finance");
        NewsGroupViewModel energyGroup = new NewsGroupViewModel("Business - 2",
                "Energy",
                DateTime.Now,
                "Assets/energy.jpg",
                "Latest news from Energy");
        NewsGroupViewModel tourismGroup = new NewsGroupViewModel("Business - 3",
                "Tourism",
                DateTime.Now,
                "Assets/tourism.jpg",
                "Latest news from Tourism");


        NewsGroupViewModel diplomacyGroup = new NewsGroupViewModel("Politics - 1",
                "Diplomacy",
                DateTime.Now,
                "Assets/diplomacy.jpg",
                "Latest news from Diplomacy");
        NewsGroupViewModel bulgariaInEuGroup = new NewsGroupViewModel("Politics - 2",
                "Bulgaria In EU",
                DateTime.Now,
                "Assets/bulgaria in eu.jpg",
                "Latest news from Bulgaria in EU");
        NewsGroupViewModel domesticGroup = new NewsGroupViewModel("Politics - 3",
                 "Domestic",
                 DateTime.Now,
                 "Assets/domestic.jpg",
                 "Latest news from Domestic");

        NewsGroupViewModel environmentGroup = new NewsGroupViewModel("Society - 1",
                "Environment",
                DateTime.Now,
                "Assets/environment.jpg",
                "Latest news from Environment");
        NewsGroupViewModel educationGroup = new NewsGroupViewModel("Society - 2",
                "Education",
                DateTime.Now,
                "Assets/education.jpg",
                "Latest news from Education");
        NewsGroupViewModel cultureGroup = new NewsGroupViewModel("Society - 3",
               "Culture",
               DateTime.Now,
               "Assets/culture.jpg",
               "Latest news from Culture");
        NewsGroupViewModel archaeologyGroup = new NewsGroupViewModel("Society - 4",
               "Archaeology",
               DateTime.Now,
               "Assets/archaeology.jpg",
               "Latest news from Archaeology");
        NewsGroupViewModel healthGroup = new NewsGroupViewModel("Society - 5",
               "Health",
               DateTime.Now,
               "Assets/health.jpg",
               "Latest news from Health");
        NewsGroupViewModel obituariesGroup = new NewsGroupViewModel("Society - 6",
              "Obituaries",
              DateTime.Now,
              "Assets/obituaries.jpg",
              "Latest news from Obituaries");

        NewsGroupViewModel sportsGroup = new NewsGroupViewModel("Sports",
                "Sports",
                DateTime.Now,
                "Assets/sports.jpg",
                "Latest news from Sports");

        NewsGroupViewModel crimeGroup = new NewsGroupViewModel("Crime",
                "Crime",
                DateTime.Now,
                "Assets/crime.jpg",
                "Latest news from Crime");

        NewsGroupViewModel lifestyleGroup = new NewsGroupViewModel("Lifestyle",
                "Lifestyle",
                DateTime.Now,
                "Assets/lifestyle.jpg",
                "Latest news from Lifestyle");

        NewsGroupViewModel viewsGroup = new NewsGroupViewModel("Views",
                "Views on BG",
                DateTime.Now,
                "Assets/views on bg.jpg",
                "Latest news from Views on BG");

        NewsGroupViewModel worldGroup = new NewsGroupViewModel("World",
                "World",
                DateTime.Now,
                "Assets/world.jpg",
                "Latest news from World");
        #endregion

        private ObservableCollection<NewsGroupViewModel> allGroups = 
            new ObservableCollection<NewsGroupViewModel>();
        public ObservableCollection<NewsGroupViewModel> AllGroups
        {
            get { return this.allGroups; }
        }

        public static async Task PopulateGroupsFromLocalFolder()
        {
            await LoadDataFromLocalFolder(dataSource.AllGroups);
        }

        public static  IEnumerable<NewsGroupViewModel> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups"))
            {
                throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");
            }

            return dataSource.AllGroups;
        }

        public static NewsGroupViewModel GetGroup(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = dataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                return matches.First();
            }
            return null;
        }

        public static NewsItemViewModel GetItem(string uniqueId)
        {
            // Simple linear search is acceptable for small data sets
            var matches = dataSource.AllGroups.SelectMany(group => group.Items)
                .Where((item) => item.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1)
            {
                return matches.First();
            }
            return null;
        }

        public AllNewsGroupsViewModel()
        {
            GetArchaeologyNewsRssFeeds();
            GetBulgariaInEuNewsRssFeeds();
            GetCrimeNewsRssFeeds();
            GetCultureNewsRssFeeds();
            GetDiplomacyNewsRssFeeds();
            GetDomesticNewsRssFeeds();
            GetEducationNewsRssFeeds();
            GetEnergyNewsRssFeeds();
            GetEnvironmentNewsRssFeeds();
            GetFinancialNewsRssFeeds();
            GetHealthNewsRssFeeds();
            GetLifestyleNewsRssFeeds();
            GetObituariesNewsRssFeeds();
            GetSportsNewsRssFeeds();
            GetTourismNewsRssFeeds();
            GetViewsNewsRssFeeds();
            GetWorldNewsRssFeeds();

            this.AllGroups.Add(archaeologyGroup);
            this.AllGroups.Add(bulgariaInEuGroup);
            this.AllGroups.Add(crimeGroup);
            this.AllGroups.Add(cultureGroup);
            this.AllGroups.Add(diplomacyGroup);
            this.AllGroups.Add(domesticGroup);
            this.AllGroups.Add(educationGroup);
            this.AllGroups.Add(energyGroup);
            this.AllGroups.Add(environmentGroup);
            this.AllGroups.Add(financeGroup);
            this.AllGroups.Add(healthGroup);
            this.AllGroups.Add(lifestyleGroup);
            this.AllGroups.Add(obituariesGroup);
            this.AllGroups.Add(sportsGroup);
            this.AllGroups.Add(tourismGroup);
            this.AllGroups.Add(viewsGroup);
            this.AllGroups.Add(worldGroup);
        }

        #region Get feeds async methods
        //Get all financial news feeds
        private async void GetFinancialNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed financialNewsFeeds = await DataPersister.GetFeeds(financialNewsUrlString, rssClient);

                //Add items to financial group
                string altImagePath = "Assets/finance-item.jpg";
                AddItemsToGroup(financeGroup, financialNewsFeeds, altImagePath);
            }
        }

        //Get all energy news feeds
        private async void GetEnergyNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed energyNewsFeeds = await DataPersister.GetFeeds(energyNewsUrlString, rssClient);

                //Add items to energy group
                string altImagePath = "Assets/energy-item.jpg";
                AddItemsToGroup(energyGroup, energyNewsFeeds, altImagePath);
            }
        }

        //Get all tourism news feeds 
        private async void GetTourismNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed tourismNewsFeeds = await DataPersister.GetFeeds(tourismNewsUrlString, rssClient);

                //Add items to tourism group
                string altImagePath = "Assets/tourism-item.jpg";
                AddItemsToGroup(tourismGroup, tourismNewsFeeds, altImagePath);
            }
        }

        //Get all diplomacy news feeds
        private async void GetDiplomacyNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed diplomacyNewsFeeds = await DataPersister.GetFeeds(diplomacyNewsUrlString, rssClient);

                //Add items to diplomacy group
                string altImagePath = "Assets/diplomacy-item.jpg";
                AddItemsToGroup(diplomacyGroup, diplomacyNewsFeeds, altImagePath);
            }
        }

        //Get all Bulgaria in EU news feeds
        private async void GetBulgariaInEuNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed bulgariaInEuNewsFeeds = await DataPersister.GetFeeds(bulgariaInEuNewsUrlString, rssClient);

                //Add items to Bulgaria in EU group
                string altImagePath = "Assets/bulgaria in eu-item.jpg";
                AddItemsToGroup(bulgariaInEuGroup, bulgariaInEuNewsFeeds, altImagePath);
            }
        }

        //Get all domestic news feeds
        private async void GetDomesticNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed domesticNewsFeeds = await DataPersister.GetFeeds(domesticNewsUrlString, rssClient);

                //Add items to domestic group
                string altImagePath = "Assets/domestic-item.jpg";
                AddItemsToGroup(domesticGroup, domesticNewsFeeds, altImagePath);
            }
        }

        //Get all environment news feeds
        private async void GetEnvironmentNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed environmentNewsFeeds = await DataPersister.GetFeeds(environmentNewsUrlString, rssClient);

                //Add items to environment group
                string altImagePath = "Assets/environment-item.png";
                AddItemsToGroup(environmentGroup, environmentNewsFeeds, altImagePath);
            }
        }

        //Get all education news feeds
        private async void GetEducationNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed educationNewsFeeds = await DataPersister.GetFeeds(educationNewsUrlString, rssClient);

                //Add items to education group
                string altImagePath = "Assets/education-item.jpg";
                AddItemsToGroup(educationGroup, educationNewsFeeds, altImagePath);
            }
        }

        //Get all culture news feeds
        private async void GetCultureNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed cultureNewsFeeds = await DataPersister.GetFeeds(cultureNewsUrlString, rssClient);

                //Add items to culture group
                string altImagePath = "Assets/culture-item.jpg";
                AddItemsToGroup(cultureGroup, cultureNewsFeeds, altImagePath);
            }
        }

        //Get all archaeology news feeds
        private async void GetArchaeologyNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed archaeologyNewsFeeds = await DataPersister.GetFeeds(archaeologyNewsUrlString, rssClient);

                //Add items to archaeology group
                string altImagePath = "Assets/archaeology-item.jpg";
                AddItemsToGroup(archaeologyGroup, archaeologyNewsFeeds, altImagePath);
            }
        }

        //Get all health news feeds 
        private async void GetHealthNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed healthNewsFeeds = await DataPersister.GetFeeds(healthNewsUrlString, rssClient);

                //Add items to health group
                string altImagePath = "Assets/health-item.jpg";
                AddItemsToGroup(healthGroup, healthNewsFeeds, altImagePath);
            }
        }

        //Get all obituaries news feeds
        private async void GetObituariesNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed obituariesNewsFeeds = await DataPersister.GetFeeds(obituariesNewsUrlString, rssClient);

                //Add items to obituaries group
                string altImagePath = "Assets/obituaries-item.jpg";
                AddItemsToGroup(obituariesGroup, obituariesNewsFeeds, altImagePath);
            }
        }

        //Get all sports news feeds
        private async void GetSportsNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed sportsNewsFeeds = await DataPersister.GetFeeds(sportsNewsUrlString, rssClient);

                //Add items to sports group
                string altImagePath = "Assets/sports-item.jpg";
                AddItemsToGroup(sportsGroup, sportsNewsFeeds, altImagePath);
            }
        }

        //Get all crime news feeds 
        private async void GetCrimeNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed crimeNewsFeeds = await DataPersister.GetFeeds(crimeNewsUrlString, rssClient);

                //Add items to crime group
                string altImagePath = "Assets/crime-item.jpg";
                AddItemsToGroup(crimeGroup, crimeNewsFeeds, altImagePath);
            }
        }

        //Get all lifestyle news feeds
        private async void GetLifestyleNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed lifestyleNewsFeeds = await DataPersister.GetFeeds(lifestyleNewsUrlString, rssClient);

                //Add items to lifestyle group
                string altImagePath = "Assets/lifestyle-item.jpg";
                AddItemsToGroup(lifestyleGroup, lifestyleNewsFeeds, altImagePath);
            }
        }

        //Get all views news feeds
        private async void GetViewsNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed viewsNewsFeeds = await DataPersister.GetFeeds(viewsNewsUrlString, rssClient);

                //Add items to views group
                string altImagePath = "Assets/views on bg-item.jpg";
                AddItemsToGroup(viewsGroup, viewsNewsFeeds, altImagePath);
            }
        }

        //Get all world news feeds
        private async void GetWorldNewsRssFeeds()
        {
            if (IsConnectedToInternet())
            {
                RSSFeed worldNewsFeeds = await DataPersister.GetFeeds(worldNewsUrlString, rssClient);

                //Add items to world group
                string altImagePath = "Assets/world.jpg";
                AddItemsToGroup(worldGroup, worldNewsFeeds, altImagePath);
            }
        }
        #endregion

        #region Auxiliary methods
        private void AddItemsToGroup(NewsGroupViewModel group, RSSFeed feeds, string altImagePath)
        {
            foreach (var item in feeds.Items)
            {
                string imageSrc = GetImageSource(item);

                if (string.IsNullOrEmpty(imageSrc))
                {
                    imageSrc = altImagePath;
                }

                string itemDescription = GetItemDescription(item.Description);

                group.Items.Add(new NewsItemViewModel(item.Title, item.Title, item.PublishedDateTime,
                    imageSrc, itemDescription, itemDescription, group));
            }
        }

        private string GetItemDescription(string description)
        {
            Regex regex = new Regex("(<.*?>\\s*)+", RegexOptions.Singleline);

            string resultText = regex.Replace(description, " ").Trim();
            resultText = Regex.Replace(resultText, "S&amp;D", " ");
            resultText = Regex.Replace(resultText, "&amp;", " ");
            resultText = Regex.Replace(resultText, "&quot;", " ");
            resultText = Regex.Replace(resultText, "&#160;", " ");

            return resultText;
        }

        private static string GetImageSource(RSSItem item)
        {
            string description = item.Description;
            string imageSrc = Regex.Match(description, "<img.+?src=[\"'](.+?)[\"'].+?>", RegexOptions.IgnoreCase).Groups[1].Value;
            return imageSrc;
        }

        private static bool IsConnectedToInternet()
        {
            ConnectionProfile connectionProfile = NetworkInformation.GetInternetConnectionProfile();
            bool isConnected = (connectionProfile != null && connectionProfile.GetNetworkConnectivityLevel()
                == NetworkConnectivityLevel.InternetAccess);
            return isConnected;
        }

        private static async Task LoadDataFromLocalFolder(IEnumerable<NewsGroupViewModel> groups)
        {
            foreach (var group in groups)
            {
                var groupItems = await DataSaverAndLoader.ReadText(group.UniqueId);
 
                group.Items.Clear();

                foreach (var groupItem in groupItems)
                {
                    if (!String.IsNullOrEmpty(groupItem))
                    {
                        string[] currentItem = Regex.Split(groupItem, "\r\n");
                        string groupItemTitle = currentItem[0];
                        DateTime groupItemPublishedOn = DateTime.Parse(currentItem[1]);
                        string groupItemUniqueId = groupItemTitle;
                        string groupItemDescription = currentItem[2];

                        NewsItemViewModel groupItemViewModel = new NewsItemViewModel(groupItemUniqueId, groupItemTitle, groupItemPublishedOn,
                            string.Empty, groupItemDescription, groupItemDescription, group);

                        group.Items.Add(groupItemViewModel);
                    }
                }

                var groupImages = await DataSaverAndLoader.ReadImageData(group.UniqueId + "-images");

                if (groupImages.Count > 0 && groupItems.Count > 0 && groupItems.Count == groupImages.Count)
                {
                    for (int i = 0; i < groupItems.Count; i++)
                    {
                        group.Items[i].Image = groupImages[i];
                    }
                }
            }
        }
        #endregion
    }
}