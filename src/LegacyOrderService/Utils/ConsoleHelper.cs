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

    public static void PrintWelcome()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("""
         _                                     ____          _              _____                 _          
        | |                                   / __ \        | |            / ____|               (_)         
        | |     ___  __ _  __ _  ___ _   _   | |  | |_ __ __| | ___ _ __  | (___   ___ _ ____   ___  ___ ___ 
        | |    / _ \/ _` |/ _` |/ __| | | |  | |  | | '__/ _` |/ _ \ '__|  \___ \ / _ \ '__\ \ / / |/ __/ _ \
        | |___|  __/ (_| | (_| | (__| |_| |  | |__| | | | (_| |  __/ |     ____) |  __/ |   \ V /| | (_|  __/
        |______\___|\__, |\__,_|\___|\__, |   \____/|_|  \__,_|\___|_|    |_____/ \___|_|    \_/ |_|\___\___|
                     __/ |            __/ |                                                                  
                    |___/            |___/                                                                           
        """);
        Console.WriteLine("-----------------------------------------");
        Console.WriteLine("     Welcome to Order Processor!      ");
        Console.WriteLine("-----------------------------------------");
        Console.ResetColor();
        Console.WriteLine(); 
    }
}
