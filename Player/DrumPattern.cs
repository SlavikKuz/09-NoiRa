using System;
using System.Collections.Generic;
using System.Linq;

namespace PlayerLib
{
    public class DrumPattern
    {
        private readonly byte[,] _hits;
        private List<SoundScape> allSounds;
        public byte this[int note, int step] => _hits[note, step];
        public int Steps { get; }
        public int Notes { get; }

        public DrumPattern(List<SoundScape> soundScapes)
        {
            Notes = soundScapes.Count;
            Steps = soundScapes.Where(ss => ss.IsBack)
                .OrderByDescending(ss => ss.Steps).FirstOrDefault().Steps;
            allSounds = soundScapes;

            _hits = new byte[Notes, Steps];
            
            RandomizePattern();
        }

        private void RandomizePattern()
        {
            var rnd = new Random();

            for (var j = 0; j<allSounds.Count; j++)
            {
                if (allSounds[j].IsBack)
                    _hits[j, 0] = 127;
                else
                {
                    var soundLength = ++allSounds[j].Steps*2;

                    var firstHit = rnd.Next((int)(Steps * 0.1));
                        _hits[j, firstHit] = (byte)rnd.Next(70, 127);
                    
                    var numberOfHits = rnd.Next((int)((Steps / soundLength) * 0.4), Steps / soundLength);

                    if (soundLength < numberOfHits / 5)
                        numberOfHits /= 3;

                    numberOfHits /= 2;

                    var positions = new byte[numberOfHits];
                    
                    for (var y = 0; y < positions.Length; y++)
                    {
                        positions[y] = (byte)rnd.Next(firstHit, Steps);
                        firstHit = positions[y] + soundLength;

                        if(Steps-firstHit<soundLength*5)
                            break;
                    }

                    foreach (var t in positions)
                    {
                        _hits[j, t] = (byte)rnd.Next(70, 127);
                    }
                }
            }
        }
    }
}