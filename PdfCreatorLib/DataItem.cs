using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace PdfCreatorLib
{
    public class DataItem
    {
        public string AzureDescription { get; set; }
        public string GoogleDescription { get; set; }
        public string AmazonDescription { get; set; }
        public Bitmap Image { get; set; }
    }
}
