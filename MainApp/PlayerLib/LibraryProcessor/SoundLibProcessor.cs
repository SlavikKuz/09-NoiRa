using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using NAudio.Lame;
using NAudio.Wave;
using PlayerLib.LibraryProcessor;

namespace PlayerLib
{
    public class SoundLibProcessor
    {
        private const string OriginWavDirectory = @"F:\sonniss\";
        private const string TargetMp3BacksDirectory = @"F:\Noiset\Backs\";
        private const string TargetMp3EventDirectory = @"F:\Noiset\Event\";
        private const string TargetWavBadDirectory = @"F:\Noiset\bad\";
        private const string WavFileDb = @"F:\Noiset\AllOriginFiles.txt";
        private const int SampleLength = 30;

        private List<string> _allWavFilesInOrigin;
        private List<string> _allWavFilesInDb;
        private List<string> _newWavFilesToDb;
        private List<string> _badWavOriginFiles;

        public SoundLibProcessor()
        {
            ProcessLib();
        }

        private void Get10SecSamples()
        {
            var mp3FilesOriginPath = Environment.CurrentDirectory + @"\wwwroot\source\";
            var mp3Files10secPath = Environment.CurrentDirectory + @"\wwwroot\source\10sec\";
            var mp3Files10secPathBad = Environment.CurrentDirectory + @"\wwwroot\source\10secBad\";
            var listAllMp3 = Directory.GetFiles(mp3FilesOriginPath, "*.mp3", SearchOption.AllDirectories).ToList();

            TimeSpan originFileLength;
            string targetWav10secPath;

            foreach (var fileMp3Path in listAllMp3)
            {
                targetWav10secPath = Path.ChangeExtension(mp3Files10secPath + Path.GetFileName(fileMp3Path), "wav");

                if (File.Exists(targetWav10secPath)) continue;

                try
                {
                    using (var reader = new Mp3FileReader(fileMp3Path))
                        originFileLength = reader.TotalTime.Duration();
                }
                catch (Exception e)
                {
                    File.Copy(fileMp3Path, mp3Files10secPathBad+Path.GetFileName(fileMp3Path));
                    continue;
                }

                var span = DetectSilence(fileMp3Path);
                var sampleLength = TimeSpan.FromSeconds(10);

                if (originFileLength == span || originFileLength <= sampleLength || originFileLength <= span+sampleLength)
                    span = TimeSpan.Zero;

                if (originFileLength <= sampleLength)
                    sampleLength = originFileLength;

                targetWav10secPath = Path.ChangeExtension(mp3Files10secPath + Path.GetFileName(fileMp3Path), "wav");
                TrimWavFile(fileMp3Path, targetWav10secPath, span, span+sampleLength);
            }
        }

        public void TrimWavFile(string fileMp3Path, string targetWav10secPath, TimeSpan startPoint, TimeSpan duration)
        {
            var tempUncutWav = Environment.CurrentDirectory + @"\wwwroot\source\10sec\temp.wav";

            using (var mp3 = new Mp3FileReader(fileMp3Path))
            {
                using (var pcm = WaveFormatConversionStream.CreatePcmStream(mp3))
                {
                    WaveFileWriter.CreateWaveFile(tempUncutWav, pcm);
                }
            }

            using (var reader = new WaveFileReader(tempUncutWav))
            {
                using (var writer = new WaveFileWriter(targetWav10secPath, reader.WaveFormat))
                {
                    var bytesPerMillisecond = reader.WaveFormat.AverageBytesPerSecond / 1000f;

                    var startBytes = (int)Math.Round(startPoint.TotalMilliseconds * bytesPerMillisecond);
                    startBytes = startBytes - startBytes % reader.WaveFormat.BlockAlign;
                    var startPos = startBytes;

                    startPos = startPos - startPos % reader.WaveFormat.BlockAlign;

                    var endBytes = (int)Math.Round(duration.TotalMilliseconds * bytesPerMillisecond);
                    endBytes = endBytes - endBytes % reader.WaveFormat.BlockAlign;
                    var endPos = endBytes;

                    TrimWavFile(reader, writer, startPos, endBytes);
                }
            }
        }

        private static void TrimWavFile(WaveFileReader reader, WaveFileWriter writer, int startPos, int endPos)
        {
            reader.Position = startPos;
            var buffer = new byte[reader.BlockAlign * 1024];
            while (reader.Position < endPos)
            {
                var bytesRequired = (int)(endPos - reader.Position);
                if (bytesRequired > 0)
                {
                    var bytesToRead = Math.Min(bytesRequired, buffer.Length);
                    var bytesRead = reader.Read(buffer, 0, bytesToRead);
                    if (bytesRead > 0)
                    {
                        writer.Write(buffer, 0, bytesRead);
                    }
                }
            }
        }

