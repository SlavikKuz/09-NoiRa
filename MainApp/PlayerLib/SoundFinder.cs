using SemanticProcessorLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using SemanticProcessorLib.Models;

namespace PlayerLib
{
    public class SoundFinder
    {
        private static readonly string BacksDirectory = Environment.CurrentDirectory + @"\wwwroot\source\Backs\";
        private static readonly string EventDirectory = Environment.CurrentDirectory + @"\wwwroot\source\Event\";

        private List<string> allBacksFiles { get; set; }
        private List<string> allEventFiles { get; set; }

        public Dictionary<string, int> BacksSoundLinks { get; set; }
        public Dictionary<string, int> EventSoundLinks { get; set; }

        //public List<string> BacksSoundLinks { get; set; }
        //public List<string> EventSoundLinks { get; set; }

        public SoundFinder(SemanticImage results, bool processLib = false)
        {
            if (processLib)
            { 
                ProcessLib(30);
                NormalizeFilenames(BacksDirectory, EventDirectory);
            }

            var words = results.Words.Select(w => w.Word);

            allBacksFiles = Directory.GetFiles(BacksDirectory, "*.mp3", SearchOption.AllDirectories).ToList();
            BacksSoundLinks = FindSoundFiles(words, allBacksFiles);

            allEventFiles = Directory.GetFiles(EventDirectory, "*.mp3", SearchOption.AllDirectories).ToList();
            EventSoundLinks = FindSoundFiles(words, allEventFiles);
        }

        public void ProcessLib(int soundLength)
        {
            var soundLibProc = new SoundLibProcessor();
            soundLibProc.GetBacksMp3Samples(soundLength);
            soundLibProc.GetEventMp3Samples(soundLength);
            soundLibProc.RemoveBadFilesFromOrigin();
        }

        private Dictionary<string, int> FindSoundFiles(IEnumerable<string> imageWords, List<string> allFiles)
        {
            var soundFiles = new List<string>();
            var soundFilesWeghted = new Dictionary<string, int>();

            foreach (var word in imageWords) 
                soundFiles.AddRange(FindSoundFile(word, allFiles));

            var soundWeight = 0;

            foreach (var sound in soundFiles)
            {
                foreach (var word in imageWords)
                {
                    if (soundFiles.Contains(word))
                        soundWeight++;
                }
                soundFilesWeghted.Add(sound,soundWeight);
            }
            return soundFilesWeghted;
        }

        private IEnumerable<string> FindSoundFile(string word, List<string> allFiles)
        {
            var found = new List<string>();

            var wordVariations = new List<string>()
                {word, word.ToUpper(), word[0].ToString().ToUpper() + word.Substring(1)};
            
            foreach (var w in wordVariations)
            {
                found.AddRange(allFiles.Where(f =>
                    f.Replace("_", " ").Replace("-", " ").Contains(" " + w)).ToList());
            }

            return found;
        }

        private void NormalizeFilenames(string backsDirectory, string eventDirectory)
        {
            var list = new List<string>();
            list.AddRange(Directory.GetFiles(BacksDirectory, "*.mp3", SearchOption.AllDirectories).ToList());
            list.AddRange(Directory.GetFiles(eventDirectory, "*.mp3", SearchOption.AllDirectories).ToList());

            foreach (var file in list)
            {
                var directory = Path.GetDirectoryName(file);
                var fileOldName = Path.GetFileName(file);
                var newFilename = fileOldName.Replace("_", " ").Replace("-", " ").ToLower();
                var newFile = Path.Combine(directory,newFilename);
                File.Move(file, newFile);
            }
        }
    }
}
