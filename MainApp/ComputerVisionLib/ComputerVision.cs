using System.IO;

namespace ComputerVisionLib
{
    public class ComputerVision
    {
        public VisorAmazon VisorAmazon { get; set; }
        public VisorAzure VisorAzure { get; set; }
        public VisorGoogle VisorGoogle { get; set; }

        public ComputerVision(MemoryStream imageStream, KeysLib keys)
        {
            VisorAmazon = new VisorAmazon(imageStream, keys.AmazonKey, keys.AmazonSecretKey);
            VisorAzure = new VisorAzure(imageStream, keys.AzureKey, keys.AzureEndpoint);
            VisorGoogle = new VisorGoogle(imageStream, keys.GoogleCredentials);
        }

    }
}
