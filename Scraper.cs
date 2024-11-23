using System.Diagnostics;
using HtmlAgilityPack;

namespace WebScraper;

public class Scraper
{
    private readonly HtmlNode _contentNode;
    private const string BaseUrl = "https://de.wikipedia.org";

    public Scraper(string url)
    {
        HtmlNode documentNode;
        try
        {
            // Load the web page
            documentNode = new HtmlWeb().Load(url).DocumentNode;
            
        }
        catch (System.Exception)
        {
            System.Console.WriteLine(url + "Caused Aggregate Exeption" );
            return;
        }

        // Select the main content node or throw an exception if not found
        _contentNode = HAPExtension.SelectDivById(documentNode, "mw-content-text") 
                       ?? throw new NullReferenceException("The document node has no content.");

        // Clean up the content node by removing unnecessary sections
        RemoveUnnecessarySections(_contentNode);
    }

    /// <summary>
    /// Retrieves all valid links from the content node.
    /// </summary>
    /// <returns>An array of absolute URLs.</returns>
    public string[] GetLinks()
    {
        return _contentNode.SelectNodes(".//a")?
            .Select(link => link.GetAttributeValue("href", ""))
            .Where(href => !string.IsNullOrEmpty(href) && !href.Contains("Datei:") && (href.Contains("wikipedia") || href.Contains("wiki"))) // Filter out empty links and file links
            .Select(href => href.StartsWith("https://") ? href : BaseUrl + href)      // Ensure links are absolute
            .Distinct()                                                           // Remove duplicates
            .ToArray() 
            ?? Array.Empty<string>(); // Return an empty array if no links are found
    }

    /// <summary>
    /// Retrieves the combined text content of all <p> and <ul> nodes.
    /// </summary>
    /// <returns>A string containing the combined text content.</returns>
    public string GetWikiText()
    {
        // Select <p> and <ul> nodes and combine their inner text
        var textNodes = _contentNode.SelectNodes(".//p") ?? Enumerable.Empty<HtmlNode>();
        textNodes = textNodes.Concat(_contentNode.SelectNodes(".//ul") ?? Enumerable.Empty<HtmlNode>());

        return textNodes
            .Select(node => HtmlEntity.DeEntitize(node.InnerText.Trim())) // Clean and decode text
            .Where(text => !string.IsNullOrWhiteSpace(text))             // Filter out empty text
            .Aggregate("", (current, text) => current + " " + text);     // Concatenate into a single string
    }

    /// <summary>
    /// Removes unnecessary sections (e.g., TOC, Literatur, Math, References) from the content node.
    /// </summary>
    /// <param name="htmlNode">The root HTML node to clean.</param>
    private static void RemoveUnnecessarySections(HtmlNode htmlNode)
    {
        // Helper to remove nodes by ID or XPath
        void RemoveNodeById(string id) => HAPExtension.SelectDivById(htmlNode, id)?.Remove();
        void RemoveNodesByXPath(string xpath) =>
            HAPExtension.RemoveAllNodes(htmlNode.SelectNodes(xpath));

        // Remove table of contents
        RemoveNodeById("toc");

        // Remove 'Literatur' section and its following nodes
        var literaturNode = htmlNode.SelectSingleNode("//*[@id='Literatur']");
        if (literaturNode != null)
        {
            RemoveNodesByXPath("following::*");
            literaturNode.Remove();
        }

        // Remove math sections
        RemoveNodesByXPath("//math");

        // Remove references
        RemoveNodesByXPath("//*[@class='reference']");
    }
}
