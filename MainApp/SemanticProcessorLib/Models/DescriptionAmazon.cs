using System.Collections.Generic;

namespace SemanticProcessorLib.Models
{
    public class DescriptionAmazon
    {
        public string LabelModelVersion { get; set; }
        public List<Label> Labels { get; set; }
        public object OrientationCorrection { get; set; }
        public ResponseMetadata ResponseMetadata { get; set; }
        public int ContentLength { get; set; }
        public int HttpStatusCode { get; set; }
    }

    public class BoundingBox
    {
        public double Height { get; set; }
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
    }

    public class Instance
    {
        public BoundingBox BoundingBox { get; set; }
        public double Confidence { get; set; }
    }

    public class Parent
    {
        public string Name { get; set; }
    }

    public class Label
    {
        public double Confidence { get; set; }
        public List<Instance> Instances { get; set; }
        public string Name { get; set; }
        public List<Parent> Parents { get; set; }
    }

    public class Metadata
    {
    }

    public class ResponseMetadata
    {
        public string RequestId { get; set; }
        public Metadata Metadata { get; set; }
    }

}





