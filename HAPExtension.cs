using HtmlAgilityPack;

namespace WebScraper;

public static class HAPExtension
{
    public static HtmlNode? SelectDivById(HtmlNode node, string id){
        HtmlNodeCollection htmlNodeCollection = node.SelectNodes($"//div[@id='{id}']");
        if(htmlNodeCollection is null || htmlNodeCollection.Count == 0){
            return null;
        }
        return htmlNodeCollection[0];
    }

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

    public static void RemoveDivById(HtmlNode root, string id){
        SelectDivById(root, id)?.Remove();
    }

    public static void RemoveNodesByXPath(HtmlNode root, string xpath){
        RemoveAllNodes(root.SelectNodes(xpath));
    }
}
