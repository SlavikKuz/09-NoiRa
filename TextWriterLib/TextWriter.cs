using DeepAI;

namespace TextWriterLib
{
    public class TextWriter
    {
        private readonly string apiKey = "quickstart-QUdJIGlzIGNvbWluZy4uLi4K";
        public string Text { get; set; }
        
        public TextWriter(string phrase)
        {
            var api = new DeepAI_API(apiKey);
            var response = api.callStandardApi("text-generator", new { text = phrase });
            Text = response.output.ToString();

            var rr = Text.Split(@"\n\n");
        }
    }
}
