using System.IO;
using DinkToPdf;

namespace PdfCreatorLib
{
    public class PdfCreator
    {
        private readonly byte[] _pdf;

        public PdfCreator()
        {
            var converter = BasicConverterCustom.Instance;

            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.Letter,
                Margins = new MarginSettings { Top = 10, Left = 10, Right = 10, Bottom = 10 },
                DocumentTitle = "PDF Report"
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = TemplateProvider.GetHTMLString(),
                WebSettings =
                {
                    DefaultEncoding = "utf-8",
                    UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css")
                },
                HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = { objectSettings }
            };

            _pdf = converter.Convert(pdf);
        }

        public void ToFile(string filePath)
        {
            File.WriteAllBytes(filePath, _pdf);
        }
    }
}