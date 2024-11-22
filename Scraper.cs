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

            // Select the main content node or throw an exception if not found
            _contentNode = HAPExtension.SelectDivById(documentNode, "mw-content-text") 
                        ?? throw new NullReferenceException("The document node has no content.");

            // Clean up the content node by removing unnecessary sections
            RemoveUnnecessarySections(_contentNode);
            
        }
        catch (System.Exception e)
        {
            System.Console.WriteLine(url + "Caused Exeption\n" + e.Message);
            return;
        }
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
        IEnumerable<HtmlNode> textNodes = _contentNode.SelectNodes(".//p") ?? Enumerable.Empty<HtmlNode>(); // get all ps
        textNodes = textNodes.Concat(_contentNode.SelectNodes(".//ul") ?? Enumerable.Empty<HtmlNode>()); // get all uls and add them to the list

        return textNodes
            .Select(node => HtmlEntity.DeEntitize(node.InnerText.Trim())) // Clean and decode text
            .Where(text => !string.IsNullOrWhiteSpace(text))             // Filter out empty text
            .Aggregate("", (current, text) => current + " " + text);     // Concatenate into a single string
    }

    /// <summary>
    /// Removes unnecessary sections (e.g., TOC, Literatur, Math, References) from the content node.
    /// </summary>
    /// <param name="rootNode">The root HTML node to clean.</param>
    private static void RemoveUnnecessarySections(HtmlNode rootNode)
    {

        // Remove table of contents
        HAPExtension.RemoveDivById(rootNode, "toc");

        // Remove 'Literatur' section and its following nodes
        HtmlNode literaturNode = rootNode.SelectSingleNode("//*[@id='Literatur']");
        if (literaturNode != null)
        {
            HAPExtension.RemoveNodesByXPath(rootNode, "following::*");
            literaturNode.Remove();
        }

        // Remove math sections
        HAPExtension.RemoveNodesByXPath(rootNode, "//math");

        // Remove references
        HAPExtension.RemoveNodesByXPath(rootNode, "//*[@class='reference']");
    }
}
