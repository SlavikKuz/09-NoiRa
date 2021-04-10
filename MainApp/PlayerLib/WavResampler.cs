using System.IO;
using NAudio.Wave;

namespace PlayerLib
{
    public class WavResampler
    {
        private string _fileLink;
        private readonly int selectedBitDepth = 24; //{ "Unchanged", "8", "16", "24", "IEEE float" };
        private readonly int selectedChannels = 2; //{ "Unchanged", "mono", "stereo" };
        private readonly int selectedSampleRate = 48000; //{ 8000, 16000, 22050, 32000, 44100, 48000, 88200, 96000 };

        public WavResampler(string link)
        {
            _fileLink = link;
        }

        public void Resample()
        {
            if(!Resample(_fileLink))
                return;

            File.Delete(_fileLink);
            File.Move(_fileLink + ".wav", _fileLink);
        }

        private bool Resample(string link)
        {
            using (var reader = new MediaFoundationReader(link))
            {
                if (IsCorrectFormat(reader.WaveFormat))
                    return false;

                using (var resampler = new MediaFoundationResampler(reader,
                    new WaveFormat(selectedSampleRate, selectedBitDepth, selectedChannels)))
                {
                    WaveFileWriter.CreateWaveFile(link + ".wav", resampler);
                }
            }
            return true;
        }

        private bool IsCorrectFormat(WaveFormat format)
        {
            return format.Channels==selectedChannels && format.SampleRate == selectedSampleRate && format.BitsPerSample == selectedBitDepth;
        }

    }
}
