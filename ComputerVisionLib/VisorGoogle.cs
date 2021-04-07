using Google.Cloud.Vision.V1;
using System.IO;
using Newtonsoft.Json;

namespace ComputerVisionLib
{
    public class VisorGoogle
    {
        public string JSON { get; set; }

        public VisorGoogle(MemoryStream imageStream, string credentialsJson)
        {
            var client = new ImageAnnotatorClientBuilder
            {
                JsonCredentials = credentialsJson
            }.Build();

            var image = Image.FromStream(imageStream);

            var response = client.DetectLabels(image);

            JSON = JsonConvert.SerializeObject(response, Formatting.Indented);
        }
    }
}
