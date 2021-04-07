using System.IO;
using Newtonsoft.Json;

namespace ComputerVisionLib
{
    public class KeysLib
    {
        public string AzureKey { get; set; }
        public string AzureEndpoint { get; set; }
        public string AmazonKey { get; set; }
        public string AmazonSecretKey { get; set; }
        public string GoogleCredentials { get; set; }

        private readonly string _filePath = @"D:\Repos\09 - BackNoRa\WebAPI\wwwroot\source\Keys.txt";

        public KeysLib()
        {
            var keys = ReadFromJsonFile<Keys>();
            AzureKey = keys.AzureKey;
            AzureEndpoint = keys.AzureEndpoint;
            AmazonKey = keys.AmazonKey;
            AmazonSecretKey = keys.AmazonSecretKey;
            GoogleCredentials = keys.GoogleCredentials;
        }

        private class Keys
        {
            public string AzureKey { get; set; }
            public string AzureEndpoint { get;set;}
            public string AmazonKey { get; set; }
            public string AmazonSecretKey { get; set; }
            public string GoogleCredentials { get; set; }
        }

        private T ReadFromJsonFile<T>() where T : new()
        {
            TextReader reader = null;
            try
            {
                reader = new StreamReader(_filePath);
                var fileContents = reader.ReadToEnd();
                return JsonConvert.DeserializeObject<T>(fileContents);
            }
            finally
            {
                reader?.Close();
            }
        }

        private void WriteToJsonFile<T>(T objectToWrite, bool append = false) where T : new()
        {
            TextWriter writer = null;
            try
            {
                var contentsToWriteToFile = JsonConvert.SerializeObject(objectToWrite);
                writer = new StreamWriter(_filePath, append);
                writer.Write(contentsToWriteToFile);
            }
            finally
            {
                writer?.Close();
            }
        }
    }
}
