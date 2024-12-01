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

        return JsonConvert.DeserializeObject<AppData>(json);
    }

    public static void ClearJson(string path)
    {
        using (StreamWriter sw = new StreamWriter(path))
        {
            sw.Write("");
        }
    }

    public static void OptimizeJsonFile(string path, int threshold)
    {
        AppData? data = ReadJson(path);
        if (data is null) return;

        KeyValuePair<string, int>[] newWordCounts = data.WordCounts.Where(x => x.Value > threshold).ToArray();

        ClearJson(path);

        SaveToJson(new AppData(newWordCounts, data.CompletedURLs, data.NotCompletedURLs), path);
    }

}


public record AppData(KeyValuePair<string, int>[] WordCounts, string[] CompletedURLs, string[] NotCompletedURLs) { }
