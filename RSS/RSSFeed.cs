using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS
{
    /// <summary>
    /// Rss feed class
    /// </summary>
    public class RSSFeed
    {
        private ObservableCollection<RSSItem> items = new ObservableCollection<RSSItem>();

        public ObservableCollection<RSSItem> Items
        {
            get
            {
                return this.items;
            }
        }
    }
}
