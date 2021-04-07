using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using ComputerVisionLib;
using ImageProviderLib;
using NAudio.Wave;
using PlayerLib;
using SemanticProcessorLib;
using SoundFinderLib;

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
