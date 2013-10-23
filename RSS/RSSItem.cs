using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RSS
{
    /// <summary>
    /// Rss item class
    /// </summary>
    public class RSSItem
    {
        public string Title { get; set; }

        public DateTime PublishedDateTime { get; set; }

        public string Description { get; set; }
    }
}
