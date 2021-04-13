using System;
using System.Collections.Generic;
using System.Text;
using NAudio.Wave;

namespace ComputerHearingLib
{
    static class AudioFileReaderExtension
    {
        public enum SilenceLocation { Start, End }

        private static bool IsSilence(float amplitude, sbyte threshold)
        {
            var dB = 20 * Math.Log10(Math.Abs(amplitude));
            return dB < threshold;
        }
        public static TimeSpan GetSilenceDuration(this AudioFileReader reader, SilenceLocation location, sbyte silenceThreshold = -40)
        {
            var counter = 0;
            var volumeFound = false;
            var eof = false;
            var oldPosition = reader.Position;

            var buffer = new float[reader.WaveFormat.SampleRate * 4];
            while (!volumeFound && !eof)
            {
                var samplesRead = reader.Read(buffer, 0, buffer.Length);
                if (samplesRead == 0)
                    eof = true;

                for (var n = 0; n < samplesRead; n++)
                {
                    if (IsSilence(buffer[n], silenceThreshold))
                    {
                        counter++;
                    }
                    else
                    {
                        if (location == SilenceLocation.Start)
                        {
                            volumeFound = true;
                            break;
                        }
                        if (location == SilenceLocation.End)
                        {
                            counter = 0;
                        }
                    }
                }
            }
            reader.Position = oldPosition;

            var silenceSamples = (double)counter / reader.WaveFormat.Channels;
            var silenceDuration = (silenceSamples / reader.WaveFormat.SampleRate) * 1000;
            return TimeSpan.FromMilliseconds(silenceDuration);
        }
    }
}
