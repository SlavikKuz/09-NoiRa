using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using SemanticProcessorLib;
using SemanticProcessorLib.Models;

namespace PdfCreatorLib
{
    public static class TemplateProvider
    {
        public static string GetHTMLString(SemanticImage results, string imagePath)
        {
            var sb = new StringBuilder();

            sb.Append(@"
            <body style='background-color: #").Append(results.Color).Append(@"'>
            <div class='container mt-3'>
                <main role='main' class='pb-3'>
                    <div class='card'>
                        <div class='card-header'>
                            <div align='center'>")
                               //@foreach (var i in Model.ImageDescription)
                               //{<a> @i </a>}
                .Append(@"</div>
                        </div>
                        <div class='card-body'>
                            <figure class='card ml-1 mt-2'>
                                <div class='row'>
                                    <div class='column'>")
                           .AppendLine($"<img src=\"{imagePath}\" width='400'/>")
                           .Append(@"</div>
                                    <div class='column'>
                                        <div>
                                            <font size = '4'>");
            foreach (var i in results.Words)
            {
                sb.AppendLine($"<a> \"{i.Count + " " + i.Score.ToString("0.##") + " " + i.Source + " " + i.Word}\" </a> <br/>");
            }
            sb.AppendLine($"<br/>");

            sb.Append(@"                    </font>
                                        </div>
                                      </div>
                                 </div>
                            </figure>
                          </div>
                     </div>
                 </main>  
            </div>
            </body>
            ");

            return sb.ToString();
        }
    }
}
