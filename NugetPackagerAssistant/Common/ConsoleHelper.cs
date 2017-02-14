using System;

namespace NugetPackagerAssistant.Common
{
    public static class ConsoleHelper
    {
        public static ConsoleKey PromptChoices(ConsoleKey firstPossibility, ConsoleKey secondPossility)
        {
            ConsoleKey usersChoice;
            do
            {
                Console.WriteLine("Type your choice [{0}/{1}] :", firstPossibility, secondPossility);
                usersChoice = Console.ReadKey(false).Key;
                if (usersChoice != ConsoleKey.Enter)
                    Console.WriteLine();
            } while (usersChoice != firstPossibility && usersChoice != secondPossility);

            return usersChoice;
        }
    }
}