        private TimeSpan DetectSilence(string filePath)
        {
            /* https://www.generacodice.com/en/articolo/25833/Detecting-audio-silence-in-WAV-files-using-C */

            TimeSpan duration;
            using (var reader = new AudioFileReader(filePath))
            {
                duration = reader.GetSilenceDuration(AudioFileReaderExtension.SilenceLocation.Start);
            }

            return duration;
        }

        private void ProcessLib()
        {
            Get10SecSamples();

            if (CheckNewFilesOrigin())
                return;

            if(_newWavFilesToDb == null)
                GetNewFilesToDb();
            
            GetBacksMp3Samples(SampleLength);
            GetEventMp3Samples(SampleLength);
            RemoveBadFilesFromOrigin();
        }

        private void GetNewFilesToDb()
        {
            foreach (var file in _allWavFilesInOrigin)
            {
                if(!_allWavFilesInDb.Contains(file))
                    _newWavFilesToDb.Add(file);
            }
        }

        private bool CheckNewFilesOrigin()
        {
            _allWavFilesInOrigin = Directory.GetFiles(OriginWavDirectory, "*.wav", SearchOption.AllDirectories).ToList();

            if (!File.Exists(WavFileDb))
            {
                File.WriteAllLines(WavFileDb, _allWavFilesInOrigin);
                _newWavFilesToDb = _allWavFilesInOrigin;
            }

            _allWavFilesInDb = File.ReadAllLines(WavFileDb).ToList();

            return _allWavFilesInDb.Count == _allWavFilesInOrigin.Count;
        }

        private void GetBacksMp3Samples(int lengthSeconds)
        {
            var sampleDict = GetSampleLength().Where(s => s.Value >= lengthSeconds);
            
            foreach (var i in sampleDict)
            {
                var targetMp3File = TargetMp3BacksDirectory+Path.ChangeExtension(Path.GetFileName(i.Key), ".mp3");
                if (!File.Exists(targetMp3File))
                {
                    ConvertToMp3(i.Key, targetMp3File, LAMEPreset.EXTREME);
                }
            }
        }

        private void GetEventMp3Samples(int lengthSeconds)
        {
            var sampleDict = GetSampleLength().Where(s => s.Value < lengthSeconds);

            foreach (var i in sampleDict)
            {
                var targetMp3File = TargetMp3EventDirectory + Path.ChangeExtension(Path.GetFileName(i.Key), ".mp3");
                if (!File.Exists(targetMp3File))
                {
                    ConvertToMp3(i.Key, targetMp3File, LAMEPreset.EXTREME);
                }
            }
        }

        private void RemoveBadFilesFromOrigin()
        { 
        foreach (var i in _badWavOriginFiles)
        {
            var filePath = TargetWavBadDirectory + Path.GetFileName(i);
                if (!File.Exists(filePath))
                    File.Move(i, filePath);
                File.Delete(i);
            }
        }

        private Dictionary<string, int> GetSampleLength()
        {
            var sampleLength = new Dictionary<string, int>();
            var badList = new List<string>();

            for (var i = 0; i < _newWavFilesToDb.Count; i++)
            {
                try
                {
                    using (var reader = new WaveFileReader(_newWavFilesToDb[i]))
                        sampleLength.Add(_newWavFilesToDb[i], (int) reader.TotalTime.Duration().TotalSeconds);
                }
                catch (Exception ex)
                {
                    if(!badList.Contains(_newWavFilesToDb[i]))
                        badList.Add(_newWavFilesToDb[i]);
                }
            }

            _badWavOriginFiles = badList;
            return sampleLength;
        }

        private void ConvertToMp3(string sourceFileName, string targetMp3File, LAMEPreset preset)
        {
            var resampledFile = sourceFileName + ".res.wav";
            try
            {
                using (var reader = new MediaFoundationReader(sourceFileName))
                using (var resampler = new MediaFoundationResampler(reader, new WaveFormat(48000, 16, 2)))
                {
                    WaveFileWriter.CreateWaveFile(resampledFile, resampler);
                }

                using (var reader = new WaveFileReader(resampledFile))
                using (var writer = new LameMP3FileWriter(targetMp3File, reader.WaveFormat, preset))
                {
                    reader.CopyTo(writer);
                }
            }
            catch
            {
                File.Delete(targetMp3File);
            }
            finally
            {
                File.Delete(resampledFile);
            }
        }
    }
}
