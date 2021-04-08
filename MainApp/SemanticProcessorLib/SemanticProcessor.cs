using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace SemanticProcessorLib
{
    public class SemanticProcessor
    {
        private string _color;
        private string _descriptionPhrase;
        public List<string> ImageDescription { get; set; }
        public string Color { get; set; }
        public string DescriptionPhrase { get; set; }
        private static readonly string[] badWords = new[] { "close", "and", "the", "with", "large", "text" };

        public static SemanticImage ProcessResults(string fromAmazon, string fromAzure, string fromGoogle)
        {
            return new SemanticImage();
        }

        public List<string> GetDistinctWords(string allWords)
        {
            var regExp = new Regex("[^a-zA-Z0-9]");
            allWords = regExp.Replace(allWords, " ");

            foreach (var i in badWords)
                allWords = allWords.Replace(" " + i + " ", " ");

            var words = allWords.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries)
                .ToList().OrderBy(x => x).ToList();

            var word_query = (from string word in words where word.Length > 2 orderby word select word).Distinct();

            return word_query.ToList();
        }

        private List<string> AnalyzeJson(string resultJsonString)
        {
            var imageDescription = JsonConvert.DeserializeObject<ImageDescription>(resultJsonString);
            _color = imageDescription.color.accentColor;
            _descriptionPhrase = imageDescription.description.captions.Aggregate(" ", (current, i) => current + (" " + i.text));

            var categoriesWords = imageDescription.categories.Aggregate(" ", (current, i) => current + (" " + i.name));
            var tagWords = imageDescription.tags.Aggregate(" ", (current, i) => current + (" " + i.name));
            var descriptionTagsWords = imageDescription.description.tags.Aggregate(" ", (current, i) => current + (" " + i));
            var descriptionCaptionsWords = imageDescription.description.captions.Aggregate(" ", (current, i) => current + (" " + i.text));

            var allWords = categoriesWords + tagWords + descriptionCaptionsWords + descriptionTagsWords;
            return GetDistinctWords(allWords);
        }

    }

}
