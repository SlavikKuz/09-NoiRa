using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace ComputerVisionLib
{
    public class VisorAzure
    {
        public string JSON { get; set; }

        public VisorAzure(MemoryStream imageStream, string azureKey, string azureEndPoint)
        {
            var uriBase = azureEndPoint + "vision/v3.1/analyze";
            JSON = MakeAnalysisRequest(uriBase, imageStream, azureKey).Result;
        }

        private async Task<string> MakeAnalysisRequest(string uriBase, MemoryStream fileStream, string azureKey)
        {
            var client = new HttpClient();

            client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", azureKey);
            var requestParameters = "visualFeatures=Categories,Description,Color,Objects,Tags,Faces";
            var uri = uriBase + "?" + requestParameters;

            HttpResponseMessage response;

            using (var content = new ByteArrayContent(fileStream.ToArray()))
            {
                content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
                response = await client.PostAsync(uri, content);
            }
            return await response.Content.ReadAsStringAsync();
        }

    }

}