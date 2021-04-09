using System.Collections.Generic;
using Google.Cloud.Vision.V1;
using System.IO;
using Newtonsoft.Json;

namespace ComputerVisionLib
{
    public class VisorGoogle
    {
        public Dictionary<string, double> JsonToDictionary { get; set; }

        public VisorGoogle(MemoryStream imageStream, string credentialsJson)
        {
            var client = new ImageAnnotatorClientBuilder
            {
                JsonCredentials = credentialsJson
            }.Build();

            var image = Image.FromStream(imageStream);

            var response = client.DetectLabels(image);

            var dictionary = new Dictionary<string, double>();

            foreach (var label in response)
            {
                dictionary.Add(label.Description, label.Score);
            }

            JsonToDictionary = dictionary;
        }
    }
}
