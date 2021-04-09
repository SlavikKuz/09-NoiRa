using System;
using System.Collections.Generic;
using System.Text;

namespace SemanticProcessorLib.Models
{
    public class WordScore
    {
        public string Word { get; set; }
        public double Score { get; set; }
        public int Count { get; set; }
        public string Source { get; set; }
    }
}
