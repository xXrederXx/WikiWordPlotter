using System.Collections.Concurrent;

namespace WebScraper;
public class UrlHandler
{
    private readonly ConcurrentBag<string> Completed = new();
    private readonly ConcurrentQueue<string> NotCompleted = new();

    public string[] CompletedURLs => Completed.ToArray();
    public string[] NotCompletedURLs => NotCompleted.ToArray();

    public const string BaseUrl = "https://de.wikipedia.org";
    public const string RelativeBaseUrl = "/wiki/";

    public void LoadData(string[] completed, string[] notCompleted)
    {

        foreach (string url in completed)
        {
            Completed.Add(url);
        }

        foreach (string url in notCompleted)
        {
            NotCompleted.Enqueue(url);
        }
    }

    /// <summary>
    /// Fetches the next URL from the NotCompleted queue.
    /// </summary>
    /// <returns>The URL if available; otherwise, an empty string.</returns>
    public string GetURL()
    {
        if (!NotCompleted.TryDequeue(out string? url))
        {
            return string.Empty;
        }

        if (url.StartsWith(RelativeBaseUrl))
        {
            url = BaseUrl + url;
        }

        Completed.Add(url);
        return url;
    }

    /// <summary>
    /// Adds a single URL to the NotCompleted queue if it's not already processed.
    /// <summary>
    /// <param name="url">The URL to add.</param>
    public void AddURL(string url)
    {
        if (IsUrlValide(url))
        {
            NotCompleted.Enqueue(url);
        }
    }

    /// <summary>
    /// Adds multiple URLs to the NotCompleted queue.
    /// </summary>
    /// <param name="urls">An array of URLs to add.</param>
    public void AddURLs(IEnumerable<string> urls)
    {
        foreach (string url in urls)
        {
            AddURL(url);
        }
    }


    public bool IsUrlValide(string url)
    {
        return !string.IsNullOrEmpty(url) && !url.Contains("Datei:") && (url.StartsWith(BaseUrl) || url.StartsWith(RelativeBaseUrl)) && !Completed.Contains(url) && !NotCompleted.Contains(url);
    }
}
