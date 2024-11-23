using System.Collections.Concurrent;
using System.Diagnostics;

namespace WebScraper;

static class MainClass
{
    private const int AmountOfWordsOnPlot = 100;
    private static readonly ConcurrentDictionary<string, int> WordCounts = new ConcurrentDictionary<string, int>();
    private static readonly UrlHandler urlHandeler = new UrlHandler();
    public static void Main()
    {
        Console.WriteLine("Start App");

        ProcessURL("https://de.wikipedia.org/wiki/Parser"); // Just the startup URL you can change this
        
        Console.WriteLine("How Many Websites do you want to count");

        int websitesToCout;
        while(!int.TryParse(Console.ReadLine(), out websitesToCout)){
            System.Console.WriteLine("Write a number");
        }
        
        Stopwatch sw = new Stopwatch();
        sw.Start();

        Parallel.For(0, websitesToCout, (i) => {
            ProcessURL(urlHandeler.GetURL());
        });

        sw.Stop();
        Console.WriteLine(sw.ElapsedMilliseconds);

        List<KeyValuePair<string, int>> sortedWordList = WordCounts.OrderByDescending(x => x.Value).ToList();

        _ = new Plotter(sortedWordList, AmountOfWordsOnPlot);
    }

    private static void ProcessURL(string url)
    {
        if(string.IsNullOrEmpty(url)) return;

        Scraper scraper = new Scraper(url); // Scrap the Website


        string[] links = scraper.GetLinks(); // Process the links
        urlHandeler.AddURL(links);

        string wikiText = scraper.GetWikiText(); // Process the Words
        string[] words = TextProcesser.GetWords(wikiText);
        foreach (string word in words)
        {
            WordCounts.AddOrUpdate(word, 1, (key, count) => count + 1); // Thread-safe update
        }
    }
}
