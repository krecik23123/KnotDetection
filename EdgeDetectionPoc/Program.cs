using System.Collections.Generic;
using System.Drawing;
using Imaging.Library;
using Imaging.Library.Entities;
using Imaging.Library.Filters.ComplexFilters;
using Imaging.Library.Enums;
using Accord.Statistics;

namespace EdgeDetectionPoc
{
    class Program
    {
        static void Main(string[] args)
        {
            var path = @"c:\seki\Sek2.png";
            var savepath = @"c:\seki\Canny2.png";

            Image image1 = Image.FromFile(path);

            Bitmap img = new Bitmap("c:\\seki\\Sek2.png");
            var pixelMap = new PixelMap(img.Width, img.Height);

            List<double> imageAvgPixels = new List<double>(); 
            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var pixel1 = img.GetPixel(i, j);
                    imageAvgPixels.Add(0.2126 * pixel1.R + 0.7152 * pixel1.G + 0.0722 * pixel1.B);
                }
            }

            List<double> exp = new List<double>();
            for (int i = 0; i < 100; i++)
            {
                exp.Add(100);
            }

            var test = exp.ToArray().Entropy();

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var pixel1 = img.GetPixel(i, j);
                    pixelMap.Map[i][j] = new Pixel(pixel1.R, pixel1.G, pixel1.B);
                }
            }

            var imaging = new ImagingManager(pixelMap);
            imaging.Render();
            imaging.AddFilter(new CannyEdgeDetector());
            imaging.Render();

            var blobCounter = new BlobCounter
            {
                ObjectsOrder = ObjectsOrder.Size
            };
            imaging.AddFilter(blobCounter);

            imaging.Render();

            var output = imaging.Output;

            for (int i = 0; i < img.Width; i++)
            {
                for (int j = 0; j < img.Height; j++)
                {
                    var pixel1 = output.Map[i][j];
                    var color = Color.FromArgb(pixel1.R, pixel1.G, pixel1.B);
                    img.SetPixel(i, j, color);
                }
            }

            img.Save(savepath);

        }
    }
}
