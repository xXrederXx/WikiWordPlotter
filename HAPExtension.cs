using HtmlAgilityPack;

namespace WebScraper;

public static class HAPExtension
{
    /// <summary>
    /// This function returns a HtmlNode or null based on if a div with the spesified id was found.
    /// </summary>
    /// <param name="node">The base node to search from</param>
    /// <param name="id">The id to search</param>
    /// <returns></returns>
    public static HtmlNode? SelectDivById(HtmlNode node, string id)
    {
        HtmlNodeCollection htmlNodeCollection = node.SelectNodes($".//div[@id='{id}']");
        if (htmlNodeCollection is null || htmlNodeCollection.Count == 0)
        {
            return null;
        }
        return htmlNodeCollection[0];
    }

    /// <summary>
    /// This removes all given nodes
    /// </summary>
    /// <param name="nodes">A list of all the nodes to remove</param>
    public static void RemoveAllNodes(HtmlNodeCollection? nodes)
    {
        if (nodes is not null)
        {
            // Iterate through and remove each following node
            foreach (HtmlNode? node in nodes)
            {
                node.Remove();
            }
        }
    }

    /// <summary>
    /// Searches a div and removes it if found
    /// </summary>
    /// <param name="root">The node to search from</param>
    /// <param name="id">The id to search</param>
    public static void RemoveDivById(HtmlNode root, string id)
    {
        SelectDivById(root, id)?.Remove();
    }

    /// <summary>
    /// Removes all nodes which are selected by the xpath
    /// </summary>
    /// <param name="root">the root node to run the xpath from</param>
    /// <param name="xpath">The xpath used to search the other nodes</param>
    public static void RemoveNodesByXPath(HtmlNode root, string xpath)
    {
        RemoveAllNodes(root.SelectNodes(xpath));
    }
}
