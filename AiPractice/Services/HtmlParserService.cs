using HtmlAgilityPack;

public class HtmlParserService
{
    public async Task<string> ExtractTextFromUrlAsync(string url)
    {
        var web = new HtmlWeb();
        var doc = await Task.Run(() => web.Load(url));

        return doc.DocumentNode.InnerText.Trim();
    }
}