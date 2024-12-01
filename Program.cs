using System.Collections.Concurrent;
using System.Diagnostics;

namespace WebScraper;

static class MainClass
{
    private const int AmountOfWordsOnPlot = 100;
    private static readonly ConcurrentDictionary<string, int> WordCounts = new ConcurrentDictionary<string, int>();
    private static readonly UrlHandler urlHandeler = new UrlHandler();

    private const string halfTab = "\x2001\x2001";
    public static void Main(string[] args)
    {
        List<string> argsList = args.ToList();
        AppArguments appArguments= new AppArguments("https://de.wikipedia.org/wiki/Parser", "./save.json", "./save.json", "./plot.png");


        if (args.Contains("-h") || args.Contains("--help"))
        {
            Console.WriteLine("-c   --count <int>           how many websites get counted. You will be ask again if you dont pass anything.");
            Console.WriteLine("-h   --help                  show this menu");
            Console.WriteLine("-i   --image ?<Path>         Save a plotted Graph as an image. The path must be .png");
            Console.WriteLine("-l   --load ?<Path>          Load the preveousely collected data. The Path must be to a .json file");
            Console.WriteLine("-o  --optimize <int> <bool>  Defines the threshold of the data safed. if it is 2 and a word occured once, it wont");
            Console.WriteLine("                             get stored. The bool tells if the links are optimized when stored");
            Console.WriteLine("-s   --save ?<Path>          Save the collected data. The Path must be to a .json file");
            Console.WriteLine("-su  --start-url <URL>       Define a start Url. Please Not that it will need to be de.wikipedia.org links");

            return;
        }
        if (args.Contains("-su") || args.Contains("--start-url"))
        {
            try
            {
                string newStartUrl = args[argsList.IndexOf("-su") + 1 + argsList.IndexOf("--start-url") + 1];
                // this just cheks if the URL is valide
                if (urlHandeler.IsUrlValide(newStartUrl))
                {
                    appArguments.startUrl = newStartUrl;
                }      
            }
            catch (IndexOutOfRangeException){}
        }
        if (args.Contains("-s") || args.Contains("--save"))
        {
            appArguments.saveData = true;

            try
            {
                string newSavePath = args[argsList.IndexOf("-s") + 1 + argsList.IndexOf("--save") + 1];
    
                // check if it is json file
                if (Path.GetExtension(newSavePath) == ".json")
                {
                    appArguments.savePath = newSavePath;
                }
            }
            catch (IndexOutOfRangeException){}

        }
        if (args.Contains("-l") || args.Contains("--load"))
        {
            appArguments.loadData = true;

            try
            {
                string newLoadPath = args[argsList.IndexOf("-l") + 1 + argsList.IndexOf("--load") + 1];
    
                // check if it is json file
                if (Path.GetExtension(newLoadPath) == ".json")
                {
                    appArguments.loadPath = newLoadPath;
                }
            }
            catch (IndexOutOfRangeException){}
        }
        if (args.Contains("-c") || args.Contains("--count"))
        {
            try
            {
                string newCount = args[argsList.IndexOf("-c") + 1 + argsList.IndexOf("--count") + 1];
                _ = int.TryParse(newCount, out appArguments.count);
            }
            catch (IndexOutOfRangeException){}
        }
        if (args.Contains("-o") || args.Contains("--optimize"))
        {
            try
            {
                string newoptimizationThreshold = args[argsList.IndexOf("-o") + 1 + argsList.IndexOf("--optimize") + 1];
                string newoptimizeURL = args[argsList.IndexOf("-o") + 1 + argsList.IndexOf("--optimize") + 1 + 1];
                _ = int.TryParse(newoptimizationThreshold, out appArguments.optimizationThreshold);
                _ = bool.TryParse(newoptimizeURL, out appArguments.optimizeURLs);
            }
            catch (IndexOutOfRangeException){}
        }
        if (args.Contains("-i") || args.Contains("--image"))
        {
            appArguments.saveImg = true;

            try
            {
                string newImgPath = args[argsList.IndexOf("-i") + 1 + argsList.IndexOf("--image") + 1];
    
                // check if it is json file
                if (Path.GetExtension(newImgPath) == ".png")
                {
                    appArguments.imagePath = newImgPath;
                }
            }
            catch (IndexOutOfRangeException){}
        }

        RunApp(appArguments);
    }
    private static void RunApp(AppArguments args)
    {
        ConsoleUtility.ColoredMessage[] text =
        [
            new($"Settings:\n{halfTab}Start Url:\t\t"), new(args.startUrl),
            new($"\n{halfTab}Websites to Count:\t"), new(args.count.ToString(), args.count == 0 ? ConsoleColor.DarkGray : ConsoleColor.White), 
            new($"\n{halfTab}Optimization:\t\t"), new(args.optimizationThreshold.ToString(), args.optimizationThreshold == 0 ? ConsoleColor.DarkGray : ConsoleColor.White), new("\t(URLs)" + halfTab), new(args.optimizeURLs.ToString(), args.optimizeURLs ? ConsoleColor.Green : ConsoleColor.Red),
            new($"\n{halfTab}Do Save Data:\t\t"), new(args.saveData.ToString(), args.saveData ? ConsoleColor.Green : ConsoleColor.Red), new("\t(Path)" + halfTab), new(args.savePath),
            new($"\n{halfTab}Do Load Data:\t\t"), new(args.loadData.ToString(), args.loadData ? ConsoleColor.Green : ConsoleColor.Red), new("\t(Path)" + halfTab), new(args.loadPath),
            new($"\n{halfTab}Do Save Plot:\t\t"), new(args.saveImg.ToString(), args.saveImg ? ConsoleColor.Green : ConsoleColor.Red), new("\t(Path)" + halfTab), new(args.imagePath),
            new($"\n\nWrite -h or --help to see the arguments.\n")
        ];
        ConsoleUtility.WriteMany(text);

        if (args.loadData)
        {
            LoadData(args.loadPath);
        }

        ProcessURL(args.startUrl); // Just the startup URL you can change this

        if(args.count == 0){
            Console.WriteLine("How Many Websites do you want to count");

            while (!int.TryParse(Console.ReadLine(), out args.count))
            {
                System.Console.WriteLine("Write a number");
            }
        }

        Stopwatch sw = new Stopwatch();
        sw.Start();

        Parallel.For(0, args.count, (i) =>
        {
            ProcessURL(urlHandeler.GetURL());
        });

        sw.Stop();
        Console.WriteLine("time -> " + sw.ElapsedMilliseconds + "ms");
        Console.WriteLine("words -> " + WordCounts.Keys.Count);

        if(args.saveImg){
            List<KeyValuePair<string, int>> sortedWordList = WordCounts.OrderByDescending(x => x.Value).ToList();
            _ = new Plotter(sortedWordList, AmountOfWordsOnPlot, args.imagePath);
        }

        if (args.saveData)
        {
            JsonUtility.SaveToJson(new AppData(WordCounts.Select(x => (kvpOptimized<string, int>)x).ToArray(), urlHandeler.CompletedURLs, urlHandeler.NotCompletedURLs), args.savePath);
            JsonUtility.OptimizeJsonFile(args.savePath, args.optimizationThreshold, args.optimizeURLs);
        }
    }
    private static void LoadData(string path)
    {
        AppData? data = JsonUtility.ReadJson(path);
        if (data is null) return;

        foreach (KeyValuePair<string, int> word in data.WordCounts)
        {
            WordCounts.AddOrUpdate(word.Key, word.Value, (key, count) => count + word.Value); // Thread-safe update
        }

        urlHandeler.LoadData(data.CompletedURLs, data.NotCompletedURLs);
    }

    private static void ProcessURL(string url)
    {
        if (string.IsNullOrEmpty(url)) return;

        Scraper scraper = new Scraper(url); // Scrap the Website


        string[] links = scraper.GetLinks(); // Process the links
        urlHandeler.AddURLs(links);

        string wikiText = scraper.GetWikiText(); // Process the Words
        string[] words = TextProcesser.GetWords(wikiText);
        foreach (string word in words)
        {
            WordCounts.AddOrUpdate(word, 1, (key, count) => count + 1); // Thread-safe update
        }
    }
}

public class AppArguments(string StartUrl, string SavePath, string LoadPath, string ImagePath)
{
    public string startUrl = StartUrl;
    public string savePath = SavePath;
    public string loadPath = LoadPath;
    public string imagePath = ImagePath;
    public bool saveImg;
    public bool saveData;
    public bool loadData;
    public bool optimizeURLs;
    public int count;
    public int optimizationThreshold;

}
