using Amazon;
using Amazon.Rekognition;
using Amazon.Rekognition.Model;
using Newtonsoft.Json;
using System.IO;

namespace ComputerVisionLib
{
    public class VisorAmazon
    {
        private AmazonRekognitionClient _client;

        private const float ConfidenceLevel = 0.55F;
        public string JSON { get; set; }

        public VisorAmazon(MemoryStream imageStream, string amazonKey, string amazonSecretKey)
        {
            _client = new AmazonRekognitionClient(amazonKey, amazonSecretKey, RegionEndpoint.USEast1);
            DetectLabels(imageStream);
        }

        private void DetectLabels(MemoryStream stream)
        {
            var response = _client.DetectLabelsAsync(new DetectLabelsRequest
            {
                MinConfidence = ConfidenceLevel,
                MaxLabels = 100,
                Image = new Image
                {
                    Bytes = stream
                }
            }).Result;

            JSON = JsonConvert.SerializeObject(response, Formatting.Indented);
        }
    }
}