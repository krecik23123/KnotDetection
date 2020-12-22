using Azure.Storage;
using Azure.Storage.Blobs;
using KnotDetectionLogic;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace KnotDetector.Views
{
    public partial class AboutPage : ContentPage
    {
        private const string Container = "knots";
        private const string AccountName = "klstoragestandard";

        public AboutPage()
        {
            InitializeComponent();

            CameraButton.Clicked += CameraButton_Clicked;
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            var fileName = "sek123.png";
            var fileUri = GetBlobUri(fileName);
            DeleteBlob(fileUri);

            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
            {
                byte[] imageAsBytes = null;
                string stream = null;
                using (var memoryStream = new MemoryStream())
                {
                    photo.GetStream().CopyTo(memoryStream);
                    photo.Dispose();
                    imageAsBytes = memoryStream.ToArray();
                    stream = Encoding.ASCII.GetString(imageAsBytes);
                    await SendAsBlob(memoryStream, fileUri);
                }

                var photoStream = ImageSource.FromStream(() => { return photo.GetStream(); });

                PhotoImage.Source = photoStream;

                var detectorService = new KnotDetectorService();

                var detectResult = await detectorService.DetectKnotAsync(fileName);

                await DisplayAlert("Detecting Result", detectResult, "OK");

                DeleteBlob(fileUri);
            }
        }

        private Uri GetBlobUri(string fileName)
        {
            return new Uri("https://" +
             AccountName +
             ".blob.core.windows.net/" +
             Container +
             "/" + fileName);
        }

        private static async Task SendAsBlob(MemoryStream memoryStream, Uri fileUri)
        {
            memoryStream.Position = 0;

            StorageSharedKeyCredential storageCredentials =
                new StorageSharedKeyCredential(AccountName, "ZDPv+b21qebIzVSaerihweAmrO4RTMhtRhMVb6uIda8z8mGJszn6mj3c8cMBC0ZoWItBedjC9Vm+IETEPxgB+g==");

            BlobClient blobClient = new BlobClient(fileUri, storageCredentials);

            await blobClient.UploadAsync(memoryStream);
        }

        private static void DeleteBlob(Uri fileUri)
        {
            StorageSharedKeyCredential storageCredentials =
                new StorageSharedKeyCredential(AccountName, "ZDPv+b21qebIzVSaerihweAmrO4RTMhtRhMVb6uIda8z8mGJszn6mj3c8cMBC0ZoWItBedjC9Vm+IETEPxgB+g==");

            BlobClient blobClient = new BlobClient(fileUri, storageCredentials);

            blobClient.DeleteIfExists();
        }
    }
}