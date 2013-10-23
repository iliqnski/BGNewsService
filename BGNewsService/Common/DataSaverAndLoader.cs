using BGNewsService.DataModel;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;

namespace BGNewsService.Common
{
    /// <summary>
    /// Class for saving and loading data from local folder.
    /// </summary>
    public class DataSaverAndLoader
    {
        /// <summary>
        /// Save data to files.
        /// </summary>
        /// <param name="groups">The groups data to be saved.</param>
        /// <returns>An asynchronous task.</returns>
        public static async Task SaveDataToFiles(IEnumerable<NewsGroupViewModel> groups)
        {
            HttpClient client = new HttpClient();
            int fileNameCounter = 0;

            foreach (var group in groups)
            {
                var folderForTextFiles = await ApplicationData.Current.LocalFolder.CreateFolderAsync(group.UniqueId,
                    CreationCollisionOption.ReplaceExisting);
                var folderForImageFiles = await ApplicationData.Current.LocalFolder.CreateFolderAsync(group.UniqueId + "-images",
                    CreationCollisionOption.ReplaceExisting);

                foreach (var newsArticle in group.Items)
                {
                    await WriteText(fileNameCounter, folderForTextFiles, newsArticle);

                    var imageFile = await folderForImageFiles.CreateFileAsync(fileNameCounter.ToString(),
                     CreationCollisionOption.ReplaceExisting);

                    if (IsValidUri(newsArticle.imagePath))
                    {
                        await WriteImageFromUri(client, newsArticle, imageFile);
                    }
                    else
                    {
                        await WriteImageFromPath(group, imageFile);
                    }
                    fileNameCounter++;
                }
                fileNameCounter = 0;
            }
        }

        /// <summary>
        /// Writes the text data of each group in file.
        /// </summary>
        /// <param name="fileNameCounter">Used for the names of the files.</param>
        /// <param name="folder">Folder that contains the all files for the specific group.</param>
        /// <param name="newsItem">A news item.</param>
        /// <returns>An asynchronous task.</returns>
        private static async Task WriteText(int fileNameCounter, StorageFolder folder, NewsItemViewModel newsItem)
        {
            var file = await folder.CreateFileAsync(fileNameCounter.ToString(),
                CreationCollisionOption.ReplaceExisting);

            await Windows.Storage.FileIO.WriteTextAsync(file, newsItem.Title + "\r\n" + newsItem.PublishedOn + "\r\n" +
             newsItem.Description);
        }

        /// <summary>
        /// Writes data from image path in file, in case the news item does not have image url.
        /// </summary>
        /// <param name="group">The group that contains the specified image.</param>
        /// <param name="imageFile">The file to store the image data.</param>
        /// <returns>An asynchronous task.</returns>
        private static async Task WriteImageFromPath(NewsGroupViewModel group, StorageFile imageFile)
        {
            var packageLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var assetsFolder = await packageLocation.GetFolderAsync("Assets");
            StorageFile myImage = await assetsFolder.GetFileAsync(group.Title.ToLower() + "-item.jpg");

            byte[] bytes = await GetBytesFromFile(myImage);

            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (IOutputStream outputStream = fileStream.GetOutputStreamAt(0))
                {
                    using (DataWriter dataWriter = new DataWriter(outputStream))
                    {
                        dataWriter.WriteBytes(bytes);
                        await dataWriter.StoreAsync();
                        dataWriter.DetachStream();
                    }

                    await outputStream.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Writes data from Url in file.
        /// </summary>
        /// <param name="client">Http client.</param>
        /// <param name="item">The news item which contains the specific image url</param>
        /// <param name="imageFile">The which stores the image data.</param>
        /// <returns>An asynchronous task.</returns>
        private static async Task WriteImageFromUri(HttpClient client, NewsItemViewModel newsItem, StorageFile imageFile)
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, newsItem.imagePath);
            HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);

            using (IRandomAccessStream fileStream = await imageFile.OpenAsync(FileAccessMode.ReadWrite))
            {
                using (IOutputStream outputStream = fileStream.GetOutputStreamAt(0))
                {
                    using (DataWriter dataWriter = new DataWriter(outputStream))
                    {
                        dataWriter.WriteBytes(await response.Content.ReadAsByteArrayAsync());
                        await dataWriter.StoreAsync();
                        dataWriter.DetachStream();
                    }

                    await outputStream.FlushAsync();
                }
            }
        }

        /// <summary>
        /// Gets image information from storage file.
        /// </summary>
        /// <param name="storageFile">The storage file which </param>
        /// <returns>Returns the information as byte array.</returns>
        private static async Task<byte[]> GetBytesFromFile(StorageFile storageFile)
        {
            var stream = await storageFile.OpenReadAsync();

            using (var dataReader = new DataReader(stream))
            {
                var bytes = new byte[stream.Size];
                await dataReader.LoadAsync((uint)stream.Size);
                dataReader.ReadBytes(bytes);

                return bytes;
            }
        }

        /// <summary>
        /// Checks if an url is valid
        /// </summary>
        /// <param name="uriAsString">The url to be checked.</param>
        /// <returns>Returns true if the url is valid or false otherwise.</returns>
        private static bool IsValidUri(string uriAsString)
        {
            Uri uri = null;

            if (!Uri.TryCreate(uriAsString, UriKind.Absolute, out uri) || uri == null)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Read text from files in the specific folder.
        /// </summary>
        /// <param name="folderName">The name of the folder which will be used.</param>
        /// <returns>Returns the content of the files.</returns>
        public static async Task<List<string>> ReadText(string folderName)
        {
            List<string> filesContent = new List<string>();
       
            try
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var folder = await localFolder.GetFolderAsync(folderName);
                var files = await folder.GetFilesAsync();

                for (int i = 0; i < files.Count; i++)
                {
                    var text = await FileIO.ReadTextAsync(files[i]);
                    filesContent.Add(text);
                }
            }
            catch (Exception ex)
            {
                //The folder or file are not saved correctly
                return filesContent;
            }

            return filesContent;
        }

        /// <summary>
        /// Read image data from files.
        /// </summary>
        /// <param name="folderName">The name of the folder which will be used.</param>
        /// <returns>Returns a collection of bitmap images.</returns>
        public static async Task<List<BitmapImage>> ReadImageData(string folderName)
        {
            List<BitmapImage> images = new List<BitmapImage>();

            try
            {
                var localFolder = Windows.Storage.ApplicationData.Current.LocalFolder;
                var folder = await localFolder.GetFolderAsync(folderName);
                var files = await folder.GetFilesAsync();

                foreach (var file in files)
                {
                    IBuffer buffer = await FileIO.ReadBufferAsync(file);
                    byte[] byteArray = buffer.ToArray();

                    using (InMemoryRandomAccessStream raStream = new InMemoryRandomAccessStream())
                    {
                        using (DataWriter writer = new DataWriter(raStream))
                        {
                            writer.WriteBytes(byteArray);
                            await writer.StoreAsync();
                            await writer.FlushAsync();
                            writer.DetachStream();
                        }

                        raStream.Seek(0);

                        BitmapImage bitMapImage = new BitmapImage();
                        bitMapImage.SetSource(raStream);

                        images.Add(bitMapImage);
                    }
                }
            }
            catch (Exception ex)
            {
                //The folder or file are not saved correctly
                return images;
            }

            return images;
        }
    }
}