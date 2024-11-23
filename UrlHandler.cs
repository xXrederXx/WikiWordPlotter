using System.Collections.Concurrent;

namespace WebScraper;

public class UrlHandler
{
    private readonly ConcurrentDictionary<string, bool> Completed = new();
    private readonly ConcurrentQueue<string> NotCompleted = new();

    /// <summary>
    /// Fetches the next URL from the NotCompleted queue.
    /// </summary>
    /// <returns>The URL if available; otherwise, an empty string.</returns>
    public string GetURL()
    {
        if (NotCompleted.TryDequeue(out string? url))
        {
            Completed.TryAdd(url, true);
            return url;
        }
        return string.Empty;
    }

    /// <summary>
    /// Adds a single URL to the NotCompleted queue if it's not already processed.
    /// </summary>
    /// <param name="url">The URL to add.</param>
    public void AddURL(string url)
    {
        if (!Completed.ContainsKey(url) && !NotCompleted.Contains(url))
        {
            NotCompleted.Enqueue(url);
        }
    }

    /// <summary>
    /// Adds multiple URLs to the NotCompleted queue.
    /// </summary>
    /// <param name="urls">An array of URLs to add.</param>
    public void AddURL(IEnumerable<string> urls)
    {
        foreach (string url in urls)
        {
            AddURL(url);
        }
    }
}
