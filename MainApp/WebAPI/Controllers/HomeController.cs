using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ComputerVisionLib;
using ImageProviderLib;
using NAudio.Wave;
using PlayerLib;
using SemanticProcessorLib;
using PdfCreatorLib;
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
            var resultGoogle = computerVision.VisorGoogle.JsonToDictionary;

            var semanticProcessor = new SemanticProcessor(resultAmazon, resultAzure, resultGoogle);

            var imagePath = Environment.CurrentDirectory + @"\wwwroot\source\temp.jpg";
            System.IO.File.WriteAllBytes(imagePath, imageProvider.ImageToStream().ToArray());

            var semanticResult = semanticProcessor.GetResult();
            var pdf = new PdfCreator(semanticResult, imagePath, Environment.CurrentDirectory + @"\wwwroot\source\noiset.pdf");

            var soundFinder = new SoundFinder(semanticResult.Words.Select(w=>w.Word).ToList(), true);

            var kit = new DrumKit(soundFinder.SoundLinksBacks, soundFinder.SoundLinksFX);
            var pattern = new DrumPattern(kit.SoundScapes);
            var waveOut = new WaveOut();
            var patternSequencer = new DrumPatternSampleProvider(pattern, kit) {Tempo = 100};
            waveOut.Init(patternSequencer);
            waveOut.Play();

            var model = new IndexViewModel()
            {
                Image = imageProvider.ImageToStream().ToArray(),
                ImageCaptions = semanticResult.ImageCaption,
                ImageDescription = semanticResult.Words.Select(w => w.Word).ToList(),
                BackSounds = soundFinder.SoundLinksBacks,
                Color = semanticResult.Color,
                LinksToPlay = kit.LinksToPlay
            };

            model.BackSounds.AddRange(soundFinder.SoundLinksFX);

            return View(model);
        }
    }
}
