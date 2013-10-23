using BGNewsService.Data;
using System;

namespace BGNewsService.DataModel
{
    /// <summary>
    /// Generic news item data model.
    /// </summary>
    public class NewsItemViewModel : ViewModelBase
    {
        public NewsItemViewModel(String uniqueId, String title, DateTime publishedDateTime, String imagePath, 
            String description, String content, NewsGroupViewModel group)
            : base(uniqueId, title, publishedDateTime, imagePath, description)
        {
            this.content = content;
            this.group = group;
        }

        private string content = string.Empty;
        public string Content
        {
            get { return this.content; }
            set { this.SetProperty(ref this.content, value); }
        }

        private NewsGroupViewModel group;
        public NewsGroupViewModel Group
        {
            get { return this.group; }
            set { this.SetProperty(ref this.group, value); }
        }
    }
}