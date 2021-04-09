using System;
using System.Collections.Generic;
using System.Text;
using SemanticProcessorLib.Models;

namespace SemanticProcessorLib
{
    public class SemanticImage
    {
        public List<WordScore> Words { get; set; }
        public string ImageCaption { get; set; }
        public string Color { get; set; }
    }
}
