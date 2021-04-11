using System;
using System.Collections.Generic;
using System.Linq;
using NAudio.Wave;

namespace PlayerLib
{
    public class DrumKit
    {
        private readonly List<SampleSource> sampleSources;
        public virtual WaveFormat WaveFormat { get; }
        public List<SoundScape> SoundScapes { get; set; }
        public List<string> LinksToPlay { get; set; } = new List<string>();

        public DrumKit(List<string> soundLinksBack, List<string> soundLinksFx)
        {

            //TO DO: consider adjusted
            var randomSelectedBack = GetRandomBack(soundLinksBack);
            var randomSelectedFx = GetRandomFx(soundLinksFx);
            
            LinksToPlay.Add(randomSelectedBack);
            LinksToPlay.AddRange(randomSelectedFx);

            var soundScape = new List<SoundScape>();
            soundScape.Add( new SoundScape() { Link = randomSelectedBack, IsBack = true });
            soundScape.AddRange(randomSelectedFx.Select(soundLink => new SoundScape() { Link = soundLink, IsBack = false }).ToList());

            foreach (var i in soundScape)
            {
                i.SoundSample = SampleSource.CreateFromMp3File(i.Link);
                var reader = new Mp3FileReader(i.Link);
                i.Steps = (int) reader.TotalTime.Duration().TotalSeconds * 2;
            }

            sampleSources = soundScape.Select(c => c.SoundSample).ToList();

            WaveFormat = WaveFormat.CreateIeeeFloatWaveFormat(soundScape[0].SoundSample.SampleWaveFormat.SampleRate,
                soundScape[0].SoundSample.SampleWaveFormat.Channels);

            SoundScapes = soundScape;
        }

        public MusicSampleProvider GetSampleProvider(int note)
        {
            return new MusicSampleProvider(sampleSources[note]);
        }

        private static string GetRandomBack(List<string> soundLinksBack)
        {
            var rnd = new Random();
            return soundLinksBack.Count == 1 ? soundLinksBack[0] : soundLinksBack[rnd.Next(0, soundLinksBack.Count)];
        }

        private static List<string> GetRandomFx(List<string> soundLinksFx)
        {

            var selectedFx = new List<string>();
            var rnd = new Random();

            var sampleCount = soundLinksFx.Count()%7;

            if (sampleCount <= 3 && soundLinksFx.Count >= 7)
                sampleCount = rnd.Next(4, 7);
            else
                sampleCount = soundLinksFx.Count;

            var randomLine = new List<byte>();
            byte randomPoint;

            for (var i = 0; i < sampleCount; i++)
            {
                do
                {
                    randomPoint = (byte) rnd.Next(0, soundLinksFx.Count);
                } while (randomLine.Contains(randomPoint));
                
                randomLine.Add(randomPoint);
            }

            foreach (var ind in randomLine)
            {
                selectedFx.Add(soundLinksFx[ind]);
            }

            return selectedFx;
        }
    }

    public class SoundScape
    {
        public SampleSource SoundSample { get; set; }
        public int Steps { get; set; }
        public string Link { get; set; }
        public bool IsBack { get; set; }
    }
}