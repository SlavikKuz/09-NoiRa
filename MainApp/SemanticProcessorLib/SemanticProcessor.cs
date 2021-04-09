using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using SemanticProcessorLib.Models;

namespace SemanticProcessorLib
{
    public class SemanticProcessor
    {
        private DescriptionAmazon _fromAmazon;
        private DescriptionAzure _fromAzure;
        private Dictionary<string,double> _fromGoogle;
        private List<string> _allWords;

        private List<WordScore> _words { get; set; }

        private static readonly string[] badWords = new[]
        {
            "pattern", "texture", "green", "grey", "outdoor", "nature", "outdoors", "land", "landscape", "line", "vehicle", "slope",
            "terrain", "peak", "scenery", "promontory", "azure", "waterway", "architecture", "building"
        };


        public SemanticProcessor(string fromAmazon, string fromAzure, Dictionary<string,double> fromGoogle)
        {
            _fromAmazon = JsonConvert.DeserializeObject<DescriptionAmazon>(fromAmazon);
            _fromAzure = JsonConvert.DeserializeObject<DescriptionAzure>(fromAzure);
            _fromGoogle = fromGoogle;
            _allWords = GetAlltWords();
            
            _words = new List<WordScore>();
            Process(_fromAmazon);
            Process(_fromAzure);
            Process(_fromGoogle);
        }

        public SemanticImage GetResult()
        {
            return new SemanticImage()
            {
                Words = ProcessWords(),
                Color = _fromAzure.color.accentColor,
                ImageCaption = _fromAzure.description.captions[0].text
            };
        }

        private List<string> GetAlltWords()
        {
            var list = new List<string>();
            foreach (var lable in _fromAmazon.Labels)
            {
                list.Add(lable.Name.ToLower());
            }
            foreach (var d in _fromGoogle)
            {
                list.Add(d.Key.ToLower());
            }
            foreach (var cat in _fromAzure.categories)
            {
                list.Add(cat.name.ToLower());
            }
            foreach (var tag in _fromAzure.tags)
            {
                list.Add(tag.name.ToLower());
            }
            return list;
        }

        private void Process(DescriptionAmazon _fromAmazon)
        {
            foreach (var lable in _fromAmazon.Labels)
            {
                _words.Add(new WordScore()
                {
                    Word = lable.Name.ToLower(),
                    Score = lable.Confidence,
                    Count = _allWords.Count(w => w.Equals(lable.Name.ToLower())),
                    Source = "Amazon"
                });
            }
        }

        private void Process(DescriptionAzure _fromAzure)
        {
            foreach (var cat in _fromAzure.categories)
            {
                _words.Add(new WordScore()
                {
                    Word = cat.name.ToLower(),
                    Score = cat.score*100,
                    Count = _allWords.Count(w => w.Equals(cat.name.ToLower())),
                    Source = "Azure"
                });
            }
            foreach (var tag in _fromAzure.tags)
            {
                _words.Add(new WordScore()
                {
                    Word = tag.name.ToLower(),
                    Score = tag.confidence*100,
                    Count = _allWords.Count(w => w.Equals(tag.name.ToLower())),
                    Source = "Azure"
                });
            }
        }

        private void Process(Dictionary<string,double> _fromGoogle)
        {
            foreach (var d in _fromGoogle)
            {
                _words.Add(new WordScore()
                {
                    Word = d.Key.ToLower(),
                    Score = d.Value*100,
                    Count = _allWords.Count(w => w.Equals(d.Key.ToLower())),
                    Source = "Google"
                });
            }
        }

        private List<WordScore> ProcessWords()
        {
            var distinct = _words.Where(w => w.Count > 1).Select(w=>w.Word).Distinct();
            var list = new List<WordScore>();
            list.AddRange(_words.Where(w=>w.Count<2));

            foreach (var word in distinct)
            {
                list.Add(_words.Where(w=>w.Word.Equals(word)).OrderByDescending(w=>w.Score).First());
            }

            return list.Where(w => w.Score >= 51)
                .Where(w=>!badWords.Contains(w.Word))
                .Where(w=>w.Word.Split(' ').Length<2)
                .Where(w => w.Word.Split('_').Length < 2)
                .OrderByDescending(w => w.Count)
                .ThenByDescending(w => w.Source)
                .ThenByDescending(w => w.Score).ToList();
        }
    }
}
