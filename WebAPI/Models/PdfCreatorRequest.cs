using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class PdfCreatorRequest
    {
        private List<ImageDescriptionToPdf> imageDescriptions { get; set; }
    }

    public class ImageDescriptionToPdf
    {
        public string AmazonJSON { get; set; }
        public string AzureJSON { get; set; }
        public string GoogleJSON { get; set; }
        public Bitmap Image { get; set; }
    }
}
