using System.Collections.Generic;

namespace SemanticProcessorLib.Models
{
    public class DescriptionAzure
    {
        public List<Category> categories { get; set; }
        public Color color { get; set; }
        public List<Tag> tags { get; set; }
        public Description description { get; set; }
        public List<object> faces { get; set; }
        public List<Object> objects { get; set; }
        public string requestId { get; set; }
        public Metadata metadata { get; set; }

        public class Category
        {
            public string name { get; set; }
            public double score { get; set; }
            public Detail detail { get; set; }
        }

        public class Color
        {
            public string dominantColorForeground { get; set; }
            public string dominantColorBackground { get; set; }
            public List<string> dominantColors { get; set; }
            public string accentColor { get; set; }
            public bool isBwImg { get; set; }
            public bool isBWImg { get; set; }
        }

        public class Detail
        {
            public List<Landmark> landmarks { get; set; }
        }

        public class Landmark
        {
            public string name { get; set; }
            public double confidence { get; set; }
        }

        public class Tag
        {
            public string name { get; set; }
            public double confidence { get; set; }
        }

        public class Caption
        {
            public string text { get; set; }
            public double confidence { get; set; }
        }

        public class Description
        {
            public List<string> tags { get; set; }
            public List<Caption> captions { get; set; }
        }

        public class Rectangle
        {
            public int x { get; set; }
            public int y { get; set; }
            public int w { get; set; }
            public int h { get; set; }
        }

        public class Object
        {
            public Rectangle rectangle { get; set; }
            public string @object { get; set; }
            public double confidence { get; set; }
        }

        public class Metadata
        {
            public int height { get; set; }
            public int width { get; set; }
            public string format { get; set; }
        }
    }
}
