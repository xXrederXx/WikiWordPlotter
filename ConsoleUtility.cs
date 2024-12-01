namespace WebScraper;

public static class ConsoleUtility
{
    public record ColoredMessage(string Message, ConsoleColor TextColor = ConsoleColor.White);

    public static void WriteLine(string message, ConsoleColor textColor)
    {
        Console.ForegroundColor = textColor;
        Console.WriteLine(message);
        Console.ResetColor();
    }
    public static void WriteLine(ColoredMessage message) => WriteLine(message.Message, message.TextColor);

    public static void Write(string message, ConsoleColor textColor)
    {
        Console.ForegroundColor = textColor;
        Console.Write(message);
        Console.ResetColor();
    }
    public static void Write(ColoredMessage message) => Write(message.Message, message.TextColor);

    public static void WriteMany(ColoredMessage[] messages)
    {
        foreach (ColoredMessage message in messages)
        {
            Write(message.Message, message.TextColor);
        }
    }
}