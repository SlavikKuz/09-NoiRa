using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.RegularExpressions;
using NAudio.Wave;
using Newtonsoft.Json;

namespace ComputerHearingLib
{
    public class ComputerHearing
    {
        private readonly string _filesPath = Environment.CurrentDirectory + @"\wwwroot\source\10sec\";
        private readonly string dbPath = Environment.CurrentDirectory + @"\wwwroot\source\10sec.txt";
        private readonly string dbPathNoDescription = Environment.CurrentDirectory + @"\wwwroot\source\10secNoDescripion.txt";
        private readonly string dbPathCleared = Environment.CurrentDirectory + @"\wwwroot\source\10secClear.txt";

        private const string URI =
            @"https://max-audio-classifier.codait-prod-41208c73af8fca213512856c7a09db52-0000.us-east.containers.appdomain.cloud/model/predict";

        private List<string> list;

        public ComputerHearing()
        {
            GetClearedList();
        }

        private void GetClearedList()
        {
            var done = File.ReadAllLinesAsync(dbPath).Result.ToList();
            var doneLinks = new List<string>();

            foreach (var d in done)
            {
                var lin = d.Split("| ");

                if (lin[0] == "  ")
                    continue;

                doneLinks.Add(d);
            }
            File.AppendAllLines(dbPathCleared, doneLinks);
        }

        private void GetNoDescription()
        {
            var done = File.ReadAllLinesAsync(dbPath).Result.ToList();
            var doneLinks = new List<string>();

            foreach (var d in done)
            {
                var lin = d.Split("| ");
    
                if(lin[0] == "  ")
                    doneLinks.Add(lin[1]);
            }
            File.AppendAllLines(dbPathNoDescription, doneLinks);
        }

        private void MoveNoDescription()
        {
            var done = File.ReadAllLinesAsync(dbPathNoDescription).Result.ToList();

            foreach (var file in done)
            {
                var mp3FileNameRestored = Path.ChangeExtension(Path.GetFileName(file), "mp3");
                var filePath = Directory.GetFiles(Environment.CurrentDirectory + @"\wwwroot\source\", mp3FileNameRestored,
                    SearchOption.AllDirectories).First();

                File.Move(filePath, Environment.CurrentDirectory + @"\wwwroot\source\10secBad\"+ Path.GetFileName(filePath));
            }
        }

        public void Hear()
        {
            list = Directory.GetFiles(_filesPath, " *.wav", SearchOption.AllDirectories).ToList();

            var done = File.ReadAllLinesAsync(dbPath).Result.ToList();
            var doneLinks = new List<string>();

            foreach (var d in done)
            {
                var lin = d.Split(" | ");
                doneLinks.Add(lin[1]);
            }

            foreach (var link in list)
            {
                if (doneLinks.Contains(link))
                    continue;

                try
                {
                    var description = Describe(link);
                    File.AppendAllLines(dbPath, new[] { " " + description + " | " + link });
                }
                catch (Exception ex)
                {
                    continue;
                }
            }
        }

        public string Describe(string fileLink)
        {
            var stream = File.OpenRead(fileLink);

            var fileContent = new StreamContent(stream)
            {
                Headers =
                {
                    ContentLength = stream.Length,
                    ContentType = new MediaTypeHeaderValue("audio/wav")
                }
            };

            HttpResponseMessage response;
            var formDataContent = new MultipartFormDataContent();
            formDataContent.Add(fileContent, "audio", "audio.wav"); // file

            using (var client = new HttpClient())
            {
                response = client.PostAsync(URI, formDataContent).Result;
            }

            var ins = JsonConvert.DeserializeObject<HearingModel>(response.Content.ReadAsStringAsync().Result);
            
            var description = String.Empty;

            if (ins.predictions != null)
                foreach (var i in ins.predictions)
                {
                    description = description + i.label + ":" + i.probability + ", ";
                }

            return description;
        }
    }
}
