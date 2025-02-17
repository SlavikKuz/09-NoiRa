﻿using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using ComputerVisionLib;
using ImageProviderLib;
using NAudio.Wave;
using PlayerLib;
using SemanticProcessorLib;
using PdfCreatorLib;
using SemanticProcessorLib.Models;
using WebAPI.Models;
using ComputerHearingLib;

namespace WebAPI.Controllers
{
    public class HomeController : Controller
    {
        private readonly string _tempImagePath = Environment.CurrentDirectory + @"\wwwroot\source\temp.png";
        private readonly string _tempPdfPath = Environment.CurrentDirectory + @"\wwwroot\source\temp.pdf";

        public IActionResult Index()
        {
            var imageProvider = new ImageProvider(_tempImagePath);
            var computerVision = new ComputerVision(imageProvider.ImageStream, new KeysLib());
            var semanticProcessor = new SemanticProcessor(computerVision.Results);
            var semanticResult = semanticProcessor.GetResult();
            new PdfCreator(semanticResult, _tempImagePath, _tempPdfPath);
            var soundFinder = new SoundFinder(semanticResult); //add true

            var kit = new DrumKit(soundFinder.BacksSoundLinks.Select(w=>w.Key).ToList(),
                soundFinder.EventSoundLinks.Select(w => w.Key).ToList());
            
            var pattern = new DrumPattern(kit.SoundScapes);
            var patternSequencer = new DrumPatternSampleProvider(pattern, kit) { Tempo = 100 };
            var waveOut = new WaveOut();
            waveOut.Init(patternSequencer);
            waveOut.Play();

            var model = new IndexViewModel()
            {
                Image = imageProvider.ImageStream.ToArray(),
                ImageCaptions = semanticResult.ImageCaption,
                ImageDescription = semanticResult.Words.Select(w => w.Word).ToList(),
                BackSounds = soundFinder.BacksSoundLinks.Select(w => w.Key).ToList(),
                Color = semanticResult.Color,
                LinksToPlay = kit.LinksToPlay
            };

            model.BackSounds.AddRange(soundFinder.EventSoundLinks.Select(w => w.Key).ToList());

            return View(model);
        }
    }
}
