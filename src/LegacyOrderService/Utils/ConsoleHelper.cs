namespace LegacyOrderService.Utils;

public static class ConsoleHelper
{
    public static string GetValidStringInput(string prompt)
    {
        string? input = "";
        while (string.IsNullOrWhiteSpace(input))
        {
            Console.Write(prompt);
            input = Console.ReadLine();
        }
        return input;
    }

    public static long GetValidLongInput(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (long.TryParse(input, out long value))
            {
                if (value > 0)
                {
                    return value;
                }
                Console.WriteLine("Quantity must be greater than 0.");
            }
            else
            {
                Console.WriteLine("Invalid input. Please enter a valid number.");
            }
        }
    }

    public static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteWarning(string message)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ResetColor();
    }

    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ResetColor();
    }
}
