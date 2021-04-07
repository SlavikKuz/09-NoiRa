using System.Drawing;

namespace WebAPI.Models
{
    public class DataItem
    {
        public string AzureDescription { get; set; }
        public string GoogleDescription { get; set; }
        public string AmazonDescription { get; set; }
        public Bitmap Image { get; set; }
    }
}
