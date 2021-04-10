using System.IO;

namespace ComputerVisionLib
{
    public class ComputerVision
    {
        public VisorResults Results { get; set; }

        public ComputerVision(MemoryStream imageStream, KeysLib keys)
        {
            Results = new VisorResults()
            {
                VisorAmazon = new VisorAmazon(imageStream, keys.AmazonKey, keys.AmazonSecretKey),
                VisorAzure = new VisorAzure(imageStream, keys.AzureKey, keys.AzureEndpoint),
                VisorGoogle = new VisorGoogle(imageStream, keys.GoogleCredentials),
            };
        }

    }
}
