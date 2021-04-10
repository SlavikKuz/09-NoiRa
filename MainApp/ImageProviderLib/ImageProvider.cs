using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net;

namespace ImageProviderLib
{
    public class ImageProvider
    {
        public string SampleImageUrl { get; set; }= @"https://picsum.photos/480/640";
        public Image Image { get; set; }
        public MemoryStream ImageStream { get; set; }

        public ImageProvider (string tempImagePath)
        {
            Image = DownloadImage(SampleImageUrl);
            ImageStream = ImageToStream();
            File.WriteAllBytes(tempImagePath, ImageStream.ToArray());
        }

        private MemoryStream ImageToStream()
        {
            var imageStream = new MemoryStream();
            Image.Save(imageStream, ImageFormat.Png);
            imageStream.Position = 0;
            return imageStream;
        }

        private static Image DownloadImage(string fromUrl)
        {
            using var webClient = new WebClient();
            using var stream = webClient.OpenRead(fromUrl);
            return Image.FromStream(stream);
        }
    }
}
