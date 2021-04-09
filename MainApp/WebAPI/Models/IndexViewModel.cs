using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebAPI.Models
{
    public class IndexViewModel
    {
        public byte[] Image { get; set; }
        public List<string> ImageDescription { get; set; }
        public List<string> BackSounds { get; set; }
        public List<string> LinksToPlay { get; set; }
        public string ImageCaptions { get; set; }
        public string Color { get; set; }
    }
}
