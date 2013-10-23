using RSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Background;
using Windows.Data.Xml.Dom;
using Windows.UI.Notifications;

namespace BackgroundTasks
{
    /// <summary>
    /// Class for creating a tile notification.
    /// </summary>
    public sealed class NewsFeedBackgroundTask : IBackgroundTask
    {
        static string domesticNewsUrlString = "http://www.novinite.com/services/news_rdf.php?category_id=23#sthash.1R5Kjvhg.dpuf";
        static string textElementName = "text";

        public async void Run(IBackgroundTaskInstance taskInstance)
        {
            BackgroundTaskDeferral deferral = taskInstance.GetDeferral();

            var rssClient = new RSSClient();
            var feedsFromDomesticNewsGroup = await rssClient.GetFeeds(domesticNewsUrlString);

            UpdateTile(feedsFromDomesticNewsGroup);

            deferral.Complete();
        }

        private static void UpdateTile(RSSFeed feedsFromDomesticNewsGroup)
        {
            var updater = TileUpdateManager.CreateTileUpdaterForApplication();
            updater.EnableNotificationQueue(true);
            updater.Clear();

            int itemCount = 0;

            // Create a tile notification for each feed item.
            foreach (var item in feedsFromDomesticNewsGroup.Items)
            {
                XmlDocument tileXml = TileUpdateManager.GetTemplateContent(TileTemplateType.TileWideText03);

                var title = item.Title;
                string titleText = title == null ? String.Empty : title;
                tileXml.GetElementsByTagName(textElementName)[0].InnerText = titleText;

                updater.Update(new TileNotification(tileXml));

                if (itemCount++ > 5)
                {
                    break;
                }
            }
        }
    }
}