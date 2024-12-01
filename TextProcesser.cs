namespace WebScraper;

public static class TextProcesser
{
    public static string[] GetWords(string text)
    {
        // Split the text into words
        string[] words = new string(text
                .ToLower() // Convert text to lowercase
                .Select(x => char.IsLetter(x) ? x : ' ') // Replace non-letters with space
                .ToArray()
            ) // Convert back to a string
            .Split(' ') // Split the string by spaces
            .Where(x => !string.IsNullOrEmpty(x) && !string.IsNullOrWhiteSpace(x)) // Filter out empty or whitespace entries
            .ToArray(); // Convert to a string array

        return words;
    }

}
