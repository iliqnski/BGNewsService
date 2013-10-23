using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Web.Syndication;

namespace RSS
{
    /// <summary>
    /// Rss client class
    /// </summary>
    public class RSSClient
    {
        static string customHeaderName = "User-Agent";
        static string customHeaderValue = "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)";

        public async Task<RSSFeed> GetFeeds(string url)
        {
            RSSFeed feeds = new RSSFeed();
            SyndicationClient client = new SyndicationClient();
            client.BypassCacheOnRetrieve = true;
            client.SetRequestHeader(customHeaderName, customHeaderValue);
            Uri feedUri = new Uri(url);

            try
            {
                SyndicationFeed feed = await client.RetrieveFeedAsync(feedUri);
                var sortedFeeds = feed.Items.OrderByDescending(x =>
                    x.PublishedDate).ToList();

                foreach (var item in sortedFeeds)
                {
                    RSSItem feedItem = new RSSItem();
                    feedItem.Title = item.Title.Text;
                    feedItem.PublishedDateTime = item.PublishedDate.DateTime;
                    feedItem.PublishedDateTime -= new TimeSpan(1, 0, 0);
                    feedItem.Description = item.Summary.Text;
                    feeds.Items.Add(feedItem);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return feeds;
        }
    }
}