using System.Collections.Concurrent;
using System.Diagnostics;

namespace WebScraper;


static class MainClass
{
    private static readonly ConcurrentDictionary<string, int> WordCounts = new ConcurrentDictionary<string, int>();
    private static readonly UrlHandler urlHandeler = new UrlHandler();
    public static void Main()
    {
        Console.WriteLine("Start App");


        processURL("https://de.wikipedia.org/wiki/Parser");
        
        System.Console.WriteLine("How Many Websites do you want to count");
        int websitesToCout;
        while(!int.TryParse(Console.ReadLine(), out websitesToCout)){}
        
        Stopwatch sw = new Stopwatch();
        sw.Start();
        Parallel.For(0, websitesToCout, (i) => {
            processURL(urlHandeler.GetURL());
        });
        sw.Stop();
        System.Console.WriteLine(sw.ElapsedMilliseconds);
        List<KeyValuePair<string, int>> sorted = WordCounts.OrderByDescending(x => x.Value).ToList();
        _ = new Plotter(sorted, 100);
    }

    private static void processURL(string url)
    {
        if(string.IsNullOrEmpty(url)) return;

        Scraper scrapper = new Scraper(url);

        string wikiText = scrapper.GetWikiText();

        string[] words = TextProcesser.GetWords(wikiText);

        string[] links = scrapper.GetLinks();

        urlHandeler.AddURL(links);

        foreach (string word in words)
        {
            WordCounts.AddOrUpdate(word, 1, (key, count) => count + 1); // Thread-safe update
        }
    }
}
