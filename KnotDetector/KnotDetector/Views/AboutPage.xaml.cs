using KnotDetectorML.Model;
using Microsoft.ML;
using System;
using System.ComponentModel;
using System.IO;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace KnotDetector.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();

            CameraButton.Clicked += CameraButton_Clicked;
        }

        private async void CameraButton_Clicked(object sender, EventArgs e)
        {
            var photo = await Plugin.Media.CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions() { });

            if (photo != null)
            {
                byte[] imageAsBytes = null;
                using (var memoryStream = new MemoryStream())
                {
                    photo.GetStream().CopyTo(memoryStream);
                    photo.Dispose();
                    imageAsBytes = memoryStream.ToArray();
                }

                var photoStream = ImageSource.FromStream(() => { return photo.GetStream(); });

                MLContext mlContext = new MLContext();

                // Load Trained Model
                //DataViewSchema predictionPipelineSchema;
                //ITransformer predictionPipeline = mlContext.Model.Load("model.zip", out predictionPipelineSchema);
                var result = ConsumeModel.Predict(new ModelInput() { ImageSource = photoStream.ToString() });
                PhotoImage.Source = photoStream;
            }
        }
    }
}