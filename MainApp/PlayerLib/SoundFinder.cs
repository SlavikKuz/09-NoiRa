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

        private List<string> allBackFiles { get; set; }
        private List<string> allFxFiles { get; set; }

        public List<string> SoundLinksBacks { get; set; }
        public List<string> SoundLinksFX { get; set; }

        public SoundFinder(IEnumerable<string> imageWords, bool processLib)
        {
            if(processLib)
                ProcessLib(30);

            //MP3

            allBackFiles = Directory.GetFiles(BacksDirectory, "*.wav", SearchOption.AllDirectories).ToList();
            SoundLinksBacks = FindSoundFiles(imageWords, allBackFiles);

            allFxFiles = Directory.GetFiles(EventDirectory, "*.wav", SearchOption.AllDirectories).ToList();
            SoundLinksFX = FindSoundFiles(imageWords, allFxFiles);
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

            found.AddRange(allFiles.Where(f => 
                f.Replace("_"," ").Replace("-"," ").Contains(" "+word)).ToList());

            found.AddRange(allFiles.Where(f =>
                f.Replace("_", " ").Replace("-", " ").Contains(" " + word.ToUpper())).ToList());

            return found;
        }
    }
}
