using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ComputerVisionLib;
using ImageProviderLib;
using NAudio.Wave;
using Newtonsoft.Json;
using PlayerLib;
using SemanticProcessorLib;
using SoundFinderLib;
using WebAPI.Models;

namespace WebAPI.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var imageProvider = new ImageProvider();
            var computerVision = new ComputerVision(imageProvider.ImageToStream(), new KeysLib());

            var resultAmazon = computerVision.VisorAmazon.JSON;
            var resultAzure = computerVision.VisorAzure.JSON;
            var resultGoogle = computerVision.VisorGoogle.JSON;

            var semanticResults = SemanticProcessor.ProcessResults(resultAmazon, resultAzure, resultGoogle);

            //client to pdf;









            var soundFinder = new SoundFinder(semanticResults.WordsOfDescription, false);

            var kit = new DrumKit(soundFinder.SoundLinksBacks, soundFinder.SoundLinksFX);
            var pattern = new DrumPattern(kit.SoundScapes);
            var waveOut = new WaveOut();
            var patternSequencer = new DrumPatternSampleProvider(pattern, kit) { Tempo = 100 };
            waveOut.Init(patternSequencer);
            waveOut.Play();

            var model = new IndexViewModel()
            {
                Image = imageProvider.ImageToStream().ToArray(),
                ImageDescription = new List<string>(),
                BackSounds = soundFinder.SoundLinksBacks,
                Color = semanticResults.Color,
                LinksToPlay = kit.LinksToPlay
            };

            model.BackSounds.AddRange(soundFinder.SoundLinksFX);

            return View(model);
        }

        //public async Task<PdfCreatorResponse> ProcessToPdf(PdfCreatorRequest request, CancellationToken cancellationToken)
        //{
        //    var urlBuilder = new StringBuilder();
        //    urlBuilder.Append(@"localhost").Append("/PdfCreator/");

        //    var client = new HttpClient();

        //    try
        //    {
        //        using (var httpRequest = new HttpRequestMessage())
        //        {
        //            var content = new StringContent(JsonConvert.SerializeObject(request));
        //            content.Headers.ContentType = MediaTypeHeaderValue.Parse("application/json");

        //            httpRequest.Content = content;
        //            httpRequest.Method = new HttpMethod("PUT");
        //            httpRequest.Headers.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));

        //            var url = urlBuilder.ToString();
        //            httpRequest.RequestUri = new Uri(url, UriKind.RelativeOrAbsolute);

        //            var response = await client
        //                .SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
        //                .ConfigureAwait(false);

        //            if ((int) response.StatusCode != 200)
        //                return new PdfCreatorResponse();

        //            var responseText = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
        //            var type = JsonConvert.DeserializeObject<T>(responseText, JsonSErializerSettings);
        //            return new ObjectRespo
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //    }
        //    finally
        //    {
        //        client.Dispose();
        //    }
        //}

        public class IndexViewModel
        {
            public byte[] Image { get; set; }
            public List<string> ImageDescription { get; set; }
            public List<string> BackSounds { get; set; }
            public List<string> LinksToPlay { get; set; }
            public string Color { get; set; }
        }

    }
}
