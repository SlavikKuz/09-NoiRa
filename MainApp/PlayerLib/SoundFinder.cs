using SemanticProcessorLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PlayerLib
{
    public class SoundFinder
    {
        private const string BacksDirectory = @"F:\Noiset\Backs\";
        private const string EventDirectory = @"F:\Noiset\Event\";

        private List<string> allBacksFiles { get; set; }
        private List<string> allEventFiles { get; set; }

        public List<string> BacksSoundLinks { get; set; }
        public List<string> EventSoundLinks { get; set; }

        public SoundFinder(SemanticImage results, bool processLib = false)
        {
            if(processLib)
                ProcessLib(30);

            var words = results.Words.Select(w => w.Word);

            allBacksFiles = Directory.GetFiles(BacksDirectory, "*.wav", SearchOption.AllDirectories).ToList();
            BacksSoundLinks = FindSoundFiles(words, allBacksFiles);

            allEventFiles = Directory.GetFiles(EventDirectory, "*.wav", SearchOption.AllDirectories).ToList();
            EventSoundLinks = FindSoundFiles(words, allEventFiles);
        }

        public void ProcessLib(int soundLength)
        {
            var soundLibProc = new SoundLibProcessor();
            soundLibProc.GetBacksMp3Samples(soundLength);
            soundLibProc.GetEventMp3Samples(soundLength);
            soundLibProc.RemoveBadFilesFromOrigin();
        }

        private List<string> FindSoundFiles(IEnumerable<string> imageWords, List<string> allFiles)
        {
            var soundFiles = new List<string>();

            foreach (var word in imageWords) 
                soundFiles.AddRange(FindSoundFile(word, allFiles));

            return soundFiles;
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
    }
}
