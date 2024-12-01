using Newtonsoft.Json;

namespace WebScraper;

public static class JsonUtility
{
    public static void SaveToJson(AppData appData, string path)
    {
        string data = JsonConvert.SerializeObject(appData);

        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.Write(data);
        }
    }
    public static AppData? ReadJson(string path)
    {
        if (!File.Exists(path)) return new([], [], []);

        string json;
        using (StreamReader sw = new StreamReader(path))
        {
            json = sw.ReadToEnd();
        }

        AppData? data = JsonConvert.DeserializeObject<AppData>(json);
        if(data is null) return null;

        // convert from optimized to normal
        string[] newURLCompletet = data.CompletedURLs.Select(x => UrlHandler.OptimizeURL(x).normal).ToArray();
        string[] newURLNotCompletet = data.NotCompletedURLs.Select(x => UrlHandler.OptimizeURL(x).normal).ToArray();

        return new AppData(data.WordCounts, newURLCompletet, newURLNotCompletet);
    }

    public static void ClearJson(string path)
    {
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.Write("");
        }
    }

    public static void OptimizeJsonFile(string path, int countThreshold, bool optimizeURLs)
    {
        AppData? data = ReadJson(path);
        if (data is null) return;

        // optimize the words
        kvpOptimized<string, int>[] newWordCounts = data.WordCounts
            .Where(x => x.v > countThreshold)
            .ToArray();

        string[] newURLCompletet = data.CompletedURLs;
        string[] newURLNotCompletet = data.NotCompletedURLs;
        if(optimizeURLs){
            newURLCompletet = newURLCompletet.Select(x => UrlHandler.OptimizeURL(x).optimized).ToArray();
            newURLNotCompletet = newURLNotCompletet.Select(x => UrlHandler.OptimizeURL(x).optimized).ToArray();
        }
        // cleare th file
        ClearJson(path);

        SaveToJson(new AppData(newWordCounts, newURLCompletet, newURLNotCompletet), path);
    }
}


public record AppData(kvpOptimized<string, int>[] WordCounts, string[] CompletedURLs, string[] NotCompletedURLs) { }

public struct kvpOptimized<TKey, TValue>{
    public TKey k;
    public TValue v;
    public kvpOptimized(TKey key, TValue value)
        {
            this.k = key;
            this.v = value;
        }

    public static explicit operator KeyValuePair<TKey, TValue>(kvpOptimized<TKey, TValue> kvp){
        return new KeyValuePair<TKey, TValue>(kvp.k, kvp.v);
    }
    public static explicit operator kvpOptimized<TKey, TValue>(KeyValuePair<TKey, TValue> kvp){
        return new kvpOptimized<TKey, TValue>(kvp.Key, kvp.Value);
    }
}
