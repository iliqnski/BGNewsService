using BGNewsService.Common;
using BGNewsService.DataModel;
using System;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace BGNewsService.Data
{
    /// <summary>
    /// Base class for <see cref="NewsItemViewModel"/> and <see cref="NewsGroupViewModel"/> that
    /// defines properties common to both.
    /// </summary>
    [WebHostHidden]
    public abstract class ViewModelBase : BindableBase
    {
        private static Uri baseUri = new Uri("ms-appx:///");

        public ViewModelBase(String uniqueId, String title, DateTime publishedDateTime, 
            String imagePath, String description)
        {
            this.uniqueId = uniqueId;
            this.title = title;
            this.publishedDateTime = publishedDateTime;
            this.description = description;
            this.imagePath = imagePath;
        }

        private string uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this.uniqueId; }
            set { this.SetProperty(ref this.uniqueId, value); }
        }

        private string title = string.Empty;
        public string Title
        {
            get { return this.title; }
            set { this.SetProperty(ref this.title, value); }
        }

        private DateTime publishedDateTime = DateTime.Now;
        public DateTime PublishedOn
        {
            get { return this.publishedDateTime; }
            set { this.SetProperty(ref this.publishedDateTime, value); }
        }

        private string description = string.Empty;
        public string Description
        {
            get { return this.description; }
            set { this.SetProperty(ref this.description, value); }
        }

        private ImageSource image = null;
        public String imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this.image == null && this.imagePath != null)
                {
                    this.image = new BitmapImage(new Uri(ViewModelBase.baseUri, this.imagePath));
                }
                return this.image;
            }

            set
            {
                this.imagePath = null;
                this.SetProperty(ref this.image, value);
            }
        }

        public void SetImage(String path)
        {
            this.image = null;
            this.imagePath = path;
            this.OnPropertyChanged("Image");
        }

        public override string ToString()
        {
            return this.Title;
        }
    }
}