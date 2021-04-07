using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Wave;

namespace SoundFinderLib
{
    public class SoundLibProcessor
    {
        private const string SoundSourceDirectory = @"f:\sonniss\";
        private const string SoundTargetDirectory = @"F:\BackNoRa\background\";
        private const string SoundFxTargetDirectory = @"F:\BackNoRa\fx\";
        private const string SoundMdlTargetDirectory = @"F:\BackNoRa\middle\";
        private const string SoundBadTargetDirectory = @"F:\BackNoRa\bad\";
        private List<string> allFiles { get; set; }
        private List<string> badFiles { get; set; }

        public SoundLibProcessor()
        {
            allFiles = Directory.GetFiles(SoundSourceDirectory, "*.wav", SearchOption.AllDirectories).ToList();
        }

        public void GetBackSamples(int lengthSeconds)
        {
            var sampleDict = GetSampleLength().Where(s => s.Value > lengthSeconds);

            foreach (var i in sampleDict)
            {
                if(!File.Exists(SoundTargetDirectory + Path.GetFileName(i.Key)))
                    File.Move(i.Key, SoundTargetDirectory+Path.GetFileName(i.Key));
                File.Delete(i.Key);
            }

            foreach (var i in badFiles)
            {
                if (!File.Exists(SoundBadTargetDirectory + Path.GetFileName(i)))
                    File.Move(i, SoundBadTargetDirectory + Path.GetFileName(i));
                File.Delete(i);
            }

            allFiles = Directory.GetFiles(SoundSourceDirectory, "*.wav", SearchOption.AllDirectories).ToList();
        }

        public void GetFxSamples(int lengthSeconds)
        {
            var sampleDict = GetSampleLength().Where(s => s.Value < lengthSeconds);

            foreach (var i in sampleDict)
            {
                if (!File.Exists(SoundFxTargetDirectory + Path.GetFileName(i.Key)))
                    File.Move(i.Key, SoundFxTargetDirectory + Path.GetFileName(i.Key));
                File.Delete(i.Key);
            }

            foreach (var i in badFiles)
            {
                if (!File.Exists(SoundBadTargetDirectory + Path.GetFileName(i)))
                    File.Move(i, SoundBadTargetDirectory + Path.GetFileName(i));
                File.Delete(i);
            }

            allFiles = Directory.GetFiles(SoundSourceDirectory, "*.wav", SearchOption.AllDirectories).ToList();
        }

        public void GetMidSamples(int backSec, int fxSec)
        {
            var sampleDict = GetSampleLength().Where(s => (s.Value <= backSec)&&(s.Value >= fxSec));

            foreach (var i in sampleDict)
            {
                if (!File.Exists(SoundMdlTargetDirectory + Path.GetFileName(i.Key)))
                    File.Move(i.Key, SoundMdlTargetDirectory + Path.GetFileName(i.Key));
                File.Delete(i.Key);
            }

            foreach (var i in badFiles)
            {
                if (!File.Exists(SoundBadTargetDirectory + Path.GetFileName(i)))
                    File.Move(i, SoundBadTargetDirectory + Path.GetFileName(i));
                File.Delete(i);
            }

            allFiles = Directory.GetFiles(SoundSourceDirectory, "*.wav", SearchOption.AllDirectories).ToList();
        }
        

        private Dictionary<string, int> GetSampleLength()
        {
            var sampleLength = new Dictionary<string, int>();
            var badList = new List<string>();

            for (var i = 0; i < allFiles.Count; i++)
            {
                try
                {
                    using (var reader = new WaveFileReader(allFiles[i]))
                        sampleLength.Add(allFiles[i], (int) reader.TotalTime.Duration().TotalSeconds);
                }
                catch (Exception ex)
                {
                    badList.Add(allFiles[i]);
                }
            }

            badFiles = badList;
            return sampleLength;
        }

        
    }
}
