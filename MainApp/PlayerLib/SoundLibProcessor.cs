using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Lame;
using NAudio.Wave;

namespace PlayerLib
{
    public class SoundLibProcessor
    {
        private const string SoundOrigin = @"F:\sonniss\";
        private const string TargetBacksDirectory = @"F:\Noiset\Backs\";
        private const string TargetEventDirectory = @"F:\Noiset\Event\";
        private const string TargetBadDirectory = @"F:\Noiset\bad\";

        private List<string> AllOriginWavFiles { get; set; }
        private List<string> BadOriginWavFiles { get; set; }

        public SoundLibProcessor()
        {
            AllOriginWavFiles = Directory.GetFiles(SoundOrigin, "*.wav", SearchOption.AllDirectories).ToList();
        }

        public void GetBacksMp3Samples(int lengthSeconds)
        {
            var sampleDict = GetSampleLength().Where(s => s.Value >= lengthSeconds);
            
            foreach (var i in sampleDict)
            {
                var targetMp3File = TargetBacksDirectory+Path.ChangeExtension(Path.GetFileName(i.Key), ".mp3");
                if (!File.Exists(targetMp3File))
                {
                    ConverTotMp3(i.Key, targetMp3File, LAMEPreset.EXTREME);
                }
            }
        }

        public void GetEventMp3Samples(int lengthSeconds)
        {
            var sampleDict = GetSampleLength().Where(s => s.Value < lengthSeconds);

            foreach (var i in sampleDict)
            {
                var targetMp3File = TargetEventDirectory + Path.ChangeExtension(Path.GetFileName(i.Key), ".mp3");
                if (!File.Exists(targetMp3File))
                {
                    ConverTotMp3(i.Key, targetMp3File, LAMEPreset.EXTREME);
                }
            }
        }

        public void RemoveBadFilesFromOrigin()
        { 
        foreach (var i in BadOriginWavFiles)
        {
            var filePath = TargetBadDirectory + Path.GetFileName(i);
                if (!File.Exists(filePath))
                    File.Move(i, filePath);
                File.Delete(i);
            }
        }

        private Dictionary<string, int> GetSampleLength()
        {
            var sampleLength = new Dictionary<string, int>();
            var badList = new List<string>();

            for (var i = 0; i < AllOriginWavFiles.Count; i++)
            {
                try
                {
                    using (var reader = new WaveFileReader(AllOriginWavFiles[i]))
                        sampleLength.Add(AllOriginWavFiles[i], (int) reader.TotalTime.Duration().TotalSeconds);
                }
                catch (Exception ex)
                {
                    if(!badList.Contains(AllOriginWavFiles[i]))
                        badList.Add(AllOriginWavFiles[i]);
                }
            }

            BadOriginWavFiles = badList;
            return sampleLength;
        }

        private void ConverTotMp3(string sourceFileName, string targetMp3File, LAMEPreset preset)
        {
            using (var reader = new AudioFileReader(sourceFileName))
            using (var writer = new LameMP3FileWriter(targetMp3File, reader.WaveFormat, preset))
            {
                reader.CopyTo(writer);
            }
        }
    }
}
