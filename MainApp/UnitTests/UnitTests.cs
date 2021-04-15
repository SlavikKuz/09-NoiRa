using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices;
using ComputerVisionLib;
using ImageProviderLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using SemanticProcessorLib;
using SemanticProcessorLib.Models;

namespace UnitTests
{
    [TestClass]
    public class UnitTests
    {
        private static readonly string _testFolder = @"D:\Repos\09 - Noiset\MainApp\WebAPI\wwwroot\source\tests\";

        [TestMethod]
        public void TestComputerVision()
        {
            var imageFile = _testFolder + DateTime.Now.ToString("u")
                .Replace(":","-").Replace("Z","") + ".png";
            var textFile = _testFolder + DateTime.Now.ToString("u")
                .Replace(":","-").Replace("Z", "") + ".txt";

            var imageProvider = new ImageProvider(imageFile);
            var computerVision = new ComputerVision(imageProvider.ImageStream, new KeysLib());

            var semanticProcessor = new SemanticProcessor(computerVision.Results);
            var semanticResult = semanticProcessor.GetResult();

            var azureRaw = semanticResult.Words.Where(w => w.Source.Equals("Azure"));
            var googleRaw = semanticResult.Words.Where(w => w.Source.Equals("Amazon"));
            var amazonRaw = semanticResult.Words.Where(w => w.Source.Equals("Google"));
            var azureString = string.Empty;
            var googleString = string.Empty;
            var amazontring = string.Empty;

            foreach (var line in azureRaw.OrderByDescending(w=>w.Count).ThenByDescending(w=>w.Score))
            {
                azureString += line.Count + " " + line.Word + ":" + line.Score.ToString() + ", ";
            }
            foreach (var line in googleRaw.OrderByDescending(w => w.Count).ThenByDescending(w => w.Score))
            {
                googleString += line.Count + " " + line.Word + ":" + line.Score.ToString() + ", ";
            }
            foreach (var line in amazonRaw.OrderByDescending(w => w.Count).ThenByDescending(w => w.Score))
            {
                amazontring += line.Count + " " + line.Word + ":" + line.Score.ToString() + ", ";
            }

            var mainWords = semanticResult.Words.Where(w => w.Count > 1).Select(w => w.Word).Distinct();
            var boldWords = string.Empty;
            foreach (var w in mainWords)
            {
                boldWords += w + ", ";
            }

            var synonyms = new List<string>();
            var synString = string.Empty;

            var client = new HttpClient();
            var listSyn = new List<string>();

            //https://rapidapi.com/ipeirotis/api/wikisynonyms



            var endPoints = new List<string>()
            {
                "/hasParts","/hasMembers",
                "/substanceOf","/hasSubstances","/inCategory", "/synonyms"
                //,"/typeOf","/partOf","/instanceOf","/hasInstances","/hasTypes","/hasCategories","/similarTo","/also","/entails","/memberOf",
            };

            var endPointsResult = new List<string>();

            foreach (var point in endPoints)
            {
                endPointsResult.Add("-----" + point);

                foreach (var w in mainWords)
                {
                    var endPointString = string.Empty;

                    var linkStr = new Uri(@"https://wordsapiv1.p.rapidapi.com/words/" + w + point);

                    var request = new HttpRequestMessage
                    {
                        Method = HttpMethod.Get,
                        RequestUri = linkStr,
                        Headers =
                        {
                            {"x-rapidapi-key", "487bf4f07dmshbedfa5448b44c4bp1d9080jsn475ddc3a38dd"},
                            {"x-rapidapi-host", "wordsapiv1.p.rapidapi.com"}
                        },
                    };

                    using (var response = client.SendAsync(request).Result)
                    {
                        response.EnsureSuccessStatusCode();
                        var body = response.Content.ReadAsStringAsync().Result;

                        var result = JsonConvert.DeserializeObject<Synonim>(body);

                        if (result.synonyms != null)
                            foreach (var word in result.synonyms)
                            {
                                listSyn.Add(word);
                            }

                        var collection = 
                            result.typeOf ?? result.hasTypes ?? result.partOf ?? result.hasParts ?? result.instanceOf ?? result.hasInstances ??
                            result.similarTo ?? result.synonyms ?? result.also ?? result.entails ?? result.memberOf ?? result.hasMembers ?? 
                            result.substanceOf ?? result.hasSubstances ?? result.inCategory ?? result.hasCategories ?? null;

                        if (collection != null)
                            foreach (var c in collection)
                            {
                                endPointString += c + ", ";
                            }
                    }

                    endPointsResult.Add("   " + w);
                    endPointsResult.Add(endPointString);

                    foreach (var l in listSyn)
                    {
                        synonyms.Add(l);
                    }
                }


            }

            var clearSynList = synonyms.Distinct();
            
            foreach (var w in clearSynList)
            {
                synString += w + ", ";
            }

            var relevant = new List<WordScore>();
            var notRelev = new List<WordScore>();
            
            foreach (var w in semanticResult.Words)
            {
                if(clearSynList.Contains(w.Word.ToLower().Replace("_","")))
                    relevant.Add(w);
                else
                    notRelev.Add(w);
            }

            var relString = string.Empty;
            foreach (var w in relevant)
            {
                relString += w.Word +": " + w.Score + ", ";
            }

            var notRelString = string.Empty;
            foreach (var w in notRelev)
            {
                notRelString += w.Word + ": " + w.Score + ", ";
            }

            var list = new List<string>()
            {
                "----------------Azure----------------------",
                azureString,
                "----------------Google---------------------",
                googleString,
                "----------------Amazon---------------------",
                amazontring,
                "----------------Bold words-----------------",
                boldWords,
                "----------------Synonyms-------------------",
                synString,
                "----------------Relevant-------------------",
                relString,
                "----------------NotRelevant----------------",
                notRelString
            };

            list.AddRange(endPointsResult);

            File.AppendAllLines(textFile, list);
        }

        public class Synonim
        {
            public string word { get; set; }
            public List<string> synonyms { get; set; }
            public List<string> similarTo { get; set; }
            public List<string> typeOf { get; set; }
            public List<string> hasTypes { get; set; }
            public List<string> partOf { get; set; }
            public List<string> hasParts { get; set; }
            public List<string> instanceOf { get; set; }
            public List<string> hasInstances { get; set; }
            public List<string> also { get; set; }
            public List<string> entails { get; set; }
            public List<string> memberOf { get; set; }
            public List<string> hasMembers { get; set; }
            public List<string> substanceOf { get; set; }
            public List<string> hasSubstances { get; set; }
            public List<string> hasCategories { get; set; }
            public List<string> inCategory { get; set; }
        }
    }
}
