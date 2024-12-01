namespace WebScraper;

public static class TextProcesser
{
    public static string[] GetWords(string text)
    {
        string[] words = text
            .Split(' ', StringSplitOptions.RemoveEmptyEntries) // Split into words
            .Select(word =>
                new string(word
                    .Where(char.IsLetter) // Keep only letters
                    .ToArray())
                .ToLower() // Convert back to char array
            )
            .Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x))
            .ToArray(); // Convert to a string array

        return words;
    }
}
