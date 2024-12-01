using HtmlAgilityPack;

namespace WebScraper;

public class Scraper
{
    private readonly HtmlNode _contentNode;

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
    /// Retrieves all links from the content node. Valid or not
    /// </summary>
    /// <returns>An array of absolute URLs.</returns>
    public string[] GetLinks()
    {
        return _contentNode.SelectNodes(".//a")?
            .Select(link => link.GetAttributeValue("href", ""))
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

        return string.Join(' ',
            textNodes
            .Select(node => HtmlEntity.DeEntitize(node.InnerText)) // Clean and decode text
            .Where(text => !string.IsNullOrWhiteSpace(text))// Filter out empty text
        );             
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

        // Remove nodes sections
        HAPExtension.RemoveNodesByXPath(rootNode, "//math");
        HAPExtension.RemoveNodesByXPath(rootNode, "//code");

        // Remove nodes class
        HAPExtension.RemoveNodesByXPath(rootNode, "//*[@class='NavFrame']");
        HAPExtension.RemoveNodesByXPath(rootNode, "//*[@class='NavContent']");
        HAPExtension.RemoveNodesByXPath(rootNode, "//*[@class='reference']");

        HAPExtension.RemoveNodesByXPath(rootNode, "//*[contains(@style, 'display: none;')]");
    }
}
