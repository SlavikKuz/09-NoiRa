using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace SoundFinderLib
{
    public class SoundFinder
    {
        private const string SoundFxDirectory = @"F:\BackNoRa\fx\";
        private const string SoundBacksDirectory = @"F:\BackNoRa\background\";

        private List<string> allBackFiles { get; set; }
        private List<string> allFxFiles { get; set; }

        public List<string> SoundLinksBacks { get; set; }
        public List<string> SoundLinksFX { get; set; }

        public SoundFinder(IEnumerable<string> imageWords, bool processLib)
        {
            if(processLib)
                ProcessLib(60, 10);

            allBackFiles = Directory.GetFiles(SoundBacksDirectory, "*.wav", SearchOption.AllDirectories).ToList();
            SoundLinksBacks = FindSoundFiles(imageWords, allBackFiles);

            allFxFiles = Directory.GetFiles(SoundFxDirectory, "*.wav", SearchOption.AllDirectories).ToList();
            SoundLinksFX = FindSoundFiles(imageWords, allFxFiles);
        }

        public void ProcessLib(int secForBack, int secForFx)
        {
            var soundLibProc = new SoundLibProcessor();
            soundLibProc.GetBackSamples(secForBack);
            soundLibProc.GetFxSamples(secForFx);
            soundLibProc.GetMidSamples(secForBack, secForFx);
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

            found.AddRange(allFiles.Where(f => 
                f.Replace("_"," ").Replace("-"," ").Contains(" "+word)).ToList());

            found.AddRange(allFiles.Where(f =>
                f.Replace("_", " ").Replace("-", " ").Contains(" " + word.ToUpper())).ToList());

            return found;
        }
    }
}
