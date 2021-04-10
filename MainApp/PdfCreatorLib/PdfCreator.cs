using System.Drawing;
using System.IO;
using DinkToPdf;
using SemanticProcessorLib;
using SemanticProcessorLib.Models;

namespace PdfCreatorLib
{
    public class PdfCreator
    {
        private byte[] _pdf;
        private readonly ObjectSettings _objectSettings;
        private readonly GlobalSettings _globalSettings;
        private readonly BasicConverter _converter;

        public PdfCreator(SemanticImage results, string imagePath, string filePath)
        {
            var pdfStylesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "source", "PdfStyles.css");

            _converter = BasicConverterCustom.Instance;

            _globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings { Top = 0, Left = 0, Right = 0, Bottom = 0 },
                DocumentTitle = "PDF Report"
            };

            _objectSettings = new ObjectSettings
            {
                PagesCount = true,
                WebSettings =
                {
                    DefaultEncoding = "utf-8",
                    UserStyleSheet = pdfStylesPath
                },
                //HeaderSettings = { FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true },
                //FooterSettings = { FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer" }
            };

            _objectSettings.HtmlContent = TemplateProvider.GetHTMLString(results, imagePath);

            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = _globalSettings,
                Objects = { _objectSettings }
            };

            _pdf = _converter.Convert(pdf);
            File.WriteAllBytes(filePath, _pdf);
        }
    }
}