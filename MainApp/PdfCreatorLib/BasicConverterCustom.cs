using DinkToPdf;

namespace PdfCreatorLib
{
    public class BasicConverterCustom
    {
        private static BasicConverter _instance;
        
        private BasicConverterCustom()
        {
        }

        public static BasicConverter Instance => _instance = _instance ?? new BasicConverter(new PdfTools());
    }
}