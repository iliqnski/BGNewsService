using RSS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGNewsService.DataPersisterModel
{
    /// <summary>
    /// Persists data from rss feeds
    /// </summary>
    public static class DataPersister
    {
        public static async Task<RSSFeed> GetFeeds(string url, RSSClient client)
        {
            RSSFeed feeds = await client.GetFeeds(url);

            return feeds;
        }
    }
}