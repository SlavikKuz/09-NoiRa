using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using PdfCreator.Utility;

namespace PdfCreator.Controllers
{
    [Route("api/pdfcreator")]
    [ApiController]
    public class PdfCreatorController : ControllerBase
    {
        private IConverter _converter;

        public PdfCreatorController(IConverter converter)
        {
            _converter = converter;
        }

        [HttpGet]
        public HttpResponseMessage CreatePDF()
        {
            var response = new HttpResponseMessage(HttpStatusCode.BadRequest);
            var pdfFile = EncodeToPDF();

            if (pdfFile != null)
            {
                var contentLength = pdfFile.Length;

                response.StatusCode = HttpStatusCode.OK;
                response.Content = new StreamContent(new MemoryStream(pdfFile));
                response.Content.Headers.ContentLength = contentLength;
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response.Content.Headers.ContentDisposition = new ContentDispositionHeaderValue("attachment")
                { 
                    FileName = "Result.pdf"
                };
            }
            else
            {
                response.StatusCode = HttpStatusCode.NotFound;
            }
            return response;
        }

        private byte[] EncodeToPDF()
        {
            var globalSettings = new GlobalSettings
            {
                ColorMode = ColorMode.Color,
                Orientation = Orientation.Portrait,
                PaperSize = PaperKind.A4,
                Margins = new MarginSettings {Top = 10},
                DocumentTitle = "PDF Report"
            };
            var objectSettings = new ObjectSettings
            {
                PagesCount = true,
                HtmlContent = TemplateGenerator.GetHTMLString(),
                WebSettings =
                {
                    DefaultEncoding = "utf-8",
                    UserStyleSheet = Path.Combine(Directory.GetCurrentDirectory(), "assets", "styles.css")
                },
                HeaderSettings = {FontName = "Arial", FontSize = 9, Right = "Page [page] of [toPage]", Line = true},
                FooterSettings = {FontName = "Arial", FontSize = 9, Line = true, Center = "Report Footer"}
            };
            var pdf = new HtmlToPdfDocument()
            {
                GlobalSettings = globalSettings,
                Objects = {objectSettings}
            };

            return _converter.Convert(pdf);
        }
    }
}
